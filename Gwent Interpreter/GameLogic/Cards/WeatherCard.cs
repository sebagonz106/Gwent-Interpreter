using System.Collections;
using System.Collections.Generic;
using System;

public class WeatherCard : Card
{
    public WeatherCard(string name, Faction faction, CardType cardType, List<Zone> availableRange, double initialDamage = 2, Effect effect = null) : 
                 base (name, faction, cardType, availableRange, initialDamage, effect)
    {
    }

    /* This card will have to possible non excluyent behaviours:
     * 1. an effect will activate upon being played
     * 2. a weather will be placed on the line where this card was played
     * 2.1 decrease value will be given by this weather's initial damage
     */

    public bool WeatherEffect(Context context) //reduces the damage of every silver unit card played:
    {
        ReduceSilverUnitCardPowerInLineIfPossible(Zone.Melee); //if affects melee
        ReduceSilverUnitCardPowerInLineIfPossible(Zone.Range); //if affects range
        ReduceSilverUnitCardPowerInLineIfPossible(Zone.Siege); //if affects siege
        return true;
    }

    private void ReduceSilverUnitCardPowerInLineIfPossible(Zone rangeType)
    {
        if (this.AvailableRange.Contains(rangeType))
        {
            List<Card> fidel = Player.Fidel.ListByZone[rangeType];
            List<Card> batista = Player.Batista.ListByZone[rangeType];

            for (int i = 0; i < Math.Min(fidel.Count, batista.Count); i++) //min will always be 5, as this is the top amount of cards
                                                                           //allowed in a board list. never the less, the math function
                                                                           // will be called to avoid any unwanted issue
            {
                if (batista[i] is UnitCard batistaUnit && batistaUnit.Level == Level.Silver && !Player.Batista.Battlefield.ClearsPlayed[Utils.IndexByZone[rangeType]])
                {
                    batistaUnit.Damage -= batistaUnit.Damage < initialDamage ? batistaUnit.Damage : initialDamage;
                }
                if (fidel[i] is UnitCard fidelUnit && fidelUnit.Level == Level.Silver && !Player.Fidel.Battlefield.ClearsPlayed[Utils.IndexByZone[rangeType]])
                {
                    fidelUnit.Damage -= fidelUnit.Damage < initialDamage ? fidelUnit.Damage : initialDamage;
                }
            }
        }
    }
}