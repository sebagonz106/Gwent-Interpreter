using System;
using System.Collections.Generic;
using System.Text;

namespace Gwent_Interpreter.Statements
{
    class CardStatement : IStatement
    {
        (int, int) coordinates;
        IExpression type;
        IExpression name;
        IExpression faction;
        List<IExpression> range;
        IExpression damage;
        OnActivation onActivation;

        static List<Card> cards = new List<Card>();

        public CardStatement((int, int) coordinates, IExpression type, IExpression name, IExpression faction, List<IExpression> range, IExpression damage, OnActivation onActivation)
        {
            this.coordinates = coordinates;
            this.type = type;
            this.name = name;
            this.faction = faction;
            this.range = range;
            this.damage = damage;
            this.onActivation = onActivation;
        }

        public static List<Card> Cards => cards;


        public bool CheckSemantic(out List<string> errors)
        {
            onActivation.CheckSemantic(out errors);

            if (type.Return != ReturnType.String) errors.Add("invalid type");
            if (name.Return != ReturnType.String) errors.Add("invalid name");
            if (faction.Return != ReturnType.String) errors.Add("invalid faction");
            if (damage.Return != ReturnType.Num) errors.Add("invalid damage");

            foreach (var item in range)
            {
                if (item.Return != ReturnType.String) errors.Add("invalid range");
            }

            return errors.Count == 0;
        }

        public void Execute()
        {
            Faction faction = this.faction.Evaluate().Equals("Batista") ? Faction.Batista : Faction.Fidel;
            List<Zone> zones = new List<Zone>();
            double damage = ((Num)this.damage.Evaluate()).Value;
            string name = (string)this.name.Evaluate();

            foreach (var item in range)
            {
                switch ((string)item.Evaluate())
                {
                    case "Melee":
                        zones.Add(Zone.Melee);
                        break;
                    case "Ranged":
                        zones.Add(Zone.Range);
                        break;
                    case "Siege":
                        zones.Add(Zone.Siege);
                        break;
                    default:
                        break;
                }
            }

            switch ((string)type.Evaluate())
            {
                case "Oro":
                    cards.Add(new UnitCard(name, faction, CardType.Unit, zones, null, null, damage, onActivation.MethodExecution));
                    break;
                case "Plata":
                    cards.Add(new UnitCard(name, faction, CardType.Unit, zones, null, null, damage, onActivation.MethodExecution));
                    break;
                default:
                    throw new EvaluationError("Invalid type");
            }
        }
    }
}
