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
        Environment environment;

        public Declaration(Token variable, Environment environment = null, Token operation = null, IExpression value = null)
        {
            this.variable = variable;
            this.operation = operation;
            this.value = value;
            this.environment = environment ?? Environment.Global;
            this.Execute();
        }

        public void Execute()
        {
            try
            {
                switch (operation.Type)
                {
                    case TokenType.Assign:
                        environment.Set(variable, value);
                        break;
                    case TokenType.Increase:
                        environment.Set(variable, new ArithmeticOperation(new Token("+", TokenType.Plus, operation.Coordinates.Item1, operation.Coordinates.Item2), environment[variable.Value], value));
                        break;
                    case TokenType.Decrease:
                        environment.Set(variable, new ArithmeticOperation(new Token("-", TokenType.Minus, operation.Coordinates.Item1, operation.Coordinates.Item2), environment[variable.Value], value));
                        break;
                    case TokenType.IncreaseOne:
                        environment.Set(variable, new ArithmeticOperation(new Token("+", operation), environment[variable.Value], new Atom(new Token("1", TokenType.Number, operation.Coordinates.Item1, operation.Coordinates.Item2))));
                        break;
                    case TokenType.DecreaseOne:
                        environment.Set(variable, new ArithmeticOperation(new Token("-", operation), environment[variable.Value], new Atom(new Token("1", TokenType.Number, operation.Coordinates.Item1, operation.Coordinates.Item2))));
                        break;
                    default:
                        throw new ParsingError($"Invalid declaration at {variable.Coordinates.Item1}:{variable.Coordinates.Item2}");
                }
            }
            catch (NullReferenceException)
            {

            }
        }
        public IExpression ExecuteAndGiveValue()
        {
            Execute();
            return environment[variable.Value];
        }

        public override string ToString()
        {
            return variable.Value + operation?? "" + value?? "";
        }
    }
}
