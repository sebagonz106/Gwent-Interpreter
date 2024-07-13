using System;
using System.Collections.Generic;
using System.Text;

namespace Gwent_Interpreter.Statements
{
    class Log : IStatement
    {
        public IExpression Value { get; private set; }

        public (int, int) Coordinates => throw new NotImplementedException();

        public Log(IExpression value)
        {
            Value = value;
        }
        public void Execute()
        {
            Console.WriteLine(Value.Evaluate());
        }

        public bool CheckSemantic(out List<string> errors)
        {
            errors = new List<string>();

            if (!Value.CheckSemantic(out string error)) errors.Add(error);
            else return true;

            return false;
        }
    }
}
