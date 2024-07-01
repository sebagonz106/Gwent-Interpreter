using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CardsWarehouse
{
    public static List<Card> RebelCards = new List<Card>
    {
        new LeaderCard("Camilo Cienfuegos", Faction.Fidel, CardType.Leader, new List<Zone>{ }, new VisualInfo(), Player.Fidel.Deck, false),
        new LeaderCard("Ernesto Che Guevara", Faction.Fidel, CardType.Leader, new List<Zone>{ }, new VisualInfo(), Player.Fidel.Deck, true),
        new BaitCard("Retirada estrategica", Faction.Fidel, CardType.Bait, new List<Zone>{Zone.Melee, Zone.Range, Zone.Siege}, new VisualInfo(), Player.Fidel.Deck),
        new ClearCard("Capas y linternas", Faction.Fidel, CardType.Clear, new List<Zone>{Zone.Melee, Zone.Range, Zone.Siege}, new VisualInfo(), Player.Fidel.Deck),
        new ClearCard("Mantas", Faction.Fidel, CardType.Clear, new List<Zone>{Zone.Melee, Zone.Range, Zone.Siege}, new VisualInfo(), Player.Fidel.Deck),
        new ClearCard("Santeria", Faction.Fidel, CardType.Clear, new List<Zone>{Zone.Melee, Zone.Range, Zone.Siege}, new VisualInfo(), Player.Fidel.Deck),
        new WeatherCard("Lluvia", Faction.Fidel, CardType.Weather, new List<Zone>{Zone.Melee, Zone.Range, Zone.Siege}, new VisualInfo(), Player.Fidel.Deck),
        new WeatherCard("Frente Frio", Faction.Fidel, CardType.Weather, new List<Zone>{Zone.Melee, Zone.Range, Zone.Siege}, new VisualInfo(), Player.Fidel.Deck),
        new WeatherCard("Niebla", Faction.Fidel, CardType.Weather, new List<Zone>{Zone.Melee, Zone.Range, Zone.Siege}, new VisualInfo(), Player.Fidel.Deck),
    };
    public static List<Card> BatistaCards = new List<Card>
    {
        new LeaderCard("Eulogio Cantillo", Faction.Batista, CardType.Leader, new List<Zone>{ }, new VisualInfo(), Player.Batista.Deck, false),
        new LeaderCard("Francisco Tabernilla", Faction.Batista, CardType.Leader, new List<Zone>{ }, new VisualInfo(), Player.Batista.Deck, true),
        new BaitCard("Retirada estrategica", Faction.Batista, CardType.Bait, new List<Zone>{Zone.Melee, Zone.Range, Zone.Siege}, new VisualInfo(), Player.Batista.Deck),
        new ClearCard("Capas y linternas", Faction.Batista, CardType.Clear, new List<Zone>{Zone.Melee, Zone.Range, Zone.Siege}, new VisualInfo(), Player.Batista.Deck),
        new ClearCard("Mantas", Faction.Batista, CardType.Clear, new List<Zone>{Zone.Melee, Zone.Range, Zone.Siege}, new VisualInfo(), Player.Batista.Deck),
        new ClearCard("Santeria", Faction.Batista, CardType.Clear, new List<Zone>{Zone.Melee, Zone.Range, Zone.Siege}, new VisualInfo(), Player.Batista.Deck),
        new WeatherCard("Lluvia", Faction.Batista, CardType.Weather, new List<Zone>{Zone.Melee, Zone.Range, Zone.Siege}, new VisualInfo(), Player.Batista.Deck),
        new WeatherCard("Frente Frio", Faction.Batista, CardType.Weather, new List<Zone>{Zone.Melee, Zone.Range, Zone.Siege}, new VisualInfo(), Player.Batista.Deck),
        new WeatherCard("Niebla", Faction.Batista, CardType.Weather, new List<Zone>{Zone.Melee, Zone.Range, Zone.Siege}, new VisualInfo(), Player.Batista.Deck),
    };
}
