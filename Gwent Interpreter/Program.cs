using System;
using System.Collections.Generic;
using Gwent_Interpreter.Expressions;
using System.IO;

namespace Gwent_Interpreter
{
    class Program
    {
        static void Main(string[] args)
        {
            //Atom<num> atom = new Atom<num>("26", new num(26));
            //Atom<num> atom1 = new Atom<num>("2", new num(2));
            //Atom<bool> at = new Atom<bool>("false", false);
            //BooleanOperation sum = new BooleanOperation(at, at, "||");
            //EqualityOperation
            //Console.WriteLine(sum);
            //Console.WriteLine(sum.CheckSemantic());
            //Console.WriteLine(sum.Evaluate());
            //"effect \n\n { Name: \"test\", \n\nParams: { Amount: Number}, \n\nAction: (targets, context) => log Amount * 2 + 4; \n\n} \n\neffect \n\n{ Name: \"test1\", Params: {Amount: Number},\n\n Action: (targets, context) => log Amount*2+4; } \n\ncard { Name: \"belga\", Type: \"Oro\", Range: \"Melee\", Faction: \"Fidel\", Power: 2 ^ 2 ^ 2, \n\nOnActivation: [{Effect: { Name: \"test\", Amount: \"testing1234\".ToString().Length}, \n\nSelector: { Source: \"board\", Predicate: (unit) => true}, PostAction: { Type: \"test1\", Amount: 2 \n\n}}\n\n] \n\n}\n\n";


            while (true)
            {
                Interptreter interptreter = new Interptreter(null, new List<string> { "test" });
                string input = File.ReadAllText("D:\\Gwent-Pro\\Gwent-Interpreter\\Gwent Interpreter\\Utils\\Interpreter.txt");
                interptreter.Evaluate(input);
                Console.ReadKey();
                Console.Clear();
            }
        }
    }
}
