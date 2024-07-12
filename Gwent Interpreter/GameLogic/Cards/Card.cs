using System.Collections;
using System.Collections.Generic;
using Gwent_Interpreter.GameLogic;

public class Card : IEffect, ICardsPlayableInCommonPositions
{
    public string Name { get; }
    public Faction Faction { get; }
    public CardType CardType { get; }
    public List<Zone> AvailableRange { get; }
    public List<Card> CurrentPosition { get; private set; }
    public VisualInfo Info { get; private set; }
    protected Effect effect;
    protected double initialDamage;

    public int Power
    {
        get => this is UnitCard unit ? (int)unit.DamageOnField : (int)initialDamage;
        set
        {
            if (this is UnitCard unit) unit.ModifyOnFieldDamage(value);
        }
    }

    public Player Owner { get => GwentInterpreterContext.Context.Players[Faction]; set => Owner = value; }

    public Card(string name, Faction faction, CardType cardType, List<Zone> availableRange, double damage = 0, Effect effect = null)
    {
        this.Name = name;
        this.Faction = faction;
        this.CardType = cardType;
        this.AvailableRange = availableRange;
        this.effect = effect;
        initialDamage = damage;
    }

    public override bool Equals(object other)
    {
        return other is Card card && this.Name == card.Name;
    }

    public override int GetHashCode()
    {
        return this.Name.GetHashCode();
    }

    public virtual bool Effect(Context context)
    {
        try
        {
            return effect is null ? true : effect.Invoke(context);
        }
        catch (System.NullReferenceException)
        {
            return false;
        }
    }

    public void AssignPosition(List<Card> currentPosition) => this.CurrentPosition = currentPosition is null ? Owner.Hand : currentPosition;

    public void AssignInfo(VisualInfo info) => this.Info = info;
}
