using System.Collections;
using System.Collections.Generic;
using System;
using NetworkManagement;
using BallPool;


public class AightBallPoolNetworkGameAdapter : NetworkGameAdapter
{
    [Obsolete]
    public static bool is3DGraphics { get => true; set { } }
    [Obsolete]
    public static bool isSameGraphicsMode { get => true; set { } }

    public void SetTurn(int turnId)
    {
        BallPoolPlayer.turnId = turnId;
    }
    public void OnMainPlayerLoaded(int playerId, string name, int coins, object avatar, string avatarURL, int prize)
	{
        if(!BallPoolPlayer.initialized)
		{
            BallPoolPlayer.players = new BallPoolPlayer[2];
            BallPoolPlayer.playersCount = 2;
		}
        BallPoolPlayer.players[0] = new AightBallPoolPlayer(0, name, coins);
	}
	public void OnUpdateMainPlayerName (string name)
	{
		AightBallPoolPlayer.mainPlayer.name = name;
	}
}
