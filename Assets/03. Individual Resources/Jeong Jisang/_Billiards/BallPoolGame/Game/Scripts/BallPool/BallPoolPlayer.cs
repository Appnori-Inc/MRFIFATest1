using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BallPool.Mechanics;
using NetworkManagement;
using System;
using Billiards;

namespace BallPool
{
    public delegate void TurnChangedHandler();
    public delegate void PlayerActionHandler(BallPoolPlayer player);
    /// <summary>
    /// The player.
    /// </summary>
    public abstract class BallPoolPlayer
    {
        /// <summary>
        /// The main player.
        /// </summary>
        public static BallPoolPlayer mainPlayer
        {
            get{ return (players == null || players.Length < 1)? null: players[0]; }
        }
        /// <summary>
        /// Gets the player identifier in game, not social id.
        /// </summary>
        /// <value>The player identifier.</value>
        public int playerId
        {
            get;
            private set;
        }

        public string uuid
        {
            get;
            private set;
        }

        public string name 
        {
            get;
            set;
        }

        public static BallPoolPlayer GetWinner()
        {
            foreach (BallPoolPlayer player in players) 
            {
                if (player.isWinner)
                {
                    return player;
                }
            }
            return null;
        }
        public static BallPoolPlayer GetNotWinner()
        {
            foreach (BallPoolPlayer player in players)
            {
                if (!player.isWinner)
                {
                    return player;
                }
            }
            return null;
        }
        public static void SetWinner(in string uuid)
        {
            foreach (BallPoolPlayer player in players) 
            {
                player.isWinner = player.uuid == uuid;
            }
        }
        /// <summary>
        /// The current player, whose turn to play at the moment.
        /// </summary>
        public static BallPoolPlayer currentPlayer
        {
            get
            {
                foreach (BallPoolPlayer player in players) 
                {
                    if(player.playerId == turnId)
                    {
                        return player;
                    }
                }
                return null;
            }
        }

        public List<Ball> balls
        {
            get;
            protected set;
        }

        public static int playersCount
        {
            get;
            set;
        }

        /// <summary>
        /// LocalPlayer: 0. Other player: 1
        /// </summary>
        public static BallPoolPlayer[] players
        {
            get;
            set;
        }
        /// <summary>
        /// if 1: Main Player turn els , if 2: Other Player turn.
        /// </summary>
        public static int turnId
        {
            get;
            set;
        }
        public bool isWinner
        {
            get;
            set;
        }
        public bool myTurn
        {
            get;
            private set;
        }

        public int CaromScore
        {
            get;
            set;
        }

        public event Action OnPlayerStateChanged;

        public void UpdatePlayerState() => OnPlayerStateChanged?.Invoke();


        private int caromTargetScore;

        public int CaromTargetScore
        {
            get
            {
                switch (LevelLoader.CurrentMatchInfo.gameType)
                {
                    case GameType.CaromFour when LevelLoader.CurrentMatchInfo.playingType == PlayType.Single:
                        return GameConfig.CurrentCaromSingleTargetScore;

                    case GameType.CaromFour when LevelLoader.CurrentMatchInfo.playingType == PlayType.Multi:
                        return Mathf.Min(GameConfig.CaromMaxPlayerTargetScore, caromTargetScore);

                    default:
                        return caromTargetScore;
                }
            }

            set => caromTargetScore = value;
        }

        public static void Deactivate()
        {
            OnPlayerInitialized = null;
            OnTurnChanged = null;
            if (players != null)
            {
                foreach (BallPoolPlayer player in players)
                {
                    player.OnDeactivate();
                    player.isWinner = false;
                    player.myTurn = false;
                    player.balls = null;
                }
            }
        }
        public static bool initialized
        {
            get
            {
                return players != null;
            }
        }
        public static event TurnChangedHandler OnTurnChanged;
        public static event PlayerActionHandler OnPlayerInitialized;

        /// <summary>
        /// Change the players turn.
        /// </summary>
        public static void ChangeTurn(bool isEndTime = false)
        {
            UnityEngine.Debug.Log("ChangeTurn " + BallPoolPlayer.turnId);

            if(BallPoolGameLogic.playMode == PlayMode.OnLine)
            {
                //if next is my turn
                if(!BallPoolPlayer.mainPlayer.myTurn)
                {
                    //and when opponent is playing, ignore turn change
                    if (Billiards.BilliardsDataContainer.Instance.OpponentGameState.Value != ShotController.GameStateType.MoveAroundTable)
                    {
                        if (isEndTime)
                            Debug.Log($"Player's turn is unclear. opponent gamestate is {Billiards.BilliardsDataContainer.Instance.OpponentGameState.Value}, endTime is {isEndTime}");
                        else
                            Debug.LogError($"Player's turn is unclear. opponent gamestate is {Billiards.BilliardsDataContainer.Instance.OpponentGameState.Value}, endTime is {isEndTime}");

                        //ignore turn change
                        if (isEndTime == false)
                            return;
                    }
                }
            }

            if (BallPoolGameLogic.playMode == PlayMode.Solo)
            {
                BallPoolPlayer.turnId = 0;
            }
            else if (BallPoolPlayer.turnId < players.Length - 1)
            {
                BallPoolPlayer.turnId++;
            }
            else
            {
                BallPoolPlayer.turnId = 0;
            }

            for (int i = 0; i < players.Length; i++)
            {
                players[i].myTurn = BallPoolPlayer.turnId == i;
            }

            if (OnTurnChanged != null)
            {
                OnTurnChanged();
            }
        }
        /// <summary>
        /// Set the players turn.
        /// </summary>
        public static void SetTurn(int turnId)
        {
            BallPoolPlayer.turnId = turnId;
            for (int i = 0; i < players.Length; i++)
            {
                players[i].myTurn = turnId == i;
            }
            if (OnTurnChanged != null)
            {
                OnTurnChanged();
            }
        }
        /// <summary>
        /// Gets the activ balls Ides array.
        /// </summary>
        public string[] GetActiveBallsIds()
        {
            if (balls == null)
            {
                return null;
            }
            string[] data = new string[balls.Count];
            for (int i = 0; i < balls.Count; i++)
            {
                data[i] = balls[i].id + "";
            }
            return data;
        }
        public BallPoolPlayer(int playerId, string name, int coins, string uuid)
        {
            this.playerId = playerId;
            this.name = name;
            this.uuid = uuid;
            if (OnPlayerInitialized != null)
            {
                OnPlayerInitialized(this);
            }
        }

        public abstract void OnDeactivate();

        public virtual void SetActiveBalls(Ball[] balls)
        {
            this.balls = new List<Ball>(0);
            foreach (Ball ball in balls)
            {
                if (!ball.inPocket)
                {
                    this.balls.Add(ball);
                }
            }
        }
    }
}
