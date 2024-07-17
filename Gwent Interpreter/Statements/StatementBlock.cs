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
            string warning = "";

            foreach (var item in stmts)
            {
                try
                {
                    if (!item.CheckSemantic(out List<string> temp)) errors.AddRange(temp);
                }
                catch(Warning warn)
                {
                    warning += warn.Message + "\n";
                }
            }

            if (warning != "") throw new Warning(warning);
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
