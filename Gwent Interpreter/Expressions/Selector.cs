using System;
using System.Collections.Generic;
using System.Text;
using Gwent_Interpreter.GameLogic;

namespace Gwent_Interpreter.Expressions
{
    class Selector : IExpression
    {
        Selector parent;
        IExpression source;
        IExpression single;
        IExpression predicate;

        public Selector(IExpression source, IExpression single, IExpression predicate, Selector parent = null)
        {
            this.parent = parent;
            this.source = source;
            this.single = single;
            this.predicate = predicate;
        }

        public ReturnType Return => ReturnType.List;

        public bool CheckSemantic(out string error)
        {
            error = "";
            if (source.Return != ReturnType.String) error = "invalid sourrce";
            else if (source.CheckSemantic(out string temp)) error = temp;
            else if ((string)source.Evaluate() == "parent" && parent is null) error = "no existing parent";
            else if (single.Return != ReturnType.Bool) error = "invalid single";
            else if (single.CheckSemantic(out temp)) error = temp;
            else if (predicate.Return != ReturnType.Predicate) error = "invalid predicate";
            else if (predicate.CheckSemantic(out temp)) error = temp;
            else return true;

            return false;
        }

        public object Evaluate()
        {
            GwentList list;
            switch ((string)source.Evaluate())
            {
                case "board":
                    list = GwentInterpreterContext.Context.Board;
                    break;
                case "deck":
                    list = GwentInterpreterContext.Context.Deck;
                    break;
                case "otherDeck":
                    list = GwentInterpreterContext.Context.OtherDeck;
                    break;
                case "hand":
                    list = GwentInterpreterContext.Context.Hand;
                    break;
                case "otherHand":
                    list = GwentInterpreterContext.Context.OtherHand;
                    break;
                case "field":
                    list = GwentInterpreterContext.Context.Field;
                    break;
                case "otherField":
                    list = GwentInterpreterContext.Context.OtherField;
                    break;
                case "parent":
                    list = (GwentList)parent.Evaluate();
                    break;
                default:
                    throw new EvaluationError("invalid source");
            }

            list = list.Find((Predicate<Card>)predicate.Evaluate());
            return (bool)single.Evaluate()? new GwentList(new List<Card>() { list[0] }, list[0].Owner) : list;
        }
    }
}
