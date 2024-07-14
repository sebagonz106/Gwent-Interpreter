using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Gwent_Interpreter.Utils
{
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
            if (player is null)
            {
                (card.Faction == Faction.Fidel ? Player.Fidel.Battlefield : Player.Batista.Battlefield).ToGraveyard(card);
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
