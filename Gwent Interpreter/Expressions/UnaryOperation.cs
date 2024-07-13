using System;
using System.Collections.Generic;
using System.Text;

namespace Gwent_Interpreter.Expressions
{
    class UnaryOperation : Expr<object>
    {
        protected Token _operator;
        protected IExpression value;

        public UnaryOperation (Token _operator, IExpression value)
        {
            this.value = value;
            this._operator = _operator;
        }

        public override ReturnType Return => value.Return;

        public override (int, int) Coordinates { get => _operator.Coordinates; protected set => throw new NotImplementedException(); }

        public override bool CheckSemantic(out List<string> errors)
        {
            errors = new List<string>();

            if (value.Return is ReturnType.Object)
                throw new Warning($"You must make sure object at {_operator.Coordinates.Item1}:{_operator.Coordinates.Item2} is a boolean or number or a compile time error may occur");

            if (value.Return is ReturnType.Num || value.Return is ReturnType.Bool) return true;

            errors.Add($"Invalid operation at {_operator.Coordinates.Item1}:{_operator.Coordinates.Item2} (number or boolean expected)");
            return false;
        }

        public override bool CheckSemantic(out string error)
        {
            error = "";

            if (value.Return is ReturnType.Object)
                throw new Warning($"You must make sure object at {_operator.Coordinates.Item1}:{_operator.Coordinates.Item2} is a boolean or number or a compile time error may occur");

            if (value.Return is ReturnType.Num || value.Return is ReturnType.Bool) return true;

            error = $"Invalid operation at {_operator.Coordinates.Item1}:{_operator.Coordinates.Item2} (number or boolean expected)";
            return false;
        }

        public override object Evaluate()
        {
            switch (_operator.Value)
            {
                case "-":
                    return new Num(-((Num)value.Evaluate()).Value);
                case "!":
                    return !(bool)value.Evaluate();
                default:
                    throw new EvaluationError($"Invalid operation at {_operator.Coordinates.Item1}:{_operator.Coordinates.Item2}"); ;
            }
        }

        public override string ToString() => _operator + value.ToString();
    }
}
