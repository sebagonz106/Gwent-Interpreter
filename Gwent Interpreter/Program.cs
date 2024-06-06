using System;
using System.Collections.Generic;

namespace Gwent_Interpreter
{
    class Program
    {
        static void Main(string[] args)
        {
            Interptreter interptreter = new Interptreter();
            string input = "";
            interptreter.Evaluate(input);
        }
    }
}
