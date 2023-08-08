using UnityEngine;
using System.Collections;

namespace Billiards
{
    using BallPool;
    using System.Collections.Generic;

    public class DummyPlayerHead : MonoBehaviour
    {
        [SerializeField]
        private PlayerManager manager;
        [SerializeField]
        private GameObject Tracker;

        [SerializeField]
        private Transform LHandTracker;

        [SerializeField]
        private Transform RHandTracker;

        [SerializeField]
        private UnityEngine.Animations.RotationConstraint HandRotationConstraint;


        private const string AI_UUID = "AI";

        private Vector3 LastWorldPosition;
        private Quaternion LastRotation;

        private Vector3 defaultLocalPosition;

        private void Awake()
        {
            defaultLocalPosition = new Vector3(-0.22f, -0.14f, -0.1f);
            if (AightBallPoolGameLogic.playMode != PlayMode.PlayerAI)
            {
                Tracker.SetActive(false);
                return;
            }

            BilliardsDataContainer.Instance.GameState.OnDataChanged += GameState_OnDataChanged;
            PlayerManager.OnInitialized += PlayerManager_OnInitialized;

        }

        private void PlayerManager_OnInitialized(PlayerManager instance)
        {
            instance.OnPlayerInitialized += Instance_OnPlayerInitialized;
        }

        private void Instance_OnPlayerInitialized(PlayerInstance player)
        {
            if (!player.isMine && AightBallPoolGameLogic.playMode == PlayMode.PlayerAI)
            {
                HandRotationConstraint.SetSource(0, new UnityEngine.Animations.ConstraintSource() { weight = 1, sourceTransform = player.BodyRoot });
                PlayerManager.OnInitialized += removeListener;
            }

            void removeListener(PlayerManager instance)
            {
                instance.OnPlayerInitialized -= Instance_OnPlayerInitialized;
            }
        }

        private void Start()
        {
            GameState_OnDataChanged(BilliardsDataContainer.Instance.GameState.Value);
        }

        private void GameState_OnDataChanged(ShotController.GameStateType obj)
        {
            switch (obj)
            {
                case ShotController.GameStateType.WaitingForOpponent:
                    Tracker.transform.SetParent(BilliardsDataContainer.Instance.StandardCueBallCameraSlot.Value, false);
                    Tracker.transform.localPosition = defaultLocalPosition;
                    Tracker.transform.LookAt(BilliardsDataContainer.Instance.StandardCueBallCameraSlot.Value.parent);
                    break;

                default:
                    if (Tracker.transform.parent != null)
                    {
                        Tracker.transform.SetParent(null, false);
                        Tracker.transform.localPosition = GetRandomPosition();
                        Tracker.transform.LookAt(BilliardsDataContainer.Instance.TableCameraCenter.Value);
                    }
                    break;
            }

        }

        private void Update()
        {
            if (LastWorldPosition != Tracker.transform.position || LastRotation != Tracker.transform.rotation || true)
            {
                manager.SetPlayerTransform(AI_UUID,
                    new List<Vector3>() { Tracker.transform.position, LHandTracker.position, RHandTracker.position },
                    new List<Quaternion>() { Tracker.transform.rotation, LHandTracker.rotation, RHandTracker.rotation });

                LastWorldPosition = Tracker.transform.position;
                LastRotation = Tracker.transform.rotation;
            }

            //limit angle
            var dir = BilliardsDataContainer.Instance.CueBallCameraRootFollower.Value.transform.position - Tracker.transform.position;
            if (Mathf.Abs(dir.x + dir.z) < Mathf.Abs(dir.y))
            {
                dir = new Vector2(
                    dir.x / Mathf.Abs(dir.x + dir.z) * Mathf.Abs(dir.y),
                    dir.z / Mathf.Abs(dir.x + dir.z) * Mathf.Abs(dir.y)).ToVector3FromXZ(dir.y);
            }

            var rotation = Quaternion.LookRotation(dir.normalized);
            Tracker.transform.rotation = rotation;

            //Tracker.transform.LookAt(BilliardsDataContainer.Instance.CueBallCameraRootFollower.CurrentData.transform);
        }

        private void OnDestroy()
        {
            if (AightBallPoolGameLogic.playMode != PlayMode.PlayerAI)
                return;

            BilliardsDataContainer.Instance.GameState.OnDataChanged -= GameState_OnDataChanged;
        }

        private Vector3 GetRandomPosition()
        {
            var slotPos = BilliardsDataContainer.Instance.TableCameraSlot.Value.position;
            var randomAxis = Quaternion.AngleAxis(Random.Range(0, 360), Vector3.up) * Vector3.right;
            return (slotPos.ToXZ().magnitude * randomAxis).ToXZ().ToVector3FromXZ(slotPos.y - 0.14f);
        }
    }

}