using System;
using System.Collections.Generic;
using System.Text;

namespace Gwent_Interpreter.Utils
{
    interface ICapsule<T>
    {
        T Value { get; }
    }
}
