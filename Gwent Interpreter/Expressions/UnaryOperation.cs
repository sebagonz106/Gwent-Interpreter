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

        public override bool CheckSemantic()
        {
            throw new NotImplementedException();
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
                    return false;
            }
        }

        public override string ToString() => _operator + value.ToString();
    }
}
