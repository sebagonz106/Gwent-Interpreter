using System;
using System.Collections.Generic;
using System.Text;
using Gwent_Interpreter.GameLogic;

namespace Gwent_Interpreter.Expressions
{
    class Indexer : Expr<object>
    {
        IExpression indexer;
        IExpression index;
        (int, int) coordinates;

        public Indexer(IExpression indexer, (int, int) coordinates, IExpression index)
        {
            this.indexer = indexer;
            this.index = index;
            this.coordinates = coordinates;
        }

        public override ReturnType Return => ReturnType.Card;

        public override bool CheckSemantic(out List<string> errors)
        {
            errors = new List<string>();

            if (index.Return is ReturnType.Object) throw new Warning($"You must make sure object at {coordinates.Item1}:{coordinates.Item2 - 1} is a number or a compile time error may occur");
            else if (!(index.Return is ReturnType.Num)) errors.Add($"Invalid indexing operation at {coordinates.Item1}:{coordinates.Item2} (index is not a number)");

            if (indexer.Return is ReturnType.Object) throw new Warning($"You must make sure object at {coordinates.Item1}:{coordinates.Item2 - 1} is a list or a compile time error may occur");
            else if (!(indexer.Return is ReturnType.List)) errors.Add($"Invalid indexing operation at {coordinates.Item1}:{coordinates.Item2} (object is not a indexable)");
            
            return errors.Count==0;
        }

        public override bool CheckSemantic(out string error)
        {
            error = "";

            if (index.Return is ReturnType.Object) throw new Warning($"You must make sure object at {coordinates.Item1}:{coordinates.Item2 - 1} is a number or a compile time error may occur");
            else if (!(index.Return is ReturnType.Num)) error = ($"Invalid indexing operation at {coordinates.Item1}:{coordinates.Item2} (index is not a number)\n");

            if (indexer.Return is ReturnType.Object) throw new Warning($"You must make sure object at {coordinates.Item1}:{coordinates.Item2 - 1} is a list or a compile time error may occur");
            else if (!(indexer.Return is ReturnType.List)) error += ($"Invalid indexing operation at {coordinates.Item1}:{coordinates.Item2} (object is not a indexable)");

            return error.Length == 0;
        }

        public override object Evaluate() => ((GwentList)indexer.Evaluate())[(Num)index.Evaluate()];

        public override (int, int) Coordinates { get => coordinates; protected set => coordinates = value; }
    }
}
