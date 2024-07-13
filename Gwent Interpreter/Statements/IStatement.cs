using System;
using System.Collections.Generic;
using System.Text;
using Gwent_Interpreter.GameLogic;

namespace Gwent_Interpreter
{
    interface IStatement
    {
        (int, int) Coordinates { get; }
        void Execute();
        bool CheckSemantic(out List<string> errors);
    }
}
