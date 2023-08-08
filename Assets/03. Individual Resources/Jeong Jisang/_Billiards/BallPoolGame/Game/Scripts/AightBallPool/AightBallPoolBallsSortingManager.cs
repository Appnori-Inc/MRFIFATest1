using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BallPool.Mechanics;
using System;
using Meta.Numerics.Analysis;
using Appnori.Util;
using Billiards;

namespace BallPool
{
    public class AightBallPoolBallsSortingManager : MonoBehaviour, BallPoolBallsSortingManager
    {
        public static AightBallPoolBallsSortingManager Sorter = null;

        [SerializeField] private Transform balls;
        [SerializeField] private Transform BallsListener;
        [SerializeField] private PhysicsManager physicsManager;
        [SerializeField] private float ballsDistance;
        [SerializeField] Transform cueBallPosition;
        [SerializeField] Transform pyramidFirstBallPosition;
        [SerializeField] private GameManager gameManager;

        private void Awake()
        {
            Sorter = this;
        }

        public void SortBalls()
        {
            Debug.Log("Balls sorted by AightBallPoolBallsSortingManager");
            Vector2[] delta = { 
                new Vector2(0.0f, 0.0f),//0
                new Vector2(4.0f, 4.0f),//15
                new Vector2(1.0f, -1.0f),//2
                new Vector2(2.0f, 2.0f),//9
                new Vector2(3.0f, -3.0f),//10
                new Vector2(3.0f, 1.0f),//8
                new Vector2(4.0f, -4.0f),//3
                new Vector2(4.0f, 0.0f),//4
                new Vector2(2.0f, 0.0f),//11
                new Vector2(1.0f, 1.0f),//5
                new Vector2(2.0f, -2.0f),//12
                new Vector2(3.0f, -1.0f),//6
                new Vector2(3.0f, 3.0f),//13
                new Vector2(4.0f, -2.0f),//7
                new Vector2(4.0f, 2.0f),//14
                new Vector2(0.0f, 0.0f)};//1

            gameManager.balls = new Ball[balls.childCount];
            physicsManager.ballsListener = new BallListener[balls.childCount];

            for (int i = 0; i < balls.childCount; i++)
            {
                Ball ball = balls.GetChild(i).GetComponent<Ball>();
                BallListener listener = BallsListener.GetChild(i).GetComponent<BallListener>();
                listener.body = listener.GetComponent<Rigidbody>();
                ball.listener = listener;
                float distance = listener.GetComponent<SphereCollider>().radius + ballsDistance;
                Vector3 position = cueBallPosition.position;
                if (i != 0)
                {
                    position = pyramidFirstBallPosition.position + new Vector3(delta[i].x * Mathf.Sqrt(Mathf.Pow(2.0f * distance, 2.0f) - Mathf.Pow(distance, 2.0f)), 0.0f, delta[i].y * distance);
                }
                ball.id = i;
                ball.transform.position = position;
                listener.transform.position = position;
                listener.id = ball.id;
                listener.physicsManager = physicsManager;
                ball.name = listener.name = "Ball_" + i;

                gameManager.balls[i] = ball;
                physicsManager.ballsListener[i] = listener;
            }
        }

        public void CaromResetBall()
        {
            bool isDuplicated;
            do
            {
                isDuplicated = false;

                Generate(BallsListener.childCount, out var positions);

                var tableSize = LevelLoader.CurrentMatchInfo.gameType switch
                {
                    GameType.CaromThree => 1.422f - (0.0615f * 2f),
                    GameType.CaromFour => 1.224f - (0.0655f * 2f),
                    _ => throw new NotImplementedException()
                };

                for (int i = 0; i < BallsListener.childCount; ++i)
                {
                    var pos = positions[i];
                    var ballListener = BallsListener.GetChild(i);
                    var ball = balls.GetChild(i);

                    var p = new Vector2(pos.x.Remap((0, 1), (-tableSize, tableSize)), pos.y.Remap((0, 1), (-tableSize * 0.5f, tableSize * 0.5f)));

                    if (!TestPosition(ball, p.ToVector3FromXZ(), out var newPosition))
                        isDuplicated = true;

                    ballListener.transform.position = newPosition;
                    ball.transform.position = newPosition;
                }

            } while (isDuplicated);
        }

        public bool TestPosition(in Transform ball, in Vector3 position, out Vector3 newPosition)
        {
            newPosition = position;
            Vector3 origin = position + Vector3.up * 0.5f;
            Vector3 direction = Vector3.down;

            var ballRadius = ball.transform.localScale.x * 0.5f;
            var clothLayer = 1 << Appnori.Layer.NameToLayer(Appnori.Layer.GameType.Billiards, "Cloth");
            var ballLayer = 1 << Appnori.Layer.NameToLayer(Appnori.Layer.GameType.Billiards, "Ball");
            var cueBallLayer = 1 << Appnori.Layer.NameToLayer(Appnori.Layer.GameType.Billiards, "CueBall");

            //permitted Space Check
            if (!Physics.Raycast(origin, direction, out var clothHit, 1f, clothLayer))
                return false;

            //duplicated Ball Position Check
            if (Physics.SphereCast(origin, ballRadius, direction, out var ballHit, 1f, ballLayer | cueBallLayer))
                return false;

            newPosition = clothHit.point + ballRadius * clothHit.normal;
            return true;
        }

        public void Generate(in int count, out List<Vector2> values)
        {
            values = new List<Vector2>();

            var d = 2;
            var seq = new SobolSequence[d];
            for (int i = 0; i < d; ++i)
            {
                var p = SobolSequenceParameters.sobolParameters[i];
                seq[i] = new SobolSequence(p.Dimension, p.Coefficients, p.Seeds);
            }

            var skip = UnityEngine.Random.Range(50, 120);
            for (int j = 0; j < skip; ++j)
            {
                seq[0].MoveNext();
                seq[1].MoveNext();
            }

            var innerSkip = UnityEngine.Random.Range(3, 8);
            for (int i = 0; i < count; ++i)
            {
                values.Add(new Vector2((float)seq[0].Current, (float)seq[1].Current));
                for (int j = 0; j < innerSkip; ++j)
                {
                    seq[0].MoveNext();
                    seq[1].MoveNext();
                }
            }
        }

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.G))
            {
                CaromResetBall();
            }
        }
    }
}
