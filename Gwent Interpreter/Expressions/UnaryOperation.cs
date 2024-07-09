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

        public override bool CheckSemantic(out string error)
        {
            error = "";
            if (value.Return is ReturnType.Num || value.Return is ReturnType.Bool) return true;

            error = ($"Invalid operation at {_operator.Coordinates.Item1}:{_operator.Coordinates.Item2}");
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
