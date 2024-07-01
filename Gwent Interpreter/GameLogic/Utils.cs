using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Faction
{
    Fidel,
    Batista,
}

public enum CardType
{
    Unit,
    Bonus,
    Leader,
    Weather,
    Clear,
    Bait,
}

//[Flags]
public enum Zone
{
    Melee,
    Range,
    Siege,
}

public enum Level
{
    Silver,
    Golden,
}

public delegate bool Effect(Context context);

public static class Utils 
{
    static CardSO baseCard = Resources.Load<CardSO>("Empty");
    public static Card BaseCard = new Card(baseCard);

    public static string[] ZonesName = { "Weather", "Batista Bonus", "Batista Melee", "Batista Range", "Batista Siege", "Fidel Bonus", "Fidel Melee", "Fidel Range", "Fidel Siege" };
    public static Dictionary<Zone, int> IndexByZone = new Dictionary<Zone, int> { { Zone.Melee, 0 }, { Zone.Range, 1 }, { Zone.Siege, 2 } };

    public static Player GetPlayerByName(string name) => name == "Fidel" ? Player.Fidel : Player.Batista;
    public static Player GetEnemyByName(string name) => name == "Batista" ? Player.Fidel : Player.Batista;
}