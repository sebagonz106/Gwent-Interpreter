using System.Collections;
using System.Collections.Generic;

public class WeatherCard : Card, IEffect, ICardsPlayableInCommonPositions
{
    public Player PlayerThatPlayedThisCard { get; set; }

    public WeatherCard(string name, Faction faction, CardType cardType, List<Zone> availableRange, VisualInfo info, List<Card> currentPosition) : base (name, faction, cardType, availableRange, info, currentPosition)
    {
    }

    public bool Effect(Context context) //reduces the damage of every silver unit card played:
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

            for (int i = 0; i < 5; i++)
            {
                if (batista[i] is UnitCard batistaUnit && batistaUnit.Level == Level.Silver && !Player.Batista.Battlefield.ClearsPlayed[Utils.IndexByZone[rangeType]])
                {
                    batistaUnit.Damage -= batistaUnit.Damage < 2 ? batistaUnit.Damage : 2;
                }
                if (fidel[i] is UnitCard fidelUnit && fidelUnit.Level == Level.Silver && !Player.Fidel.Battlefield.ClearsPlayed[Utils.IndexByZone[rangeType]])
                {
                    fidelUnit.Damage -= fidelUnit.Damage < 2 ? fidelUnit.Damage : 2;
                }
            }
        }
    }
}