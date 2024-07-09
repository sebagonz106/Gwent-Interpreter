using System;
using System.Collections.Generic;
using System.Text;

namespace Gwent_Interpreter.Expressions
{
    class Predicate : Expr<object>
    {
        Token variable;
        IExpression condition;
        Environment environment;

        public Predicate(Token variable, IExpression condition, Environment environment)
        {
            this.variable = variable;
            this.condition = condition;
            this.environment = environment;
        }

        public override bool CheckSemantic()
        {
            throw new NotImplementedException();
        }

        public override object Evaluate() => new Predicate<Card>(Evaluate);

        bool Evaluate(Card card)
        {
            environment.Set(variable, new Atom(card));
            return (bool)condition.Evaluate();
        }
    }
}
