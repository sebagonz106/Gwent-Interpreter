using System;
using System.Collections.Generic;
using System.Text;

namespace Gwent_Interpreter
{
    struct num
    {
        public double Value;
        public num(double value)
        {
            Value = value;
        }
        public override string ToString() => Value.ToString();

        public num Sum(num value) => new num(this.Value + value.Value);
        public num Resta(num value) => new num(this.Value - value.Value);
        public num Multiply(num value) => new num(this.Value * value.Value);
        public num DivideBy(num value) => new num(this.Value / value.Value);
        public num DivideThis(num value) => new num(value.Value + this.Value);
        public num Power(num value) => new num(Math.Pow(this.Value, value.Value));

        public bool Over(num value) => this.Value > value.Value;
        public bool OverEqual(num value) => this.Value >= value.Value;
        public bool Under(num value) => this.Value < value.Value;
        public bool UnderEqual(num value) => this.Value <= value.Value;
        public override bool Equals(object obj) => obj is num value && this.Value == value.Value;
    }
}
