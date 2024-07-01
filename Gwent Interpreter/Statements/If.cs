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

        public If(IExpression conditional, IStatement body, IStatement elseBody = null)
        {
            this.conditional = conditional;
            this.body = body;
            this.elseBody = elseBody;
        }

        public void Execute()
        {
            if (conditional.Evaluate() is bool conditionalValue && conditionalValue) body.Execute();
            else if (!(elseBody is null)) elseBody.Execute();
        }
    }
}
