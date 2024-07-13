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

        public (int, int) Coordinates => (item.Coordinates.Item1, item.Coordinates.Item2-3);

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
                environment.Set(item, new ObjectAtom(collection.Current, (-1,-1)));
                body.Execute();
            }
        }

        public bool CheckSemantic(out List<string> errors)
        {
            bool result = true;

            result = body.CheckSemantic(out errors) && result;

            if (!collection.CheckSemantic(out string error))
            {
                errors.Add(error);
                return false;
            }
            if (!(collection.Return is ReturnType.List)) throw new Warning($"You must make sure object at {item.Coordinates.Item1}:{item.Coordinates.Item2 + item.Value.Length + 4} is a list or a compile time error may occur");

            return result;
        }
    }
}