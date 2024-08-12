using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Gwent_Interpreter.Statements;
using Gwent_Interpreter.Expressions;

namespace Gwent_Interpreter
{
    class Interptreter
    {
        Lexer lexer = new Lexer();
        Parser parser;
        string mainPath = "D:\\";
        bool validLoad = true;

        public Interptreter(List<string> previousCards = null, List<string> previousEffects = null, string path = "")
        {
            if (path != "") mainPath = path;

            foreach (var item in previousEffects)
            {
                StreamReader sr = new StreamReader(mainPath + "Effects" + item + ".gwf");
                this.Evaluate(sr.ReadLine());
                sr.Close();
            }

            foreach (var item in previousCards)
            {
                StreamReader sr = new StreamReader(mainPath + "Cards" + item + ".gwc");
                if (!this.Evaluate(sr.ReadLine()))
                {
                    Log("Invalid load of previous declarations. There is an unloaded effect used in a card.");
                    validLoad = false;
                }
                sr.Close();
            }
        }

        public bool Evaluate(string input)
        {
            if (!validLoad) return false;
            //input = "++ + += CUba45 es _lomejor Amount class  card que3_3 le ha 5ucedid0 //al effect  mundo porque \n 45+12==6 is false  amount ha vemaria 45.98/0.9!=0;{\"cualifaier#$'`~ de icpc\n voy a ti\n\" si esto p1mcha soy feli555.";
            //input = "25.48*9.8-4*(2.7+4.3)";
            //input = "2+2!=24";
            //input = "2+-2*(-2)";
            //input = "true==false";
            //input = "effect { Action: { wifiActivada = true; tiempoDeEspera = 0; while(wifiActivada) {tiempoDeEspera+=2; if(tiempoDeEspera==10) wifiActivada = !wifiActivada; log tiempoDeEspera;}}}"
            //input = "effect { Name: "test", Params: {Amount: Number}, Action: (targets, context) => log Amount; }";
            //input = "card { Name: "belga", Type: "Oro", Range: "Melee", Faction: "Fidel", Power: 2^2^2, OnActivation: [{Effect:{Name: "test", Amount: 4}, Selector: {Source: "board", Predicate: (unit) => true}}] }"
            //input = "effect { Name: "test", Params: {Amount: Number}, Action: (targets, context) => log Amount.ToString().Length; } card { Name: "belga", Type: "Oro", Range: "Melee", Faction: "Fidel", Power: 2^2^2, OnActivation: [{Effect:{Name: "test", Amount: "testing".ToString().Length}, Selector: {Source: "board", Predicate: (unit) => true}}] }";
            //input = effect { Name: "test", Params: {Amount: Number}, Action: (targets, context) => log Amount*2+4; } card { Name: "belga", Type: "Oro", Range: "Melee", Faction: "Fidel", Power: 2^2^2, OnActivation: [{Effect:{Name: "test", Amount: "testing1234".ToString().Length}, Selector: {Source: "board", Predicate: (unit) => true}}] }

            List<Token> list = lexer.Tokenize(input, out string[] lexicalErrors);

            if (lexicalErrors.Length > 0)
            {
                for (int i = 0; i < lexicalErrors.Length; i++)
                {
                    Log($"{i+1}. {lexicalErrors[i]}");
                }
                return false;
            }
            else
            {
                parser = new Parser(list);

                IStatement program = parser.Parse();

                if (parser.Errors.Count > 0)
                {
                    foreach (var error in parser.Errors) Log(error);
                    return false;
                }
                else
                {
                    List<string> semanticErrors = new List<string>();

                    try
                    {
                        program.CheckSemantic(out semanticErrors);
                    }
                    catch (Warning warning)
                    {
                        Log(warning.Message);
                    }

                    if (semanticErrors.Count > 0)
                    {
                        foreach (var error in semanticErrors) Log(error);
                        return false;
                    }
                    else
                    {
                        try
                        {
                            program.Execute();
                        }
                        catch (MyException error)
                        {
                            Log(error.Message);
                            return false;
                        }
                    }
                }
                foreach (var item in CardStatement.Cards) //testing
                {
                    item.Effect(Player.Fidel.context);
                }
            }
            return true;
        }

        void Log(string text) => Console.WriteLine(text);
    }
}