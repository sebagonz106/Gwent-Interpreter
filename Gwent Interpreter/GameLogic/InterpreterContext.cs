using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Collections;
using Gwent_Interpreter.Utils;

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
        public GwentList OtherDeck => DeckOfPlayer(board.GetCurrentEnemy());
        public GwentList HandOfPlayer(Player player) => new GwentList(player.Hand, player);
        public GwentList Hand => HandOfPlayer(TriggerPlayer);
        public GwentList OtherHand => HandOfPlayer(board.GetCurrentEnemy());
        public GwentList FieldOfPlayer(Player player) => new GwentList(player.Battlefield.CardsInBattlefield, player);
        public GwentList Field => FieldOfPlayer(TriggerPlayer);
        public GwentList OtherField => FieldOfPlayer(board.GetCurrentEnemy());
        public GwentList GraveyardOfPlayer(Player player) => new GwentList(player.Battlefield.Graveyard, player);
        public GwentList Graveyard => GraveyardOfPlayer(TriggerPlayer);
        public GwentList OtherGraveyard => GraveyardOfPlayer(board.GetCurrentEnemy());
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

        public ReturnType Return => ReturnType.Context;

        public (int, int) Coordinates => throw new NotImplementedException();

        public bool CheckSemantic(out string error) { error = ""; return true; }

        public object Evaluate() => this;

        public bool CheckSemantic(out List<string> errors) { errors = new List<string>(); return true; }
    }
}
