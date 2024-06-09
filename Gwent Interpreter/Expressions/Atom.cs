using System;
using System.Collections.Generic;
using System.Text;

namespace Gwent_Interpreter.Expressions
{
    class Atom<T> : Expr<T>
    {
        string strValue;
        T value;

        public Atom(string strValue, T value)
        {
            this.strValue = strValue;
            this.value = value;
        }

        public override bool CheckSemantic()
        {
            return (value is bool? value.ToString().ToLower() : value.ToString())==strValue;
        }

        public override T Evaluate() => value;

        public override string ToString() => value.ToString();
    }
}
