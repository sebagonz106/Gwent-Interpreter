using System;
using System.Collections.Generic;
using System.Text;

namespace Gwent_Interpreter.Utils
{
    class Str : ICapsule<string>
    {
        string value;
        public string Value => value;
        public Str(string value)
        {
            this.value = value;
        }

        public static Str Sum(Str left, Str right, bool space = false) => new Str(left.Value + (space ? " " : "") + right.Value);

        public override bool Equals(object obj)
        {
            return (obj is Str str && this.Value==str.Value)|| (obj is string _str && this.Value == _str);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(value);
        }

        public override string ToString() => Value;
        public int Length => value.Length;
    }
}
