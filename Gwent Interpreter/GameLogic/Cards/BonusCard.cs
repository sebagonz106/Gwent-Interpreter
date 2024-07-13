using System.Collections;
using System.Collections.Generic;

public class BonusCard : Card
{
    public double Increase { get; private set; }

    public BonusCard(string name, Faction faction, CardType cardType, List<Zone> availableRange, double initialDamage = 0, Effect effect = null) : base(name, faction, cardType, availableRange, initialDamage, effect)
    {
        this.Increase = initialDamage == 0? 1 : initialDamage>=10? initialDamage/10 : initialDamage;
    }

    /* This card will have to possible non excluyent behaviours:
     * 1. an effect will activate upon being played
     * 2. a bonus will be placed on the line where this card was played
     * 2.1 bonus value will be given by this bonus' initial damage
     */
}
