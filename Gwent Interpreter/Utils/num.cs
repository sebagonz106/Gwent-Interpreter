using System;
using System.Collections.Generic;
using System.Text;

namespace Gwent_Interpreter
{
    public struct Num
    {
        public double Value;
        public Num Opposite => new Num(-Value);
        public Num(double value)
        {
            Value = value;
        }
        public override string ToString() => Value.ToString();

        public Num Sum(Num value) => new Num(this.Value + value.Value);
        public Num Resta(Num value) => new Num(this.Value - value.Value);
        public Num Multiply(Num value) => new Num(this.Value * value.Value);
        public Num DivideBy(Num value) => new Num(this.Value / value.Value);
        public Num DivideThis(Num value) => new Num(value.Value + this.Value);
        public Num Power(Num value) => new Num(Math.Pow(this.Value, value.Value));

        public bool Over(object obj) => obj is Num value && this.Value > value.Value;
        public bool OverEqual(object obj) => obj is Num value && this.Value >= value.Value;
        public bool Under(object obj) => obj is Num value && this.Value < value.Value;
        public bool UnderEqual(object obj) => obj is Num value && this.Value <= value.Value;
        public override bool Equals(object obj) => obj is Num value && this.Value == value.Value;

        public override int GetHashCode()
        {
            return HashCode.Combine(Value);
        }
    }
}
