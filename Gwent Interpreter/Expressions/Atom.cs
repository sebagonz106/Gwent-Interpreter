using Gwent_Interpreter.GameLogic;
using Gwent_Interpreter.Statements;
using System;
using System.Collections.Generic;
using Gwent_Interpreter.Utils;

namespace Gwent_Interpreter.Expressions
{
    abstract class Atom<T> : Expr<T>
    {
        protected T value;
        public override (int, int) Coordinates { get; protected set; }

        public override bool CheckSemantic(out List<string> errors)
        {
            errors = new List<string>();
            return true;
        }
        public override bool CheckSemantic(out string error)
        {
            error = "";
            return true;
        }

        public override string ToString() => value.ToString();
    }

    class ValueAtom : Atom<Token>
    {
        public ValueAtom(Token value)
        {
            this.value = value;
            this.Coordinates = value.Coordinates;
        }

        public override ReturnType Return
        {
            get
            {
                switch (value.Type)
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
                        throw new EvaluationError($"Invalid value at {value.Coordinates.Item1}:{value.Coordinates.Item2 + value.Value.Length - 1}");
                }
            }
        }


        public override object Evaluate()
        {
            switch (value.Type)
            {
                case TokenType.Number:
                    return new Num(Convert.ToDouble(value.Value));
                case TokenType.String:
                    return value.Value.Substring(1, value.Value.Length - 2);
                case TokenType.True:
                    return true;
                case TokenType.False:
                    return false;
                default:
                    throw new EvaluationError($"Invalid value at {value.Coordinates.Item1}:{value.Coordinates.Item2 + value.Value.Length - 1}");
            }
        }
    }

    class DeclarationAtom : Atom<Declaration>
    {
        public DeclarationAtom(Declaration value)
        {
            this.value = value;
            this.Coordinates = value.Coordinates;
        }

        public override ReturnType Return => value.Return;

        public override object Evaluate() => value.ExecuteAndGiveValue().Evaluate();
    }

    class CallableAtom : Atom<Callable>
    {
        public CallableAtom(Callable value)
        {
            this.value = value;
            this.Coordinates = value.Coordinates;
        }

        public override ReturnType Return => value.Return;

        public override object Evaluate() => value.Evaluate();
    }

    class ObjectAtom : Atom<object>
    {
        public ObjectAtom(object value, (int, int) coordinates)
        {
            this.value = value;
            this.Coordinates = coordinates;
        }

        public override ReturnType Return => (value is int || value is double) ? ReturnType.Num :
                                              value is string ? ReturnType.String :
                                              value is GwentList ? ReturnType.List :
                                              value is Card ? ReturnType.Card : ReturnType.Object;

        public override object Evaluate() => value;
    }
}