using System;
using System.Collections.Generic;
using System.Text;

namespace Gwent_Interpreter.Statements
{
    class While : IStatement
    {
        IExpression conditional;
        IStatement body;
        (int, int) coordinates;

        public (int, int) Coordinates => coordinates;

        public While(IExpression conditional, IStatement body, (int, int) coordinates)
        {
            this.conditional = conditional;
            this.body = body;
            this.coordinates = coordinates;
        }

        public void Execute()
        {
            try
            {
                while ((bool)conditional.Evaluate()) body.Execute();
            }
            catch (InvalidCastException)
            {
                throw new EvaluationError("Incapable of converting conditional expression to boolean value.");
            }
        }

        public bool CheckSemantic(out List<string> errors)
        {
            bool result = true;

            result = body.CheckSemantic(out errors);

            if (!(conditional.Return is ReturnType.Bool))
            {
                errors.Add($"Expression at {coordinates.Item1}:{coordinates.Item2 + 2} must be boolean");
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
