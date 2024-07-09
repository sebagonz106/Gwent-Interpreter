﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Gwent_Interpreter.Expressions
{
    abstract class BinaryOperation<T> : Expr<T>
    {
        protected IExpression leftValue;
        protected Token _operator;
        protected IExpression rightValue;

        public BinaryOperation(Token _operator, IExpression leftValue, IExpression rightValue)
        {
            this.leftValue = leftValue;
            this.rightValue = rightValue;
            this._operator = _operator;
        }
        public override string ToString() => leftValue.ToString() + " " + _operator + " " + rightValue.ToString();
    }

    class ArithmeticOperation : BinaryOperation<Num>
    {
        static List<string> possibleOperations = new List<string> { "+", "-", "*", "/", "^" };

        public ArithmeticOperation(Token _operator, IExpression leftValue, IExpression rightValue)
                            : base(_operator, leftValue, rightValue) { }

        public override ReturnType Return => ReturnType.Num;

        public override bool CheckSemantic(out string error)
        {
            error = "";

            if (!possibleOperations.Contains(_operator.Value)) error = $"Invalid operation at {_operator.Coordinates.Item1}:{_operator.Coordinates.Item2}";
            else if (!(leftValue.Return is ReturnType.Num)) error = $"Invalid operation at {_operator.Coordinates.Item1}:{_operator.Coordinates.Item2} (left value is not a number)";
            else if (!(rightValue.Return is ReturnType.Num)) error = $"Invalid operation at {_operator.Coordinates.Item1}:{_operator.Coordinates.Item2} (right value is not a number)";
            else return true;

            return false;
        }

        public override Num Accept(IVisitor<Num> visitor) => base.Accept(visitor);

        public override object Evaluate()
        {
            try
            {
                switch (_operator.Value)
                {
                    case "+":
                        return ((Num)leftValue.Evaluate()).Sum((Num)rightValue.Evaluate());
                    case "-":
                        return ((Num)leftValue.Evaluate()).Resta((Num)rightValue.Evaluate());
                    case "*":
                        return ((Num)leftValue.Evaluate()).Multiply((Num)rightValue.Evaluate());
                    case "/":
                        return ((Num)leftValue.Evaluate()).DivideBy((Num)rightValue.Evaluate());
                    case "^":
                        return ((Num)leftValue.Evaluate()).Power((Num)rightValue.Evaluate());
                    default:
                        return null;
                }
            }
            catch (Exception)
            {
                throw new EvaluationError($"Invalid operation at {_operator.Coordinates.Item1}:{_operator.Coordinates.Item2}");
            }
        }
    }

    class BooleanOperation : BinaryOperation<bool>
    {
        static List<string> possibleOperations = new List<string> { "|", "||", "&", "&&" };
        public BooleanOperation(Token _operator, IExpression leftValue, IExpression rightValue)
                            : base(_operator, leftValue, rightValue) { }

        public override ReturnType Return => ReturnType.Bool;

        public override bool Accept(IVisitor<bool> visitor) => base.Accept(visitor);

        public override bool CheckSemantic(out string error)
        {
            error = "";

            if (!possibleOperations.Contains(_operator.Value)) error = $"Invalid operation at {_operator.Coordinates.Item1}:{_operator.Coordinates.Item2}";
            else if (!(leftValue.Return is ReturnType.Bool)) error = $"Invalid operation at {_operator.Coordinates.Item1}:{_operator.Coordinates.Item2} (left value is not a boolean)";
            else if (!(rightValue.Return is ReturnType.Bool)) error = $"Invalid operation at {_operator.Coordinates.Item1}:{_operator.Coordinates.Item2} (right value is not a boolean)";
            else return true;

            return false;
        }

        public override object Evaluate()
        {
            try
            {
                switch (_operator.Value)
                {
                    case "|":
                        return (bool)leftValue.Evaluate() | (bool)rightValue.Evaluate();
                    case "||":
                        return (bool)leftValue.Evaluate() || (bool)rightValue.Evaluate();
                    case "&":
                        return (bool)leftValue.Evaluate() & (bool)rightValue.Evaluate();
                    case "&&":
                        return (bool)leftValue.Evaluate() && (bool)rightValue.Evaluate();
                    default:
                        return null;
                }
            }
            catch (Exception)
            {
                throw new EvaluationError($"Invalid operation at {_operator.Coordinates.Item1}:{_operator.Coordinates.Item2}");
            }
        }
    }

    class StringOperation : BinaryOperation<string>
    {
        static List<string> possibleOperations = new List<string> { "@", "@@"};
        public StringOperation(Token _operator, IExpression leftValue, IExpression rightValue)
                            : base(_operator, leftValue, rightValue) { }

        public override string Accept(IVisitor<string> visitor) => base.Accept(visitor);

        public override ReturnType Return => ReturnType.String;

        public override bool CheckSemantic(out string error)
        {
            error = "";

            if (!possibleOperations.Contains(_operator.Value)) error = $"Invalid operation at {_operator.Coordinates.Item1}:{_operator.Coordinates.Item2}";
            else if (!(leftValue.Return is ReturnType.String)) error = $"Invalid operation at {_operator.Coordinates.Item1}:{_operator.Coordinates.Item2} (left value is not a string)";
            else if (!(rightValue.Return is ReturnType.String)) error = $"Invalid operation at {_operator.Coordinates.Item1}:{_operator.Coordinates.Item2} (right value is not a string)";
            else return true;

            return false;
        }

        public override object Evaluate()
        {
            try
            {
                switch (_operator.Value)
                {
                    case "@":
                        return (string)leftValue.Evaluate() + (string)rightValue.Evaluate();
                    case "@@":
                        return (string)leftValue.Evaluate() + " " + (string)rightValue.Evaluate();
                    default:
                        return null;
                }
            }
            catch (Exception)
            {
                throw new EvaluationError($"Invalid operation at {_operator.Coordinates.Item1}:{_operator.Coordinates.Item2}");
            }
        }
    }

    class ComparingOperation : BinaryOperation<bool>
    {
        static List<string> possibleOperations = new List<string> { ">", ">=", "<", "<=", "==", "!=" };

        public ComparingOperation(Token _operator, IExpression leftValue, IExpression rightValue)
                            : base(_operator, leftValue, rightValue) { }

        public override bool Accept(IVisitor<bool> visitor) => base.Accept(visitor);

        public override ReturnType Return => ReturnType.Bool;

        public override bool CheckSemantic(out string error)
        {
            error = "";

            if (!possibleOperations.Contains(_operator.Value)) error = $"Invalid operation at {_operator.Coordinates.Item1}:{_operator.Coordinates.Item2}";
            else if (!(leftValue.Return is ReturnType.Num)) error = $"Invalid operation at {_operator.Coordinates.Item1}:{_operator.Coordinates.Item2} (left value is not a number)";
            else if (!(rightValue.Return is ReturnType.Num)) error = $"Invalid operation at {_operator.Coordinates.Item1}:{_operator.Coordinates.Item2} (right value is not a number)";
            else return true;

            return false;
        }

        public override object Evaluate()
        {
            try
            {
                switch (_operator.Value)
                {
                    case ">":
                        return ((Num)leftValue.Evaluate()).Over(rightValue.Evaluate());
                    case ">=":
                        return ((Num)leftValue.Evaluate()).OverEqual(rightValue.Evaluate());
                    case "<":
                        return ((Num)leftValue.Evaluate()).Under(rightValue.Evaluate());
                    case "<=":
                        return ((Num)leftValue.Evaluate()).UnderEqual(rightValue.Evaluate());
                    case "==":
                        return leftValue.Evaluate().Equals(rightValue.Evaluate());
                    case "!=":
                        return !leftValue.Evaluate().Equals(rightValue.Evaluate());
                    default:
                        return null;
                }
            }
            catch (Exception)
            {
                throw new EvaluationError($"Invalid operation at {_operator.Coordinates.Item1}:{_operator.Coordinates.Item2}");
            }
        }
    }
}