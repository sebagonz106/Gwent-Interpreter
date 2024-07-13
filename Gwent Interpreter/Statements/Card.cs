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

            if (type.Return != ReturnType.String) errors.Add("Invalid type declared" + position +" (string expected)");
            if (name.Return != ReturnType.String) errors.Add("Invalid name declared" + position + " (string expected)");
            if (faction.Return != ReturnType.String) errors.Add("Invalid faction declared" + position + " (string expected)");
            if (damage.Return != ReturnType.Num) errors.Add("Invalid damage declared" + position + " (string expected)");

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
                        throw new EvaluationError("Invalid range declared" + position + " (ranges include: \"Melee\", \"Ranged\", \"Siege\")");
                }
            }

            switch ((string)type.Evaluate())
            {
                case "Oro":
                    cards.Add(new UnitCard(name, faction, CardType.Unit, zones, damage, onActivation.MethodExecution));
                    break;
                case "Plata":
                    cards.Add(new UnitCard(name, faction, CardType.Unit, zones, damage, onActivation.MethodExecution));
                    break;
                case "Weather":
                    cards.Add(new WeatherCard(name, faction, CardType.Weather, zones, damage, onActivation.MethodExecution));
                    break;
                case "Bonus":
                    cards.Add(new BonusCard(name, faction, CardType.Bonus, zones, damage, onActivation.MethodExecution));
                    break;
                case "Bait":
                    cards.Add(new BaitCard(name, faction, CardType.Bait, zones, damage, onActivation.MethodExecution));
                    break;
                case "Clear":
                    cards.Add(new ClearCard(name, faction, CardType.Clear, zones, damage, onActivation.MethodExecution));
                    break;
                case "Leader":
                    cards.Add(new LeaderCard(name, faction, CardType.Leader, zones, damage, onActivation.MethodExecution));
                    break;
                default:
                    throw new EvaluationError("Invalid type declared" + position + " (types include: \"Oro\", \"Plata\", \"Weather\", \"Bonus\", \"Clear\", \"Bait\"), \"Leader\")");
            }
        }

        string position => $"in card declaration at {coordinates.Item1}:{coordinates.Item2}";
    }
}
