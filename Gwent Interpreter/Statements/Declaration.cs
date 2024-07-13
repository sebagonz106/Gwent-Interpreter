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
            this.Execute(); //TODO: check if really needed in Parser
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
                        environment.Set(variable, new ArithmeticOperation(new Token("+", operation), environment[variable.Value], new ValueAtom(new Token("1", TokenType.Number, operation.Coordinates.Item1, operation.Coordinates.Item2))));
                        break;
                    case TokenType.DecreaseOne:
                        environment.Set(variable, new ArithmeticOperation(new Token("-", operation), environment[variable.Value], new ValueAtom(new Token("1", TokenType.Number, operation.Coordinates.Item1, operation.Coordinates.Item2))));
                        break;
                    default:
                        throw new ParsingError($"Invalid declaration at {variable.Coordinates.Item1}:{variable.Coordinates.Item2}");
                }
            }
            catch (NullReferenceException)
            {
                //if there is no operation defined, then nothing will execute and this will only allow to access the variable value
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

        public ReturnType Return => value is null? environment[variable.Value].Return : value.Return;

        public (int, int) Coordinates => operation is null? variable.Coordinates : operation.Coordinates;

        public bool CheckSemantic(out List<string> errors)
        {
            errors = new List<string>();

            if(!(value is null) && !value.CheckSemantic(out string error))
            {
                errors.Add(error);
                return false;
            }
            else if (!(environment[variable.Value] is null) && !value.CheckSemantic(out error))
            {
                errors.Add(error);
                return false;
            }

            return true;
        }
    }
}
