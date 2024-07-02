using System.Collections;
using System.Collections.Generic;

public class BaitCard : Card, ICardsPlayableInCommonPositions
{
    public Player PlayerThatPlayedThisCard { get; set; }

    public BaitCard(string name, Faction faction, CardType cardType, List<Zone> availableRange, VisualInfo visual, List<Card> currentPosition) : base(name, faction, cardType, availableRange, visual, currentPosition)
    {
    }

    public bool Effect(Player player, List<Card> list, int index)
    {
        Card card = list[index];
        if (card is BaitCard) return false;
        list[index] = this;
        player.Hand[player.Hand.IndexOf(this)] = card;
        if (card is UnitCard unit) unit.InitializeDamage(); //in case any permanent effects were applied on this card
        if (card is ClearCard) player.Battlefield.RemoveClearEffect(Utils.IndexByZone[player.ZoneByList[list]]);
        this.PlayerThatPlayedThisCard = player;
        return true;
    }
}
