using System.Collections;
using System.Collections.Generic;

public class LeaderCard : Card
{
    public bool NeedsCardSelection { get; private set; }

    public LeaderCard(string name, Faction faction, CardType cardType, List<Zone> availableRange, double initialDamage = 0, Effect effect = null, bool needsCardSelection = false) : 
                 base(name, faction, cardType, availableRange, initialDamage, effect)
    {
        NeedsCardSelection = needsCardSelection;
    }

    public override bool Effect(Context context)
    {
        try
        {
            return !(effect is null)? effect.Invoke(context) : 
                                      NeedsCardSelection? KeepInBattlefield(context.CurrentPlayer, context.CurrentCard, context.CurrentPosition) : 
                                      StealCard(context.CurrentPlayer);
        }
        catch(System.NullReferenceException)
        {
            return false;
        }
    }

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
