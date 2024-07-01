using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Context
{
    public Board Board { get; private set; }
    public Player CurrentPlayer { get; private set; }
    public Player EnemyPlayer { get; private set; }
    public List<Card> CurrentPosition { get; private set; }
    public Card CurrentCard { get; private set; }

    public Context(Player player, Player enemy)
    {
        this.Board = Board.Instance;
        this.CurrentPlayer = player;
        this.EnemyPlayer = enemy;
        this.CurrentPosition = this.Board.Weather;
        this.CurrentCard = Utils.BaseCard;
    }

    public Context UpdatePlayerInstance(List<Card> position, Card card)
    {
        this.CurrentPosition = position;
        this.CurrentCard = card;

        return this;
    }
}
