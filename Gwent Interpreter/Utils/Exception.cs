using System;
using System.Collections.Generic;
using System.Text;

namespace Gwent_Interpreter
{
    class ParsingError : Exception
    {
        public override string Message { get; }
        public ParsingError(string message)
        {
            Message = message;
        }
    }
    class EvaluationError : Exception
    {
        public override string Message { get; }
        public EvaluationError(string message)
        {
            Message = message;
        }
    }
    class Warning : Exception
    {
        public override string Message { get; }
        public Warning(string message)
        {
            Message = message;
        }
    }
}
