using UnityEngine;
using System.Collections;

namespace Billiards
{
    using BallPool;
    using BallPool.AI;
    using BallPool.Mechanics;
    using System;

    using Random = UnityEngine.Random;

    public static class BallPoolAIExtension
    {
        /// <summary>
        /// Lower is Smart (5 ~ 1)
        /// </summary>
        public static int DumbLevel = 5;

        /// <summary>
        /// (dumb)0 ~ 4(smart).  if notuse, return -1
        /// </summary>
        public static int AILevel
        {
            get
            {
                if (BallPoolGameLogic.playMode != PlayMode.PlayerAI)
                    return -1;

                return 5 - Mathf.Clamp(DumbLevel, 0, 4);
            }
        }


        public static BestTargetBallInfo ToDumb(this BestTargetBallInfo bestInfo)
        {
            //set space
            Matrix4x4 space = Matrix4x4.identity;
            var dumbFactor = Mathf.Clamp(DumbLevel - 1, 0, 5) * GameConfig.AIDumbPerLevel + GameConfig.AIMinimumDumb;
            var weight = Random.Range(-dumbFactor, dumbFactor);

            space.SetTRS(Vector3.zero, Quaternion.Euler(Vector3.up * weight), Vector3.one);

            Vector3 shotPoint = space.MultiplyPoint(bestInfo.shotPoint);
            Vector3 aimpoint = space.MultiplyPoint(bestInfo.aimpoint);

            BestTargetBallInfo dumbInfo = new BestTargetBallInfo(bestInfo.targetBall, bestInfo.pocketId, bestInfo.shotBallPosition, shotPoint, aimpoint, bestInfo.impulse);

            return dumbInfo;
        }

        public static FindBestTargetBallException<int> BuildPlayerException(this BallPoolAIManager instance)
        {
            var exceptionFunc = new FindBestTargetBallException<int>((new Func<int, bool>((ballId) =>
            {
                if (AightBallPoolGameLogic.isCueBall(ballId))
                    return true;

                bool isBlackBall = AightBallPoolGameLogic.isBlackBall(ballId);
                if (!AightBallPoolGameLogic.gameState.playersHasBallType)
                    return isBlackBall;

                bool mainPlayerIsBlack = AightBallPoolPlayer.mainPlayer.isBlack;
                bool otherPlayerIsBlack = AightBallPoolPlayer.otherPlayer.isBlack;
                bool ballIsStripes = AightBallPoolGameLogic.isStripesBall(ballId);
                bool ballIsSolids = AightBallPoolGameLogic.isSolidsBall(ballId);

                if (AightBallPoolPlayer.mainPlayer.myTurn)
                {
                    if (mainPlayerIsBlack)
                    {
                        return !isBlackBall;
                    }
                    else if (isBlackBall)
                    {
                        return true;
                    }

                    bool mainPlayerIsStripes = AightBallPoolPlayer.mainPlayer.isStripes;
                    bool mainPlayerIsSolids = AightBallPoolPlayer.mainPlayer.isSolids;
                    if (ballIsStripes)
                    {
                        return !mainPlayerIsStripes;
                    }
                    else if (ballIsSolids)
                    {
                        return !mainPlayerIsSolids;
                    }
                }
                else if (AightBallPoolPlayer.otherPlayer.myTurn)
                {
                    if (otherPlayerIsBlack)
                    {
                        return !isBlackBall;
                    }
                    else if (isBlackBall)
                    {
                        return true;
                    }

                    bool otherPlayerIsStripes = AightBallPoolPlayer.otherPlayer.isStripes;
                    bool otherPlayerIsSolids = AightBallPoolPlayer.otherPlayer.isSolids;

                    if (ballIsStripes)
                    {
                        return !otherPlayerIsStripes;
                    }
                    else if (ballIsSolids)
                    {
                        return !otherPlayerIsSolids;
                    }
                }
                return false;
            })));

            return exceptionFunc;
        }

    }
}