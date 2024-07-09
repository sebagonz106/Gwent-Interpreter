﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Gwent_Interpreter.Expressions
{
    class Predicate : Expr<object>
    {
        Token variable;
        IExpression condition;
        Environment environment;

        public override ReturnType Return => ReturnType.Predicate;

        public Predicate(Token variable, IExpression condition, Environment environment)
        {
            this.variable = variable;
            this.condition = condition;
            this.environment = environment;
        }

        public override bool CheckSemantic(out string error)
        {
            error = "";

            if (!(condition.Return is ReturnType.Bool)) error = $"The right member of the predicate at {variable.Coordinates.Item1}:{variable.Coordinates.Item2} must be a boolean operation";
            else return true;

            return false;
        }

        public override object Evaluate() => new Predicate<Card>(Evaluate);

        bool Evaluate(Card card)
        {
            environment.Set(variable, new Atom(card));
            return (bool)condition.Evaluate();
        }
    }
}