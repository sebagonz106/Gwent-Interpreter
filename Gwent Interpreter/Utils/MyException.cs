using System;
using System.Collections.Generic;
using System.Text;

namespace Gwent_Interpreter
{
    class MyException : Exception { }
    class ParsingError : MyException
    {
        public override string Message { get; }
        public ParsingError(string message)
        {
            Message = message;
        }
    }
    class EvaluationError : MyException
    {
        public override string Message { get; }
        public EvaluationError(string message)
        {
            Message = message;
        }
    }
    class Warning : MyException
    {
        public override string Message { get; }
        public Warning(string message)
        {
            Message = message;
        }
    }
}
