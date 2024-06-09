using System;
using System.Collections.Generic;
using System.Text;

namespace Gwent_Interpreter.Expressions
{
    abstract class BinaryOperation<inT, outT> : Expr<outT>
    {
        protected Expr<inT> leftValue;
        protected string _operator;
        protected Expr<inT> rightValue;

        public BinaryOperation (Expr<inT> leftValue, Expr<inT> rightValue, string _operator)
        {
            this.leftValue = leftValue;
            this.rightValue = rightValue;
            this._operator = _operator;
        }
        public override string ToString() => leftValue.ToString() + " " + _operator + " " + rightValue.ToString();
    }

    class ArithmeticOperation : BinaryOperation<num,num>
    {
        static List<string> possibleOperations = new List<string> { "+", "-", "*", "/", "^" };

        public ArithmeticOperation(Expr<num> leftValue, Expr<num> rightValue, string _operator)
                            : base(leftValue, rightValue, _operator) { }

        public override bool CheckSemantic() => possibleOperations.Contains(this._operator) && this.leftValue.CheckSemantic() && this.rightValue.CheckSemantic();

        public override num Accept(IVisitor<num> visitor) => base.Accept(visitor);

        public override num Evaluate()
        {
            switch (_operator)
            {
                case "+":
                    return leftValue.Evaluate().Sum(rightValue.Evaluate());
                case "-":
                    return leftValue.Evaluate().Resta(rightValue.Evaluate());
                case "*":
                    return leftValue.Evaluate().Multiply(rightValue.Evaluate());
                case "/":
                    return leftValue.Evaluate().DivideBy(rightValue.Evaluate());
                case "^":
                    return leftValue.Evaluate().Power(rightValue.Evaluate());
                default:
                    return leftValue.Evaluate();
            }
        }
    }

    class BooleanOperation : BinaryOperation<bool,bool>
    {
        static List<string> possibleOperations = new List<string> { "|", "||", "&", "&&" };
        public BooleanOperation(Expr<bool> leftValue, Expr<bool> rightValue, string _operator)
                         : base(leftValue, rightValue, _operator) { }

        public override bool Accept(IVisitor<bool> visitor) => base.Accept(visitor);

        public override bool CheckSemantic() => possibleOperations.Contains(this._operator) && this.leftValue.CheckSemantic() && this.rightValue.CheckSemantic();

        public override bool Evaluate()
        {
            switch (_operator)
            {
                case "|":
                    return leftValue.Evaluate() | rightValue.Evaluate();
                case "||":
                    return leftValue.Evaluate() || rightValue.Evaluate();
                case "&":
                    return leftValue.Evaluate() & rightValue.Evaluate();
                case "&&":
                    return leftValue.Evaluate() && rightValue.Evaluate();
                default:
                    return leftValue.Evaluate();
            }
        }
    }

    class StringOperation : BinaryOperation<string,string>
    {
        static List<string> possibleOperations = new List<string> { "@", "@@"};
        public StringOperation(Expr<string> leftValue, Expr<string> rightValue, string _operator)
                         : base(leftValue, rightValue, _operator) { }

        public override string Accept(IVisitor<string> visitor) => base.Accept(visitor);

        public override bool CheckSemantic() => possibleOperations.Contains(this._operator) && this.leftValue.CheckSemantic() && this.rightValue.CheckSemantic();

        public override string Evaluate()
        {
            switch (_operator)
            {
                case "@":
                    return leftValue.Evaluate() + rightValue.Evaluate();
                case "@@":
                    return leftValue.Evaluate() + " " + rightValue.Evaluate();
                default:
                    return leftValue.Evaluate();
            }
        }
    }

    class ComparingOperation : BinaryOperation<num, bool>
    {
        static List<string> possibleOperations = new List<string> { ">", ">=", "<", "<=", "==", "!=" };

        public ComparingOperation(Expr<num> leftValue, Expr<num> rightValue, string _operator)
                           : base(leftValue, rightValue, _operator) { }

        public override bool Accept(IVisitor<bool> visitor) => base.Accept(visitor);

        public override bool CheckSemantic() => possibleOperations.Contains(this._operator) && this.leftValue.CheckSemantic() && this.rightValue.CheckSemantic();


        public override bool Evaluate()
        {
            switch (_operator)
            {
                case ">":
                    return leftValue.Evaluate().Over(rightValue.Evaluate());
                case ">=":
                    return leftValue.Evaluate().OverEqual(rightValue.Evaluate());
                case "<":
                    return leftValue.Evaluate().Under(rightValue.Evaluate());
                case "<=":
                    return leftValue.Evaluate().UnderEqual(rightValue.Evaluate());
                case "==":
                    return leftValue.Evaluate().Equals(rightValue.Evaluate());
                case "!=":
                    return !leftValue.Evaluate().Equals(rightValue.Evaluate());
                default:
                    return false;
            }
        }
    }
}