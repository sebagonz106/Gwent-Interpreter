using System;
using System.Collections.Generic;
using System.Text;

namespace Gwent_Interpreter
{
    class Interptreter
    {
        Lexer lexer;
        Parser parser;

        public void Evaluate(string input)
        {
            List<Expression> expressions = parser.Parse(lexer.Tokenize(input));
        }
    }
}
