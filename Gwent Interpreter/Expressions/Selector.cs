using System;
using System.Collections.Generic;
using System.Text;
using Gwent_Interpreter.GameLogic;
using Gwent_Interpreter.Utils;

namespace Gwent_Interpreter.Expressions
{
    class Selector : Expr<object>
    {
        (int, int) coordinates;
        IExpression parent;
        IExpression source;
        IExpression single;
        IExpression predicate;

        public Selector((int, int) coordinates, IExpression source, IExpression predicate, IExpression single = null, IExpression parent = null)
        {
            this.coordinates = coordinates;
            this.parent = parent;
            this.source = source;
            this.single = single;
            this.predicate = predicate;
        }

        public override ReturnType Return => ReturnType.List;

        public override bool CheckSemantic(out string error)
        {
            error = "";
            try
            {
                if (source.Return != ReturnType.String) error = "Invalid source return type" + position;
                else if (source.CheckSemantic(out string temp)) error = temp;
                else if ((string)source.Evaluate() == "parent" && parent is null) error = "No existing parent" + position;
                else if (predicate.Return != ReturnType.Predicate) error = "Invalid predicate return type" + position;
                else if (predicate.CheckSemantic(out temp)) error = temp;
                else if (single is null) { single = new ObjectAtom(false, coordinates); return true; } //if single is not received, it will be false by default
                else if (single.Return is ReturnType.Object) throw new Warning($"You must make sure single in selector at { coordinates.Item1}:{ coordinates.Item2 - 1} is boolean or a compile time error may occur");
                else if (single.Return != ReturnType.Bool) error = "Invalid single return type" + position;
                else if (single.CheckSemantic(out temp)) error = temp;
                else return true;
            }
            catch (InvalidCastException)
            {
                error = ("Invalid source return type" + position);
            }

            return false;
        }

        public override bool CheckSemantic(out List<string> errors)
        {
            errors = new List<string>();
            string warning = "";

            try
            {
                if (source.Return != ReturnType.String || source.Return != ReturnType.Object) errors.Add("Invalid source return type" + position);
                else if (!source.CheckSemantic(out List<string> temp)) errors.AddRange(temp);
                else if ((string)source.Evaluate() == "parent" && parent is null) errors.Add("No existing parent" + position); //no need to throw a warning about an object because it is being evaluated.
            }
            catch (InvalidCastException)
            {
                errors.Add("Invalid source return type" + position);
            }

            if (single is null) { single = new ObjectAtom(false, coordinates); return true; } //if single is not received, it will be false by default
            else if (single.Return is ReturnType.Object) warning = $"You must make sure single {position} is boolean or a compile time error may occur";
            else if (single.Return != ReturnType.Bool) errors.Add("Invalid single return type" + position);
            else if (!single.CheckSemantic(out List<string> temp)) errors.AddRange(temp);

            if (predicate.Return != ReturnType.Predicate) errors.Add("Invalid predicate return type" + position);
            else if (predicate.CheckSemantic(out List<string> temp)) errors.AddRange(temp);

            if (warning != "") throw new Warning(warning);
            return errors.Count == 0;
        }

        public override object Evaluate()
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
                    throw new EvaluationError("Invalid source" + position);
            }

            list = list.Find((Predicate<Card>)predicate.Evaluate());
            return (bool)single.Evaluate()? new GwentList(new List<Card>() { list[0] }, list[0].Owner) : list;
        }

        string position => $"in selector at { coordinates.Item1}:{ coordinates.Item2 - 1}";

        public override (int, int) Coordinates { get => coordinates; protected set => coordinates=value; }
    }
}
