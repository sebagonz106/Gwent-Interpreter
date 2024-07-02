using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Gwent_Interpreter.GameLogic
{
    public static class GwentInterpreterContext
    {
        public static Dictionary<Faction, Player> Players = new Dictionary<Faction, Player> { { Faction.Fidel, Player.Fidel }, { Faction.Batista, Player.Batista } };
        static Board board = Player.Fidel.context.Board;

        public static Player TriggerPlayer => board.GetCurrentPlayer();

        public static GwentList DeckOfPlayer(Player player) => new GwentList(player.Deck, player);
        public static GwentList Deck => DeckOfPlayer(TriggerPlayer);
        public static GwentList HandOfPlayer(Player player) => new GwentList(player.Hand, player);
        public static GwentList Hand => HandOfPlayer(TriggerPlayer);
        public static GwentList FieldOfPlayer(Player player) => new GwentList(player.Battlefield.CardsInBattlefield, player);
        public static GwentList Field => FieldOfPlayer(TriggerPlayer);
        public static GwentList GraveyardOfPlayer(Player player) => new GwentList(player.Battlefield.Graveyard, player);
        public static GwentList Graveyard => GraveyardOfPlayer(TriggerPlayer);
        public static GwentList Board
        {
            get
            {
                List<Card> list = new List<Card>();
                foreach (var player in Players.Values)
                {
                    list.AddRange(player.Battlefield.CardsInBattlefield);
                }
                return new GwentList(list);
            }
        }
    }

    public class GwentList
    {
        List<Card> list;
        Board board;
        Player player;

        public GwentList(List<Card> list, Player player = null)
        {
            this.list = list;
            if (player is null) board = Board.Instance;
            else this.player = player;
        }
        public GwentList() { }
        
        Card this[int index] => list[index];

        public void Remove(Card card)
        {
            if(player is null)
            {
                (card.Faction == Faction.Fidel? Player.Fidel.Battlefield : Player.Batista.Battlefield).ToGraveyard(card);
            }
            else player.Battlefield.ToGraveyard(card);
        }
        public GwentList Find(Predicate<Card> predicate) => new GwentList(list.FindAll(predicate), player);
        public void Push(Card card) => list.Add(card);
        public Card Pop()
        {
            Card card = list[list.Count - 1];
            list.RemoveAt(list.Count - 1);
            return card;
        }
        public void Shuffe()
        {
            int randomNumber;
            Card swapCard;

            for (int i = list.Count - 1; i >= 0; i--)
            {
                randomNumber = (new System.Random()).Next(list.Count - 1);
                swapCard = list[randomNumber];
                list[randomNumber] = list[i];
                list[i] = swapCard;
            }
        }
        public void SendBottom(Card card) => list.Insert(0, card);
    }
}
