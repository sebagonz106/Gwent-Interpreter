using System.Collections;
using System.Collections.Generic;

public class ClearCard : Card
{
    public ClearCard(string name, Faction faction, CardType cardType, List<Zone> availableRange, double initialDamage = 0, Effect effect= null) : 
                base(name, faction, cardType, availableRange, initialDamage, effect)
    {
    }

    /* This card will have to possible non excluyent behaviours:
     * 1. an effect will activate upon being played
     * 2. line of cards where played will be protected from weather effects
     */
}