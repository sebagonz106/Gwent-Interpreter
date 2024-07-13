using System;
using System.Collections.Generic;
using System.Text;

namespace Gwent_Interpreter.Statements
{
    class If : IStatement
    {
        IExpression conditional;
        IStatement body;
        IStatement elseBody;
        (int, int) coordinates;

        public (int, int) Coordinates => coordinates;

        public If(IExpression conditional, IStatement body, (int, int) coordinates, IStatement elseBody = null)
        {
            this.conditional = conditional;
            this.body = body;
            this.elseBody = elseBody;
            this.coordinates = coordinates;
        }

        public void Execute()
        {
            if (conditional.Evaluate() is bool conditionalValue && conditionalValue) body.Execute();
            else if (!(elseBody is null)) elseBody.Execute();
        }

        public bool CheckSemantic(out List<string> errors)
        {
            bool result = true;

            result = body.CheckSemantic(out errors);
            if (!(elseBody is null)) { result = elseBody.CheckSemantic(out List<string> tempErrors) && result; errors.AddRange(tempErrors); }

            if(!(conditional.Return is ReturnType.Bool))
            {
                errors.Add($"Expression at {coordinates.Item1}:{coordinates.Item2} must be boolean");
                return false;
            }
            else if (!conditional.CheckSemantic(out string error))
            {
                errors.Add(error);
                return false;
            }

            return result;
        }
    }
}
