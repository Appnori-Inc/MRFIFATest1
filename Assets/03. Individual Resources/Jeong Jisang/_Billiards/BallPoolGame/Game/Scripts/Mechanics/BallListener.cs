using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BallPool.Mechanics;
using Billiards;

namespace BallPool
{
    public class BallListener : MonoBehaviour
    {
        public static event System.Action<BallListener, GameObject> OnBoardHit;

        public int id;
        public Rigidbody body;

        public PocketListener pocket{ get; set; }

        public PhysicsManager physicsManager;

        public float radius{ get; private set; }

        public int pocketId{ get; set; }

        public int hitShapeId{ get; set; }

        public bool isInPocket { get => body.isKinematic; }

        public Vector3 normalizedVelocity{ get { return body.velocity / physicsManager.ballMaxVelocity; } }

        private bool firstHit = false;
        private bool inMove;

        void OnCollisionEnter(Collision other)
        {
            if (physicsManager.inMove)
            {
                firstHit = true;
            }
            if (!firstHit && other.gameObject.layer == Appnori.Layer.NameToLayer(Appnori.Layer.GameType.Billiards, "Cloth"))
            {
                firstHit = true;
                if (id != 0)
                {
                    body.Sleep();
                }
            }
        }

        public void OnTriggerEnter(Collider other)
        {
            if (BallPoolGameLogic.playMode == PlayMode.Replay || BallPoolGameLogic.controlFromNetwork)
            {
                return;
            }
            PocketListener pocket = other.GetComponent<PocketListener>();
            if (pocket)
            {
                OnEnterPocket(pocket);
            }
        }

        public void OnEnterPocket(PocketListener pocket)
        {
            if (!isInPocket)
            {
                body.isKinematic = true;
                pocketId = pocket.id;
                hitShapeId = -2;
                Debug.Log(id + " OnEnterPocket");
                physicsManager.CallOnBallHitPocket(this, pocket, true);
            }
        }


        public void OnCollisionExit(Collision other)
        {
            if (BallPoolGameLogic.playMode == PlayMode.Replay || BallPoolGameLogic.controlFromNetwork)
            {
                return;
            }
            BallListener ball = other.collider.GetComponent<BallListener>();

            if (ball)
            {
                if (AightBallPoolGameLogic.isCueBall(id))
                {
                    //if collision detect but not move, value under E-05. ex)8.049959E-06
                    var hitchecksum = Mathf.Abs(ball.normalizedVelocity.x) + Mathf.Abs(ball.normalizedVelocity.z);
                    if (hitchecksum > GameConfig.MinContactThresholdVelocity)
                    {
                        OnHitBall(ball);
                    }
                }
                else
                {
                    OnHitBall(ball);
                }

            }
            else if (other.gameObject.layer == Appnori.Layer.NameToLayer(Appnori.Layer.GameType.Billiards, "Board"))
            {
                OnHitBoard();
                OnBoardHit?.Invoke(this, other.gameObject);
            }
        }

        public void OnHitBall(BallListener ball)
        {
            pocketId = -1;
            hitShapeId = ball.id;
            physicsManager.CallBallHitBall(this, ball, true);
        }

        public void OnHitBoard()
        {
            pocketId = -1;
            hitShapeId = -1;
            physicsManager.CallBallHitBoard(this, true);
        }

        void Awake()
        {
            radius = body.GetComponent<SphereCollider>().radius;
            pocketId = -1;
            hitShapeId = -2;
            inMove = physicsManager.inMove;

            //20230120 DEV
            body.drag = DummySettings.DragValue;

            if (LevelLoader.CurrentMatchInfo.gameType != GameType.PocketBilliards)
            {
                if (CaromGameLogic.isCueBall(id))
                    gameObject.layer = Appnori.Layer.NameToLayer(Appnori.Layer.GameType.Billiards, "CueBall");
                else
                    gameObject.layer = Appnori.Layer.NameToLayer(Appnori.Layer.GameType.Billiards, "Ball");
            }
        }

        void FixedUpdate()
        {
            if (!body.isKinematic && !body.IsSleeping() && physicsManager.inMove)
            {
                physicsManager.CallBallMove(id, body.position, body.velocity, body.angularVelocity);
            }
            if (inMove != physicsManager.inMove)
            {
                inMove = physicsManager.inMove;
                if (!inMove && !body.isKinematic)
                {
                    body.Sleep();
                }
            }
        }
    }
}
