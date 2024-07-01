using System;
using System.Collections.Generic;
using System.Text;

namespace Gwent_Interpreter.Statements
{
    class While : IStatement
    {
        IExpression conditional;
        IStatement body;

        public While(IExpression conditional, IStatement body)
        {
            this.conditional = conditional;
            this.body = body;
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
    }
}
