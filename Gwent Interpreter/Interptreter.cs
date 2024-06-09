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
            input = "++ + += CUba45 es _lomejor Amount class  card que3_3 le ha 5ucedid0 //al effect  mundo porque \n 45+12==6 is false  amount ha vemaria 45.98/0.9!=0;{\"cualifaier#$'`~ de icpc\n voy a ti\n\" si esto p1mcha soy feli555.";

            List<Token> list = lexer.Tokenize(input, out string[] errors);
            if (errors.Length > 0)
            {
                for (int i = 0; i < errors.Length; i++)
                {
                    Console.WriteLine($"{i+1}. {errors[i]}");
                }
            }
            else
            {
                foreach (Token item in list)
                {
                    Console.WriteLine(item);
                }

                List<IExpression<object>> expressions = parser.Parse(list);
            }
        }
    }
}
