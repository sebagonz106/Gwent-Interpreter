using System;
using System.Collections.Generic;
using System.Text;
using Gwent_Interpreter.Expressions;

namespace Gwent_Interpreter.Statements
{
    class For : IStatement
    {
        Token item;
        IExpression collection;
        Environment environment;
        IStatement body;

        public For(Token item, IExpression collection, Environment environment, IStatement body)
        {
            this.item = item;
            this.collection = collection;
            this.environment = environment;
            this.body = body;
        }

        public void Execute()
        {
            IEnumerator<object> collection;
            try
            {
                collection = ((IEnumerable<object>)this.collection.Evaluate()).GetEnumerator();
            }
            catch (InvalidCastException)
            {
                throw new EvaluationError($"The given expression in the for declaration at {item.Coordinates.Item1}:{item.Coordinates.Item2 + item.Value.Length + 2} must be a collection");
            }

            if (environment.Contains(item.Value)) throw new EvaluationError($"Invalid for statement declaration (identifier already in use) at {item.Coordinates.Item1}:{item.Coordinates.Item2}");

            while (collection.MoveNext())
            {
                environment.Set(item, new Atom(collection.Current));
                body.Execute();
            }
        }
    }
}
