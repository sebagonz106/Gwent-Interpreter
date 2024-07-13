using System;
using System.Collections.Generic;
using System.Text;

namespace Gwent_Interpreter.Statements
{
    class StatementBlock : IStatement
    {
        IEnumerable<IStatement> stmts;

        public StatementBlock(IEnumerable<IStatement> stmts)
        {
            this.stmts = stmts;
        }

        public (int, int) Coordinates => throw new NotImplementedException();

        public bool CheckSemantic(out List<string> errors)
        {
            errors = new List<string>();

            foreach (var item in stmts)
            {
                if(!item.CheckSemantic(out List<string> temp)) errors.AddRange(temp);
            }

            return errors.Count == 0;
        }

        public void Execute()
        {
            foreach (var item in stmts)
            {
                item.Execute();
            }
        }
    }
}
