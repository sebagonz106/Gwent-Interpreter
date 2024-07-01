using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Board
{
    #region Fields, properties and builder
    int roundCount = 0;
    bool newRound = true;
    public List<Card> Weather = Enumerable.Repeat<Card>(Utils.BaseCard, 3).ToList<Card>();
    public bool AlmeidaIsPlayed = false;
    private static Board instance = new Board();
    public Dictionary<string, List<Card>> ZonesList;

    public bool IsBatistaPlayingOrAboutToPlay { get; private set; }
    public static Board Instance => instance == null ? SetBoard() : instance;
    public int RoundCount { get => roundCount; private set => roundCount = value; }
    public bool ValidTurn { get; set; }

    private Board() { }

    private static Board SetBoard()
    {
        instance = new Board();
        instance.ZonesList = new Dictionary<string, List<Card>>
        {
            {"Weather", instance.Weather },
            {"Batista Bonus",  Player.Batista.Battlefield.Bonus},
            {"Batista Melee",  Player.Batista.Battlefield.Melee},
            {"Batista Range",  Player.Batista.Battlefield.Range},
            {"Batista Siege",  Player.Batista.Battlefield.Siege},
            {"Fidel Bonus",  Player.Fidel.Battlefield.Bonus},
            {"Fidel Melee",  Player.Fidel.Battlefield.Melee},
            {"Fidel Range",  Player.Fidel.Battlefield.Range},
            {"Fidel Siege",  Player.Fidel.Battlefield.Siege},

        };
        return instance;
    }
    #endregion

    #region Functions
    #region Rounds Control
    public void StartRoundIfNecesary()
    {
        if (newRound)
        {
            if (RoundCount == 0)
            {
                Player.Fidel.GetCard(10);
                Player.Batista.GetCard(10);
            }
            else
            {
                Player.Fidel.GetCard(2);
                Player.Batista.GetCard(2);
                if (!Player.Fidel.Leader.NeedsCardSelection) Player.Fidel.LeaderEffectUsedThisRound = false;
                if (!Player.Batista.Leader.NeedsCardSelection) Player.Batista.LeaderEffectUsedThisRound = false;
            }

            RoundCount++;
            newRound = false;
        }
    }

    public bool EndTurn(Player player)
    {
        if (!ValidTurn) return false;
        ValidTurn = true;

        if (IsBatistaPlayingOrAboutToPlay) IsBatistaPlayingOrAboutToPlay = Player.Fidel.EndRound;
        else IsBatistaPlayingOrAboutToPlay = Player.Batista.EndRound;

        return true;
    }

    public bool EndRound(Player player)
    {
        if (ValidTurn) return false;
        ValidTurn = true;

        if (IsBatistaPlayingOrAboutToPlay) Player.Batista.EndRound = true;
        else Player.Fidel.EndRound = true;

        return true;
    }
    public bool CheckNextRound(out string winner)
    {
        winner = "";

        if (Player.Fidel.EndRound && Player.Batista.EndRound)
        {

            if (Player.Fidel.TotalDamage > Player.Batista.TotalDamage || (this.AlmeidaIsPlayed && Player.Batista.TotalDamage - Player.Fidel.TotalDamage <= 3)) //fidel wins
            {
                SumScore(Player.Fidel, 2, Player.Batista, 0);
                IsBatistaPlayingOrAboutToPlay = false;
                winner = "Fidel";
            }
            else if (Player.Fidel.TotalDamage < Player.Batista.TotalDamage) //batista wins
            {
                SumScore(Player.Batista, 2, Player.Fidel, 0);
                IsBatistaPlayingOrAboutToPlay = true;
                winner = "Batista";
            }
            else //draw
            {
                SumScore(Player.Fidel.StartedPlaying ? Player.Batista : Player.Fidel, 1, Player.Batista.StartedPlaying ? Player.Fidel : Player.Batista, 1);
                IsBatistaPlayingOrAboutToPlay = Player.Batista.StartedPlaying;
            }

            Player.Fidel.EndRound = false;
            Player.Batista.EndRound = false;
            newRound = true;

            this.AlmeidaIsPlayed = Player.Fidel.Battlefield.StaysInBattlefieldIs("Almeida"); //checks if almeida will stay in battlefield to modify its boolean

            return true;
        }

        return false;
    }

    public bool CheckIfEnded(out string winner)
    {
        winner = "";

        if (RoundCount >= 2)
        {
            if (Player.Fidel.Score >= 4 && Player.Fidel.Score > Player.Batista.Score)
            {
                winner = "Fidel";
                return true;
            }
            else if (Player.Batista.Score >= 4 && Player.Batista.Score > Player.Fidel.Score)
            {
                winner = "Batista";
                return true;
            }

        }
        return false;
    }
    #endregion
    public void UpdateTotalDamage(Player player = null)
    {
        if(player.Equals(null))
        {
            UpdateTotalDamage(Player.Batista);
            player = Player.Fidel;
        }

        player.TotalDamage = 0;

        double[] bonus = new double[3];
        for (int i = 0; i < bonus.Length; i++)
        {
            bonus[i] = (player.Battlefield.Bonus[i] is BonusCard) ? ((BonusCard)player.Battlefield.Bonus[i]).Increase : 1;
        }

        foreach (Card card in this.Weather) //applies weather effects
        {
            if (card is WeatherCard weather) weather.Effect(player.context.UpdatePlayerInstance(this.Weather, weather));
        }

        for  (int i = 0; i < player.Battlefield.Zones.Length; i++)
        {
            foreach (Card item in player.Battlefield.Zones[i]) //sums the damage of every unit cards
            {
                if (item is UnitCard unit)
                {
                    player.TotalDamage += unit.Level == Level.Golden ? unit.Damage : bonus[i] * unit.Damage;
                    unit.ResetDamage();
                }
            }
        }
    }

    public Player GetCurrentPlayer() => IsBatistaPlayingOrAboutToPlay ? Player.Batista : Player.Fidel;
    public Player GetCurrentEnemy() => IsBatistaPlayingOrAboutToPlay ? Player.Fidel : Player.Batista;

    private void SumScore (Player winner, int scoreWinner, Player looser, int scoreLooser)
    {
        winner.Score += scoreWinner;
        winner.StartedPlaying = true;
        looser.Score += scoreLooser;
        looser.StartedPlaying = false;
    }
    #endregion
}