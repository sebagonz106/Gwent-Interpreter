using System.Collections;
using System.Collections.Generic;

public class BaitCard : Card
{
    public BaitCard(string name, Faction faction, CardType cardType, List<Zone> availableRange, double initialDamage = 0, Effect effect = null) : 
               base(name, faction, cardType, availableRange, initialDamage, effect)
    {
    }

    public override bool Effect(Context context)
    {
        try
        {
             return effect is null ? Effect(context.CurrentPosition, context.CurrentPosition.IndexOf(context.CurrentCard)) : effect.Invoke(context);
        }
        catch (System.NullReferenceException)
        {
            return false;
        }
    }

    public bool Effect(List<Card> list, int index)
    {
        Card card = list[index];
        if (card is BaitCard) return false;
        list[index] = this;
        Owner.Hand[Owner.Hand.IndexOf(this)] = card;
        if (card is UnitCard unit) unit.InitializeDamage(); //in case any permanent effects were applied on this card
        if (card is ClearCard) Owner.Battlefield.RemoveClearEffect(Utils.IndexByZone[Owner.ZoneByList[list]]);
        this.Owner = Owner;
        return true;
    }
}
