using System;
using System.Collections.Generic;
using System.Text;

namespace Gwent_Interpreter
{
    interface IExpression
    {
        bool CheckSemantic();
        object Evaluate();
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

    abstract class Expr<T> : IVisitable<T>, IExpression
    {
        public virtual T Accept(IVisitor<T> visitor) => visitor.Visit(this);

        public abstract bool CheckSemantic();

        public abstract object Evaluate();
    }
}
