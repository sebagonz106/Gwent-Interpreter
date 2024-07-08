using System.Collections;
using System.Collections.Generic;
using Gwent_Interpreter.GameLogic;

public class Card
{
    public string Name { get; }
    public Faction Faction { get; }
    public CardType CardType { get; }
    public List<Zone> AvailableRange { get; }
    public List<Card> CurrentPosition { get; }
    public VisualInfo Info { get; }

    public Player Owner => GwentInterpreterContext.Context.Players[Faction];
    public int Power
    {
        get => this is UnitCard unit ? (int)unit.DamageOnField : 0;
        set
        {
            if (this is UnitCard unit) unit.ModifyOnFieldDamage(value);
        }
    }

    public Card(string name, Faction faction, CardType cardType, List<Zone> availableRange, VisualInfo info, List<Card> currentPosition)
    {
        this.Name = name;
        this.Faction = faction;
        this.CardType = cardType;
        this.AvailableRange = availableRange;
        this.CurrentPosition = currentPosition;
        this.Info = info;
    }

    public override bool Equals(object other)
    {
        return other is Card card && this.Name == card.Name;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}
