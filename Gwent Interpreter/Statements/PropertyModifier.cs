using System;
using System.Collections.Generic;
using System.Text;
using Gwent_Interpreter.Expressions;
using Gwent_Interpreter.Utils;

namespace Gwent_Interpreter.Statements
{
    class PropertyModifier : IStatement
    {
        Property property;
        Token operation;
        IExpression value;

        static List<TokenType> validOperations = new List<TokenType> { TokenType.Assign, TokenType.Increase, TokenType.IncreaseOne, TokenType.Decrease, TokenType.DecreaseOne };

        public PropertyModifier(Property property, Token operation, IExpression value = null)
        {
            this.property = property;
            this.operation = operation;
            this.value = value;
        }

        public (int, int) Coordinates => operation.Coordinates;

        public bool CheckSemantic(out List<string> errors)
        {
            value.CheckSemantic(out errors);
            if(!validOperations.Contains(operation.Type)) errors.Add($"Invalid declaration at {operation.Coordinates.Item1}:{operation.Coordinates.Item2}");

            if (errors.Count > 0) return false;
            else return property.CheckSemantic(out string temp);
        }
        public void Execute()
        {
            object callee = property.callee.Evaluate();

            Type type;
            if (callee is GwentList) type = typeof(GwentList);
            else if (callee is Card) type = typeof(Card);
            else if (callee is Str) type = typeof(Str);
            else if (callee is Num) type = typeof(Num);
            else type = typeof(object);

            if (type.GetProperty(property.caller.Value) != null)
            {
                object value = this.value is null? null : this.value.Evaluate();

                switch (operation.Type)
                {
                    case TokenType.Assign:
                        type.GetProperty(property.caller.Value).SetValue(callee, value);
                        break;
                    case TokenType.Increase:
                        type.GetProperty(property.caller.Value).SetValue(callee, ((Num)callee).Sum((Num)value));
                        break;
                    case TokenType.Decrease:
                        type.GetProperty(property.caller.Value).SetValue(callee, ((Num)callee).Resta((Num)value));
                        break;
                    case TokenType.IncreaseOne:
                        type.GetProperty(property.caller.Value).SetValue(callee, ((Num)callee).Sum(new Num(1)));
                        break;
                    case TokenType.DecreaseOne:
                        type.GetProperty(property.caller.Value).SetValue(callee, ((Num)callee).Resta(new Num(1)));
                        break;
                    default:
                        throw new EvaluationError($"Invalid declaration at {Coordinates.Item1}:{Coordinates.Item2}");
                }
            }
            else throw new EvaluationError($"Property not found at {property.caller.Coordinates.Item1}:{property.caller.Coordinates.Item2}");
        }
    }
}
