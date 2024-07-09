using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Collections;

namespace Gwent_Interpreter.GameLogic
{
    public class GwentInterpreterContext : IExpression
    {
        public Dictionary<Faction, Player> Players;
        Board board;

        GwentInterpreterContext()
        {
            Players = new Dictionary<Faction, Player> { { Faction.Fidel, Player.Fidel }, { Faction.Batista, Player.Batista } };
            board = Player.Fidel.context.Board;
        }

        static GwentInterpreterContext context;
        public static GwentInterpreterContext Context
        {
            get
            {
                if (context is null) context = new GwentInterpreterContext();
                return context;
            }
        }

        public Player TriggerPlayer => board.GetCurrentPlayer();

        public GwentList DeckOfPlayer(Player player) => new GwentList(player.Deck, player);
        public GwentList Deck => DeckOfPlayer(TriggerPlayer);
        public GwentList HandOfPlayer(Player player) => new GwentList(player.Hand, player);
        public GwentList Hand => HandOfPlayer(TriggerPlayer);
        public GwentList FieldOfPlayer(Player player) => new GwentList(player.Battlefield.CardsInBattlefield, player);
        public GwentList Field => FieldOfPlayer(TriggerPlayer);
        public GwentList GraveyardOfPlayer(Player player) => new GwentList(player.Battlefield.Graveyard, player);

        public ReturnType Return => ReturnType.Context;

        public bool CheckSemantic(out string error) { error = ""; return true; }

        public object Evaluate() => this;

        public GwentList Graveyard => GraveyardOfPlayer(TriggerPlayer);
        public GwentList Board
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

    public class GwentList : IList<Card>
    {
        List<Card> list;
        Board board;
        Player player;

        public int Count => list.Count;

        public bool IsReadOnly => false;

        Card IList<Card>.this[int index] { get => list[index]; set => list[index] = value; }

        public GwentList(List<Card> list, Player player = null)
        {
            this.list = list;
            if (player is null) board = Board.Instance;
            else this.player = player;
        }
        public GwentList()
        {
            list = new List<Card>();
            board = Board.Instance;
        }
        
        public Card this[Num index]
        {
            get => list[(int)index.Value];
            set => list[(int)index.Value] = value;
        }
        public Card this[int index]
        {
            get => list[index];
            set => list[index] = value;
        }

        public void Remove(Card card)
        {
            if(player is null)
            {
                (card.Faction == Faction.Fidel? Player.Fidel.Battlefield : Player.Batista.Battlefield).ToGraveyard(card);
            }
            else player.Battlefield.ToGraveyard(card);
            list.Remove(card);
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

        public int IndexOf(Card item) => list.IndexOf(item);

        public void Insert(int index, Card item) => list.Insert(index, item);

        public void RemoveAt(int index)
        {
            if (player is null)
            {
                (list[index].Faction == Faction.Fidel ? Player.Fidel.Battlefield : Player.Batista.Battlefield).ToGraveyard(list[index]);
            }
            else player.Battlefield.ToGraveyard(list[index]);

            list.RemoveAt(index);
        }

        public void Add(Card item)
        {
            throw new NotImplementedException();
        }

        public void Clear() => list.Clear();

        public bool Contains(Card item) => list.Contains(item);

        public void CopyTo(Card[] array, int arrayIndex) => list.CopyTo(array, arrayIndex);

        bool ICollection<Card>.Remove(Card item)
        {
            if (this.Contains(item)) this.Remove(item);

            return this.Contains(item);
        }

        public IEnumerator<Card> GetEnumerator() => list.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
}
