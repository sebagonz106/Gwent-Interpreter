﻿using System;
using System.Collections.Generic;
using System.Text;
using Gwent_Interpreter.GameLogic;

namespace Gwent_Interpreter.Expressions
{
    abstract class Callable : IExpression
    {
        protected IExpression callee;
        protected Token caller;

        public abstract object Evaluate();
        public abstract bool CheckSemantic();
    }

    class Property : Callable
    {
        public Property(Token caller, IExpression callee)
        {
            this.caller = caller;
            this.callee = callee;
        }
        public override bool CheckSemantic()
        {
            throw new NotImplementedException();
        }

        public override object Evaluate()
        {
            object callee = this.callee.Evaluate();

            Type type;
            if (callee is GwentList) type = typeof(GwentList);
            else if (callee is Card) type = typeof(Card);
            else if (callee is string) type = typeof(string);
            else if (callee is Num) type = typeof(Num);
            else type = typeof(object);

            if (type.GetProperty(caller.Value) != null)
            {
                return type.GetProperty(caller.Value).GetValue(callee);
            }
            else throw new EvaluationError($"Property not found at {caller.Coordinates.Item1}:{caller.Coordinates.Item2}");
        }
    }
    class Method : Callable
    {
        public IExpression[] arguments;

        public Method(Token caller, IExpression callee, IExpression[] arguments = null)
        {
            this.caller = caller;
            this.callee = callee;
            this.arguments = arguments;
        }

        public override bool CheckSemantic()
        {
            throw new NotImplementedException();
        }

        public override object Evaluate()
        {
            object callee = this.callee.Evaluate();

            Type type;
            if (callee is GwentList) type = typeof(GwentList);
            else if (callee is Card) type = typeof(Card);
            else if (callee is string) type = typeof(string);
            else if (callee is Num) type = typeof(Num);
            else type = typeof(object);

            if (type.GetMethod(caller.Value) != null)
            {
                try
                {
                    return type.GetMethod(caller.Value).Invoke(callee, this.arguments);
                }
                catch (Exception)
                {
                    throw new EvaluationError($"Invalid arguments given at {caller.Coordinates.Item1}:{caller.Coordinates.Item2}");
                }
            }
            else throw new EvaluationError($"Method not found at {caller.Coordinates.Item1}:{caller.Coordinates.Item2}");
        }
    }
}
