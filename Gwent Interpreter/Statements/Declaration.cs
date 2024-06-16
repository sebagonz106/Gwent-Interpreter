using System;
using System.Collections.Generic;
using System.Text;
using Gwent_Interpreter.Expressions;

namespace Gwent_Interpreter.Statements
{
    class Declaration : IStatement
    {
        Token variable;
        Token operation;
        IExpression value;

        public Declaration(Token variable, Token operation = null, IExpression value = null)
        {
            this.variable = variable;
            this.operation = operation;
            this.value = value;
            this.Execute();
        }

        public void Execute()
        {
            try
            {
                switch (operation.Type)
                {
                    case TokenType.Asign:
                        Utils.usedVariables[variable.Value] = value;
                        break;
                    case TokenType.Increase:
                        Utils.usedVariables[variable.Value] = new ArithmeticOperation(new Token("+", TokenType.Plus, operation.Coordinates.Item1, operation.Coordinates.Item2), Utils.usedVariables[variable.Value], value);
                        break;
                    case TokenType.Decrease:
                        Utils.usedVariables[variable.Value] = new ArithmeticOperation(new Token("-", TokenType.Minus, operation.Coordinates.Item1, operation.Coordinates.Item2), Utils.usedVariables[variable.Value], value);
                        break;
                    case TokenType.IncreaseOne:
                        Utils.usedVariables[variable.Value] = new ArithmeticOperation(new Token("+", operation), Utils.usedVariables[variable.Value], new Atom(new Token("1", TokenType.Number, operation.Coordinates.Item1, operation.Coordinates.Item2)));
                        break;
                    case TokenType.DecreaseOne:
                        Utils.usedVariables[variable.Value] = new ArithmeticOperation(new Token("-", operation), Utils.usedVariables[variable.Value], new Atom(new Token("1", TokenType.Number, operation.Coordinates.Item1, operation.Coordinates.Item2)));
                        break;
                    default:
                        throw new EvaluationError($"Invalid declaration at {variable.Coordinates.Item1}:{variable.Coordinates.Item2}");
                }
            }
            catch (NullReferenceException)
            {
                try
                {
                    Utils.usedVariables[variable.Value] = null;
                    throw new ParsingError($"Already declared variable at {variable.Coordinates.Item1}:{variable.Coordinates.Item2 + variable.Value.Length}");
                }
                catch (NullReferenceException)
                {
                    Utils.usedVariables.Add(variable.Value, null);
                }
            }
            catch (InvalidCastException)
            {
                throw new ParsingError($"Invalid declaration at {variable.Coordinates.Item1}:{variable.Coordinates.Item2}");
            }
        }
    }
}
