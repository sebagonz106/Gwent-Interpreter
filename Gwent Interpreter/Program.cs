using System;
using System.Collections.Generic;
using Gwent_Interpreter.Expressions;

namespace Gwent_Interpreter
{
    class Program
    {
        static void Main(string[] args)
        {
            Atom<num> atom = new Atom<num>("26", new num(26));
            Atom<bool> at = new Atom<bool>("true", true);
            ArithmeticOperation sum = new ArithmeticOperation(atom, atom, "+");
            Console.WriteLine(sum);
            Console.WriteLine(sum.CheckSemantic());
            Console.WriteLine(sum.Evaluate());

            Interptreter interptreter = new Interptreter();
            string input = Console.ReadLine();
            interptreter.Evaluate(input);
        }
    }
}
