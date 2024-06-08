using System;
using System.Collections.Generic;
using System.Text;

namespace Gwent_Interpreter
{
    interface IExpression<out T>
    {
        bool CheckSemantic();
        T Evaluate();
        string ToString();
    }
}
