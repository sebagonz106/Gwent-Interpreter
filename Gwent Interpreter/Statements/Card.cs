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
            if (onActivation is null) errors = new List<string>();
            else onActivation.CheckSemantic(out errors);

            if (type.Return != ReturnType.String) errors.Add("Invalid type declared" + position +" (string expected)");
            if (name.Return != ReturnType.String) errors.Add("Invalid name declared" + position + " (string expected)");
            if (faction.Return != ReturnType.String) errors.Add("Invalid faction declared" + position + " (string expected)");
            if (!(damage is null) && damage.Return != ReturnType.Num) errors.Add("Invalid damage declared" + position + " (number expected)");

            for (int i = 0; i < range.Count; i++)
                if (range[i].Return != ReturnType.String) errors.Add("Invalid range declared" + position + " (string expected at range no. " + i +")");

            return errors.Count == 0;
        }

        public void Execute()
        {
            string _faction = (string)this.faction.Evaluate();
            Faction faction = _faction == "Batista"? Faction.Batista : 
                              _faction == "Fidel"? Faction.Fidel : throw new EvaluationError("Invalid faction declared" + position + " (factions include: \"Fidel\", \"Batista\")");
            List<Zone> zones = new List<Zone>();
            double damage = this.damage is null? 0 : ((Num)this.damage.Evaluate()).Value;
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
                        throw new EvaluationError("Invalid range declared" + position + " (ranges include: \"Melee\", \"Ranged\", \"Siege\")");
                }
            }

            switch ((string)type.Evaluate())
            {
                case "Oro":
                    cards.Add(new UnitCard(name, faction, CardType.Unit, zones, Level.Golden, damage));
                    break;
                case "Plata":
                    cards.Add(new UnitCard(name, faction, CardType.Unit, zones, Level.Silver, damage));
                    break;
                case "Weather":
                    cards.Add(new WeatherCard(name, faction, CardType.Weather, zones, damage));
                    break;
                case "Bonus":
                    cards.Add(new BonusCard(name, faction, CardType.Bonus, zones, damage));
                    break;
                case "Bait":
                    cards.Add(new BaitCard(name, faction, CardType.Bait, zones, damage));
                    break;
                case "Clear":
                    cards.Add(new ClearCard(name, faction, CardType.Clear, zones, damage));
                    break;
                case "Leader":
                    cards.Add(new LeaderCard(name, faction, CardType.Leader, zones, damage));
                    break;
                default:
                    throw new EvaluationError("Invalid type declared" + position + " (types include: \"Oro\", \"Plata\", \"Weather\", \"Bonus\", \"Clear\", \"Bait\"), \"Leader\")");
            }

            if (!(onActivation is null)) cards[cards.Count - 1].AssignEffect((Context context) => {
                try
                {
                    onActivation.Execute();
                    return true;
                }
                catch (EvaluationError)
                {
                    return false;
                }
            });
        }

        string position => $"in card declaration at {coordinates.Item1}:{coordinates.Item2}";

        public (int, int) Coordinates => coordinates;
    }
}
