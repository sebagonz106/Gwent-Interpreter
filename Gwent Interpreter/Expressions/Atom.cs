using System;
using System.Collections.Generic;
using System.Text;
using Gwent_Interpreter.Statements;
using Gwent_Interpreter.GameLogic;

namespace Gwent_Interpreter.Expressions
{
    class Atom : Expr<object> //use inheritance
    {
        Token token;
        Declaration declaration;
        Callable callable;
        object obj;

        public Atom(Token token)
        {
            this.token = token;
        }

        public Atom(Declaration declaration)
        {
            this.declaration = declaration;
        }

        public Atom (Callable callable)
        {
            this.callable = callable;
        }

        public Atom(object obj) => this.obj = obj;

        public override bool CheckSemantic(out string error)
        {
            error = "";
            return true;
        }

        public override ReturnType Return
        {
            get
            {
                if (!(token is null))
                {
                    switch (token.Type)
                    {
                        case TokenType.Number:
                            return ReturnType.Num;
                        case TokenType.String:
                            return ReturnType.String;
                        case TokenType.True:
                            return ReturnType.Bool;
                        case TokenType.False:
                            return ReturnType.Bool;
                        default:
                            throw new EvaluationError($"Invalid value at {token.Coordinates.Item1}:{token.Coordinates.Item2 + token.Value.Length - 1}");
                    }
                }
                else if (!(declaration is null)) return declaration.Return;
                else if (!(callable is null)) return callable.Return;
                else if (!(obj is null)) return (obj is int || obj is double) ? ReturnType.Num : obj is string ? ReturnType.String : obj is GwentList ? ReturnType.List : obj is Card ? ReturnType.Card : ReturnType.Object;
                else throw new EvaluationError($"Invalid value at { token.Coordinates.Item1 }:{ token.Coordinates.Item2 + token.Value.Length - 1} ");
                
            }
        }

        public override object Evaluate()
        {
            if (!(token is null))
            {
                switch (token.Type)
                {
                    case TokenType.Number:
                        return new Num(Convert.ToDouble(token.Value));
                    case TokenType.String:
                        return token.Value.Substring(1, token.Value.Length - 2);
                    case TokenType.True:
                        return true;
                    case TokenType.False:
                        return false;
                    default:
                        throw new EvaluationError($"Invalid value at {token.Coordinates.Item1}:{token.Coordinates.Item2 + token.Value.Length - 1}");
                }
            }
            else if (!(declaration is null)) return declaration.ExecuteAndGiveValue().Evaluate();
            else if (!(callable is null)) return callable.Evaluate();
            else return obj;
        }

        public override string ToString() => token.Value?? declaration.ToString()?? callable.ToString()?? obj.ToString();
    }
}
