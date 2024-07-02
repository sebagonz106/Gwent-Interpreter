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

        public void Execute()
        {
            foreach (var item in stmts)
            {
                item.Execute();
            }
        }
    }
}
