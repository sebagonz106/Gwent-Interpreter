using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Effects
{ //parameters composition: 0 Board, 1 Player player, 2 Player enemy, 3 List<Card> currentPosition, 4 Card
  //works depending on cards being played before aplying their effects.

    #region Effects storage
    static Dictionary<string, Effect> effects = new Dictionary<string, Effect>
    {
        {"Empty", VoidEffect },
        {"Juan Almeida", NoOneSurrendersHereGodDamn },
        {"Raul Castro", StealCard },
        {"Celia Sanchez", ClearsLineWithFewerCards },
        {"Luchadores clandestinos", PlaceBonusInLineWhereIsPlayed },
        {"Guajiros" , MultipliesHisDamageTimesCardsLikeThis },
        {"Cuarteles", MultipliesHisDamageTimesCardsLikeThis },
        {"Tanque de guerra", PlaceLightButIrremovableWeatherInEnemysBattlefield },
        {"Chivatos", RemoveEnemyWorstCard },
        {"Pilar Garcia", RemovePowerfulCard },
        {"Martin Diaz Tamayo", EqualsSilverUnitsDamageToBattlefieldsAverage }
    };

    public static void AddEffect(string name, Effect effect) { effects.Add(name, effect); }
    public static Effect GetEffect(string name)
    {
        try
        {
            return effects[name];
        }
        catch (System.ArgumentOutOfRangeException)
        {
            return effects["Empty"];
        }
    }
    #endregion

    #region Rebel effects

    public static bool StealCard(Context context) => context.CurrentPlayer.GetCard(); //for it to work, the card with this effect must not 
                                                                                     //be the first card played of a full hand
    public static bool NoOneSurrendersHereGodDamn(Context context)
    {
        context.Board.AlmeidaIsPlayed = true;
        return true;
    }

    public static bool ClearsLineWithFewerCards(Context context) // if there are two or more lists with the same amount of cards played
                                                                 // this method will clear the first of the lists checked, altough if an enemy 
                                                                 // list has the same amount of cards, it will clear the enemy's list.
                                                                 // In case a card set to stay in battlefield is discarded, it will reappear
                                                                 // next round, as soon as battlefield's clear method is called.
                                                                 // Creator's License here: this effect will be able to affect golden cards.
    {
        int minCount = 6; //Max amount of cards this can count
        List<Card> list = null;
        Card bonus = Utils.BaseCard;
        Player player = context.EnemyPlayer;

        for (int i = 0; i < context.CurrentPlayer.Battlefield.Zones.Length; i++)
        {

            if (CountCardsInListAndCompareWithMinCount(context.CurrentPlayer.Battlefield.Zones[i], context.CurrentPlayer, ref minCount, ref list, context.CurrentPlayer))
            {
                bonus = context.CurrentPlayer.Battlefield.Bonus[i];
                player = context.CurrentPlayer;
            }
            if (CountCardsInListAndCompareWithMinCount(context.EnemyPlayer.Battlefield.Zones[i], context.EnemyPlayer, ref minCount, ref list, context.CurrentPlayer))
            {
                bonus = context.EnemyPlayer.Battlefield.Bonus[i];
                player = context.EnemyPlayer;
            }
        }

        if (list.Equals(null)) return false;

        player.Battlefield.ToGraveyard(bonus, player.Battlefield.Bonus);
        player.Battlefield.ToGraveyard(list);

        return true;
    }

    public static bool PlaceBonusInLineWhereIsPlayed(Context context)
    {
        List<Card> currentPosition = context.CurrentPosition;
        Player player = context.CurrentPlayer;
        BonusCard riot = new BonusCard(Resources.Load<BonusCardSO>("Huelga Revolucionaria"));

        return player.Battlefield.AddCard(riot, player.ZoneByList[currentPosition]);
    }
    #endregion

    #region Batista effects

    public static bool RemovePowerfulCard(Context context)
    {
        (UnitCard, List<Card>) currentPlayerList = context.CurrentPlayer.Battlefield.SilverCardWithHighestOrLowestDamage(true);
        (UnitCard, List<Card>) enemyPlayerList = context.EnemyPlayer.Battlefield.SilverCardWithHighestOrLowestDamage(true);

        if (currentPlayerList.Item1 == null && enemyPlayerList.Item1 == null) return false; //checks if there is no unit silver card played

        //checks if enemy's most powerful card outpowers current player's most powerful card. If so, it sends enemy's card to graveyard
        if (currentPlayerList.Item1 == null || currentPlayerList.Item1.InitialDamage < enemyPlayerList.Item1.InitialDamage)
        {
            context.EnemyPlayer.Battlefield.ToGraveyard(enemyPlayerList.Item1, enemyPlayerList.Item2);
        }

        //checks if current player's most powerful card outpowers enemy's most powerful card. If so, it sends it to graveyard
        else if (enemyPlayerList.Item1 == null || currentPlayerList.Item1.InitialDamage > enemyPlayerList.Item1.InitialDamage) 
        {
            context.CurrentPlayer.Battlefield.ToGraveyard(currentPlayerList.Item1, currentPlayerList.Item2);
        }

        //If both cards inflict the same damage, it sends them to graveyard.
        else if (currentPlayerList.Item1.InitialDamage == enemyPlayerList.Item1.InitialDamage)
        {
            context.EnemyPlayer.Battlefield.ToGraveyard(enemyPlayerList.Item1, enemyPlayerList.Item2);
            context.CurrentPlayer.Battlefield.ToGraveyard(currentPlayerList.Item1, currentPlayerList.Item2);
        }

        else return false; //in case of unexpected behaviour

        return true;
    }

    public static bool RemoveEnemyWorstCard(Context context)
    {
        (Card, List<Card>) leastPower = context.EnemyPlayer.Battlefield.SilverCardWithHighestOrLowestDamage(false);
        if (leastPower.Item1 == null) return false;

        context.EnemyPlayer.Battlefield.ToGraveyard(leastPower.Item1, leastPower.Item2); //sends to graveyard the least powerful card of the given player
        return true;
    }

    public static bool PlaceLightButIrremovableWeatherInEnemysBattlefield(Context context)
    {
        Battlefield enemy = context.EnemyPlayer.Battlefield;

        for (int i = 0; i < enemy.Zones.Length; i++)
            ReduceDamagePermanently(enemy.Zones[i]);

        return true;
    }

    public static bool EqualsSilverUnitsDamageToBattlefieldsAverage(Context context)
    {
        List<UnitCard> list = new List<UnitCard>();
        double pureTotalDamage = 0;
        double average = 0;

        for (int i = 0; i < context.CurrentPlayer.Battlefield.Zones.Length; i++)
        {
            StoreUnitsInListAndCountDamage(context.CurrentPlayer.Battlefield.Zones[i], list, ref pureTotalDamage);
            StoreUnitsInListAndCountDamage(context.EnemyPlayer.Battlefield.Zones[i], list, ref pureTotalDamage);
        }

        average = pureTotalDamage / list.Count;

        foreach (UnitCard item in list)
        {
            if (item.Level == Level.Silver) item.ModifyOnFieldDamage(average, true);
        }

        return true;
    }
    #endregion

    #region Common effects
    public static bool MultipliesHisDamageTimesCardsLikeThis(Context context)
    {
        Player player = context.CurrentPlayer;
        UnitCard card = (UnitCard)context.CurrentCard;
        int count = 0; 
        List<UnitCard> list = new List<UnitCard>();

        for (int i = 0; i < player.Battlefield.Zones.Length; i++)
            FindEveryCardInListAndCountThem(player.Battlefield.Zones[i], card, list, ref count);

        foreach (UnitCard item in list)
        {
            item.ModifyOnFieldDamage(count * item.InitialDamage, true);
        }
        return true;
    }

    public static bool VoidEffect(Context context) => true;
    #endregion

    #region Utils
    static bool CountCardsInListAndCompareWithMinCount(List<Card> toCount, Player checkingPlayer, ref int minCount, ref List<Card> minCountList, Player thisPlayer)
    {
        int tempCount = 0;
        foreach (Card item in toCount)
        {
            if (!item.Equals(Utils.BaseCard)) tempCount++;
        }
        if (!thisPlayer.Battlefield.Bonus[Utils.IndexByZone[checkingPlayer.ZoneByList[toCount]]].Equals(Utils.BaseCard)) tempCount++;

        if (tempCount>0 && (tempCount<minCount||(tempCount==minCount && !checkingPlayer.Equals(thisPlayer))))
        {
            minCountList = toCount;
            minCount = tempCount;
            return true;
        }
        return false;
    }
    static void FindEveryCardInListAndCountThem(List<Card> listToSearch, UnitCard card, List<UnitCard> listToSave, ref int count)
    {
        foreach (Card item in listToSearch)
        {
            if (card.Equals(item))
            {
                count++;
                listToSave.Add((UnitCard)item);
            }
        }
    }
    static void ReduceDamagePermanently(List<Card> list, int toTake = 1)
    {
        foreach (Card item in list)
        {
            if (item is UnitCard unit && unit.Level == Level.Silver) unit.ModifyOnFieldDamage(unit.DamageOnField - toTake, true);
        }
    }
    static void StoreUnitsInListAndCountDamage(List<Card> listToCheck, List<UnitCard> listToStore, ref double damage)
    {
        foreach (Card item in listToCheck)
        {
            if (item is UnitCard unit)
            {
                listToStore.Add(unit);
                damage += unit.DamageOnField;
            }
        }
    }
    #endregion
}