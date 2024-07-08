using System;
using System.Collections.Generic;
using System.Text;
using Gwent_Interpreter.Statements;
using Gwent_Interpreter.Expressions;

namespace Gwent_Interpreter
{
    class Interptreter
    {
        Lexer lexer = new Lexer();
        Parser parser;

        public void Evaluate(string input)
        {
            //input = "++ + += CUba45 es _lomejor Amount class  card que3_3 le ha 5ucedid0 //al effect  mundo porque \n 45+12==6 is false  amount ha vemaria 45.98/0.9!=0;{\"cualifaier#$'`~ de icpc\n voy a ti\n\" si esto p1mcha soy feli555.";
            //input = "25.48*9.8-4*(2.7+4.3)";
            //input = "2+2!=24";
            //input = "2+-2*(-2)";
            //input = "true==false";
            //input = "effect { Action: { wifiActivada = true; tiempoDeEspera = 0; while(wifiActivada) {tiempoDeEspera+=2; if(tiempoDeEspera==10) wifiActivada = !wifiActivada; log tiempoDeEspera;}}}"

            List<Token> list = lexer.Tokenize(input, out string[] lexicalErrors);
            if (lexicalErrors.Length > 0)
            {
                for (int i = 0; i < lexicalErrors.Length; i++)
                {
                    Console.WriteLine($"{i+1}. {lexicalErrors[i]}");
                }
            }
            else
            {
                //foreach (Token item in list)
                //{
                //    Console.WriteLine(item);
                //}

                parser = new Parser(list);
                List<IStatement> statements = parser.Parse();
                while (parser.Errors.Count!=0)
                {
                    Console.WriteLine(parser.Errors[0]);
                    parser.Errors.RemoveAt(0);
                }
                foreach (var item in statements)
                {
                    if (item is Declaration) continue;

                    try
                    {
                        item.Execute();
                    }
                    catch (EvaluationError error)
                    {
                        Console.WriteLine(error.Message);
                    }
                }
            }
        }
    }
}
