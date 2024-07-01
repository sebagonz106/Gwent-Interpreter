using System.Collections;
using System.Collections.Generic;

public class BonusCard : Card
{
    public double Increase { get; private set; }

    public BonusCard(string name, Faction faction, CardType cardType, List<Zone> availableRange, VisualInfo info, List<Card> currentPosition, double increase) : base(name, faction, cardType, availableRange, info, currentPosition)
    {
        this.Increase = increase;
    }
}
