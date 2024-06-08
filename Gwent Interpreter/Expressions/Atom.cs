using System;
using System.Collections.Generic;
using System.Text;

namespace Gwent_Interpreter.Expressions
{
    public class Atom<T> : IExpression<T>
    {
        string strValue;
        T value;

        public Atom(string strValue, T value)
        {
            this.strValue = strValue;
            this.value = value;
        }

        public bool CheckSemantic()
        {
            return value.ToString()==strValue;
        }

        public T Evaluate() => value;

        public override string ToString() => value.ToString();
    }
}
