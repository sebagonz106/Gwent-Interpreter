using System;
using System.Collections.Generic;
using System.Text;
using Gwent_Interpreter.GameLogic;

namespace Gwent_Interpreter.Expressions
{
    class Indexer : IExpression
    {
        IExpression indexer;
        IExpression index;

        public Indexer(IExpression indexer, IExpression index)
        {
            this.indexer = indexer;
            this.index = index;
        }

        public bool CheckSemantic() => indexer.Evaluate() is GwentList && index.Evaluate() is Num;

        public object Evaluate() => ((GwentList)indexer.Evaluate())[(Num)index.Evaluate()];
    }
}
