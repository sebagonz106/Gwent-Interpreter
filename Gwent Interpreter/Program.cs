using System;
using System.Collections.Generic;
using Gwent_Interpreter.Expressions;

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

            while (true) //TODO: wrappear string
            {
                Interptreter interptreter = new Interptreter();
                string input = Console.ReadLine();
                interptreter.Evaluate(input);
                Console.ReadKey();
                Console.Clear();
            }
        }
    }
}
