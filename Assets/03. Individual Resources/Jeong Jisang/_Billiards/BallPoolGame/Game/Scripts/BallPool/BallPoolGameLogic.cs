using Appnori.Util;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BallPool
{
    public enum PlayMode
    {
        PlayerAI = 0,
        HotSeat,
        OnLine,
        Replay,
        Solo,
    }
    /// <summary>
    /// The game current state.
    /// </summary>
    public abstract class GameState
    {
        public bool gameIsComplete = false;
        public bool needToChangeTurn { get => NeedToChangeTurn.Value; set => NeedToChangeTurn.Value = value; }
        public readonly Notifier<bool> NeedToChangeTurn;
        public GameState()
        {
            gameIsComplete = false;
            NeedToChangeTurn = new Notifier<bool>();
            needToChangeTurn = false;
            NeedToChangeTurn.OnDataChanged += NeedToChangeTurn_OnDataChanged;
        }

        private void NeedToChangeTurn_OnDataChanged(bool obj)
        {
            Debug.Log("NeedToChangeTurn modified : " + obj);
        }
    }
    /// <summary>
    /// The game logic.
    /// </summary>
    public class BallPoolGameLogic
    {
        public static BallPoolGameLogic instance
        {
            get;
            private set;
        }
        public BallPoolGameLogic()
        {
            instance = this;
        }
        /// <summary>
        /// The play mode, (PlayerAI, HotSeat, OnLine or Replay).
        /// </summary>
        public static PlayMode playMode;

        /// <summary>
        /// The game is online
        /// </summary>
        public static bool isOnLine
        {
            get
            {
                return playMode == PlayMode.OnLine && BallPoolGameLogic.instance != null;
            }
        }
        /// <summary>
        /// The game is control with opponent from network.
        /// </summary>
        public static bool controlFromNetwork
        {
            get
            {
                return playMode == PlayMode.OnLine && !BallPoolPlayer.mainPlayer.myTurn && BallPoolGameLogic.instance != null;
            }
        }
        /// <summary>
        /// The game is control with main player in network.
        /// </summary>
        public static bool controlInNetwork
        {
            get
            {
                return playMode == PlayMode.OnLine && BallPoolPlayer.mainPlayer.myTurn && BallPoolGameLogic.instance != null;
            }
        }

        /// <summary>
        /// Calls when game time has ended.
        /// </summary>
        public virtual void OnEndTime()
        {

        }
        public virtual void Deactivate()
        {
            instance = null;
        }
    }
}
