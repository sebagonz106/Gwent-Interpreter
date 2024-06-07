using System;
using System.Collections.Generic;
using System.Text;

namespace Gwent_Interpreter
{
    class Interptreter
    {
        Lexer lexer = new Lexer();
        Parser parser = new Parser();

        public void Evaluate(string input)
        {
            List<Token> list = lexer.Tokenize(input);
            foreach (Token item in list)
            {
                Console.WriteLine(item);
            }

            List<Expression> expressions = parser.Parse(lexer.Tokenize(input));
        }
    }
}
