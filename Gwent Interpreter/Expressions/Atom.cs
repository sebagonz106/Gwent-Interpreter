using System;
using System.Collections.Generic;
using System.Text;

namespace Gwent_Interpreter.Expressions
{
    class Atom : Expr<object>
    {
        Token token;

        public Atom(Token token)
        {
            this.token = token;
        }

        public override bool CheckSemantic()
        {
            return true;
        }

        public override object Evaluate()
        {
            switch (token.Type)
            {
                case TokenType.Number:
                    return new Num(Convert.ToDouble(token.Value));
                case TokenType.String:
                    return token.Value.Substring(1,token.Value.Length-2);
                case TokenType.True:
                    return true;
                case TokenType.False:
                    return false;
                default:
                    return null;
            }
        }

        public override string ToString() => token.Value.ToString();
    }
}
