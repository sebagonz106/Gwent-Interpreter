using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player
{
    #region Fields, properties and builder
    Faction playerName;
    public List<Card> Hand = new List<Card>(10);
    private List<int> emptySlotsInHand = new List<int>(10);
    public List<Card> Deck = new List<Card>(25);
    public LeaderCard Leader;
    public bool LeaderCardSelected = false;
    public bool LeaderEffectUsedThisRound = false;
    public bool EndRound = false;
    public bool StartedPlaying = false;
    public double TotalDamage = 0;
    public int Score = 0;
    public Battlefield Battlefield;
    public Context context;
    private static Player fidel;
    private static Player batista;

    public static Player Fidel => fidel == null ? SetPlayer(ref fidel, batista, Faction.Fidel) : fidel;
    public static Player Batista => batista == null ? SetPlayer(ref batista, fidel, Faction.Batista) : batista;
    public Faction PlayerName => playerName;

    public Dictionary<string, List<Card>> ListByName;
    public Dictionary<Zone, List<Card>> ListByZone;
    public Dictionary<List<Card>, Zone> ZoneByList;

    private Player(Faction faction)
    {
        Hand = Enumerable.Repeat<Card>(Utils.BaseCard, 10).ToList<Card>();
        for (int i = 0; i < 10; i++)
        {
            emptySlotsInHand.Add(i);
        }
    }

    private static  Player SetPlayer(ref Player player, Player enemy, Faction faction)
    {
        player = new Player(faction);
        player.context = new Context(player, enemy);
        player.Battlefield = new Battlefield(player);
        player.ListByName = new Dictionary<string, List<Card>> {
                                                                { "Melee", player.Battlefield.Melee },
                                                                { "Range", player.Battlefield.Range },
                                                                { "Siege", player.Battlefield.Siege },
                                                                { "Bonus", player.Battlefield.Bonus },
                                                                { "Weather", Board.Instance.Weather },
                                                                { "Hand", player.Hand }
                                                               };
        player.ListByZone = new Dictionary<Zone, List<Card>> {
                                                                { Zone.Melee, player.Battlefield.Melee },
                                                                { Zone.Range, player.Battlefield.Range },
                                                                { Zone.Siege, player.Battlefield.Siege }
                                                             };
        player.ZoneByList= new Dictionary<List<Card>, Zone> {
                                                                { player.Battlefield.Melee, Zone.Melee },
                                                                { player.Battlefield.Range, Zone.Range },
                                                                { player.Battlefield.Siege, Zone.Siege }
                                                             };
        return player;
    }
    #endregion

    #region Functions
    public bool GetCard(int cardsToSteal = 1)
    {
        if (Deck.Count <= cardsToSteal) //If there are no longer cards left in deck, this conditional shuffles
                                        //graveyard cards and add them to the deck so the game goes on
        {
            int randomNumber;
            Card swapCard;

            for (int k = Battlefield.Graveyard.Count - 1; k >= 0; k--)
            {
                randomNumber = Random.Range(0, Battlefield.Graveyard.Count - 1);
                swapCard = Battlefield.Graveyard[randomNumber];
                Battlefield.Graveyard[randomNumber] = Battlefield.Graveyard[k];
                Battlefield.Graveyard[k] = swapCard;
            }

            Deck.AddRange(Battlefield.Graveyard);
            Battlefield.Graveyard.Clear();
        }

        while (emptySlotsInHand.Count < cardsToSteal) //if cardsToSteal is bigger than the amount of empty slots in players hand, this sends 
                                                      //a random card in deck to graveyard until a proper value for cardsToSteal is achieved
        {
            int index = Random.Range(0, Deck.Count - 1);
            Battlefield.Graveyard.Add(Deck[index]);
            Deck.RemoveAt(index);
            cardsToSteal--;
        }

        if (cardsToSteal == 0) return false;

        for (int count = cardsToSteal; count > 0; count--)
        {
            int index = Random.Range(0, Deck.Count - 1);
            Hand[emptySlotsInHand[0]] = Deck[index];
            Deck.RemoveAt(index);
            emptySlotsInHand.RemoveAt(0);
        }

        return true;
    }

    public bool PlayCard(int originPosition, int targetPosition, Zone rangeType, out bool effectFailed)
    {
        effectFailed = false;

        if (!(this.Hand[originPosition] is Card card) || card.Equals(Utils.BaseCard) || card is BaitCard || Board.Instance.ValidTurn) //in case of unexpected behaviours. bait cards will be played through their effect
        {
            return false;
        }

        if (card is WeatherCard weather && Board.Instance.Weather[targetPosition].Equals(Utils.BaseCard)) //play weather card
        {
            Board.Instance.Weather[targetPosition] = weather;
            weather.PlayerThatPlayedThisCard = this;
        }
        else if (this.Battlefield.AddCard(card, rangeType, targetPosition)) //play unit, clear and bonus card
        {
            if (card is UnitCard unit) //activating unit cart effect
            {
                if (!unit.Effect(this.context.UpdatePlayerInstance(this.ListByZone[rangeType], unit)))
                {
                    effectFailed = true;
                }
            }
        }
        else
        {
            return false;
        }

        Board.Instance.ValidTurn = true;
        EmptyHandAt(originPosition);
        return true;
    }

    public void EmptyHandAt(int index)
    {
        this.emptySlotsInHand.Add(index);
        this.Hand[index] = Utils.BaseCard;
    }

    public override bool Equals(object other)
    {
        return other is Player otherPlayer && this.PlayerName == otherPlayer.PlayerName;
    }
    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
    #endregion
}