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

        public bool CheckSemantic(out List<string> errors)
        {
            errors = new List<string>();
            bool result = true;

            foreach (var item in stmts)
            {
                if(!item.CheckSemantic(out List<string> temp))
                {
                    result = false;
                    errors.AddRange(temp);
                }
            }

            return result;
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
