using System;
using System.Collections.Generic;
using System.Text;

namespace Gwent_Interpreter.Statements
{
    class Log : IStatement
    {
        public IExpression Value { get; private set; }
        public Log(IExpression value)
        {
            Value = value;
        }
        public void Execute()
        {
            Console.WriteLine(Value.Evaluate());
        }
    }
}
