using System;
using System.Collections.Generic;
using System.Text;

namespace Gwent_Interpreter
{
    interface IExpression
    {
        (int,int) Coordinates { get; }
        bool CheckSemantic(out List<string> errors);
        bool CheckSemantic(out string error);
        object Evaluate();
        ReturnType Return { get; }
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

        public abstract bool CheckSemantic(out List<string> errors);
        public abstract bool CheckSemantic(out string error);

        public abstract object Evaluate();

        public abstract ReturnType Return { get; }

        public abstract (int, int) Coordinates { get; protected set; }
    }

    public enum ReturnType
    {
        Num,
        String,
        Bool,
        Card,
        List,
        Void,
        Predicate,
        Context,
        Object
    }
}
