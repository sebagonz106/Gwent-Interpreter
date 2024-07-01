using System.Collections;
using System.Collections.Generic;

public class LeaderCard : Card, IEffect
{
    public bool NeedsCardSelection { get; private set; }

    public LeaderCard(string name, Faction faction, CardType cardType, List<Zone> availableRange, VisualInfo info, List<Card> currentPosition, bool needsCardSelection) : base(name, faction, cardType, availableRange, info, currentPosition)
    {
        NeedsCardSelection = needsCardSelection;
    }

    public bool Effect (Context context) => NeedsCardSelection? KeepInBattlefield(context.CurrentPlayer, context.CurrentCard, context.CurrentPosition) : StealCard(context.CurrentPlayer);


    private bool KeepInBattlefield(Player player, Card card, List<Card> list)
    {
        if (player.LeaderEffectUsedThisRound || !this.NeedsCardSelection || !player.Battlefield.StaysInBattlefieldModifier(card, list)) return false;

        player.LeaderEffectUsedThisRound = true;
        return true;
    }

    private bool StealCard(Player player)
    {
        if (player.LeaderEffectUsedThisRound || this.NeedsCardSelection || !player.GetCard()) return false;

        player.LeaderEffectUsedThisRound = true;
        return true;
    }
}
