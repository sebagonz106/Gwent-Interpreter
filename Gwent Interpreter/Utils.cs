using System;
using System.Collections.Generic;
using System.Text;

namespace Gwent_Interpreter
{
    static class Utils
    {
        public static Dictionary<string, IExpression> usedVariables = new Dictionary<string, IExpression>();
    }
}
