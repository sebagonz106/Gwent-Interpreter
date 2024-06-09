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

    interface IVisitable<T>
    {
        T Accept(IVisitor<T> visitor);
    }

    interface IVisitor<T>
    {
        T Visit(IVisitable<T> visitable);
    }

    abstract class Expr<T> : IVisitable<T>, IExpression<T>
    {
        public virtual T Accept(IVisitor<T> visitor) => visitor.Visit(this);

        public abstract bool CheckSemantic();

        public abstract T Evaluate();
    }
}
