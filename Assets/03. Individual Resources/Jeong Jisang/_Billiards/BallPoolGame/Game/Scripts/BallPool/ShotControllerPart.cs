using UnityEngine;
using System.Collections;
using BallPool.Mechanics;
using Billiards;
using UnityEditor;
using NetworkManagement;
using System;
using Appnori.Util;

namespace BallPool
{

    public partial class ShotController : MonoBehaviour
    {
        //
        private float TouchInputTime = 0f;

        [SerializeField]
        private AnimationCurve CameraMovementCurve;
        [SerializeField]
        private AnimationCurve CueSliderAnimationCurve;

        private const float CUE_LENGTH = 1.375f;

        [SerializeField]
        private Transform LeftHandAnchor;

        [SerializeField]
        private Transform TableSpace;

        [SerializeField]
        private PermittedSpaceController SpaceController;

        [SerializeField]
        private AnimationCurve ForceMultipler;

        private float ShotDeltaForce;

        private CoroutineWrapper PermittedSpaceMoveRoutine;
        private CoroutineWrapper WaitAndStartShotRoutine;

        private Vector3 FixedRightHandPosition;
        private bool AllowedSetCuePosition
        {
            get => BilliardsDataContainer.Instance.AllowedSetCuePositionState.Value;
            set => BilliardsDataContainer.Instance.AllowedSetCuePositionState.Value = value;
        }


        public static bool IsRightHanded { get; private set; } = true;

        public static Appnori.XR.XRControllerState MainHandController
        {
            get => IsRightHanded ? BilliardsDataContainer.Instance.XRRightControllerState : BilliardsDataContainer.Instance.XRLeftControllerState;
        }

        public static Appnori.XR.XRControllerState SubHandController
        {
            get => IsRightHanded ? BilliardsDataContainer.Instance.XRLeftControllerState : BilliardsDataContainer.Instance.XRRightControllerState;
        }



        public enum GameStateType
        {
            //moveable
            WaitingForOpponent = 0,
            MoveAroundTable,

            //Freeball
            SetBallPosition,

            //Rotate Cue
            SelectShotDirection,

            //Waiting for swing
            CameraFixAndWaitShot,

            Shot
        }

        //network
        public void OpponentStateFormNetwork(int gameState)
        {
            BilliardsDataContainer.Instance.OpponentGameState.Value = (GameStateType)gameState;
        }

        public void OpponentMainHandFormNetwork(bool isRightHanded)
        {
            BilliardsDataContainer.Instance.OpponentMainHanded.Value = isRightHanded;
        }


        //왼손/오른손잡이 관련
        private void OnMainHandChanged(bool isRightHanded)
        {
            OnDisable();
            IsRightHanded = isRightHanded;
            OnEnable();
        }

        //State 관련

        /// <summary>
        /// state가 변경될때 처리할 작업 목록
        /// </summary>
        /// <param name="nextState"></param>
        public void SetState(GameStateType nextState)
        {
            Debug.Log($"[input] '{currentGameState}' -> '{nextState}' ");

            //state Exit
            switch (currentGameState)
            {
                case GameStateType.WaitingForOpponent:
                    HandLineManager.Instance.MainHand.RequestShow(false, this);
                    break;

                case GameStateType.SetBallPosition:
                    freeBallChecker.gameObject.SetActive(false);
                    HandLineManager.Instance.MainHand.RequestShow(false, this);

                    TryCalculateShot(true);
                    //SetToBestTarget();
                    //SetCueCameraPosition();

                    //BilliardsDataContainer.Instance.isFreeBallSubmited.OnDataChanged -= IsFreeBallSubmited_OnDataChanged;
                    if (hand.gameObject.activeInHierarchy)
                    {
                        hand.gameObject.SetActive(false);
                    }

                    ballInHand = false;

                    break;
                case GameStateType.Shot:
                    cuePivotLocalRotationYAdd = 0;
                    break;

                default:
                    break;
            }

            //state Enter
            switch (nextState)
            {
                case GameStateType.WaitingForOpponent:
                {
                    BilliardsDataContainer.Instance.OpponentGameState.Value = GameStateType.MoveAroundTable;
                    //.outposition

                    MoveOutOfTable(() => SpaceController.enabled = true);

                    HandLineManager.Instance.MainHand.RequestShow(true, this);
                    break;
                }

                //reset Cue
                case GameStateType.MoveAroundTable:
                {
                    cuePivot.localEulerAngles = new Vector3(0, cuePivot.localEulerAngles.y, cuePivot.localEulerAngles.z);

                    //reset CueVertical Rotation
                    cueVertical.localRotation = cueVerticalStartRotation;

                    //reset CueSliderPosition
                    cueSliderDisplacementZ = 0f;
                    cueSlider.localPosition = new Vector3(0.0f, 0.0f, cueSliderDisplacementZ);
                    cueSlider.localEulerAngles = Vector3.zero;

                    //SetSubCameraPosition();
                    PermittedSpaceMoveRoutine.Stop();

                    SpaceController.enabled = false;

                    if (currentGameState == GameStateType.WaitingForOpponent)
                    {
                        BilliardsDataContainer.Instance.TurnCount.Value += 1;
                        Debug.Log("BilliardsDataContainer.Instance.TurnCount.CurrentData added : " + BilliardsDataContainer.Instance.TurnCount.Value);
                    }
                    break;
                }

                case GameStateType.SetBallPosition:
                {
                    HandLineManager.Instance.MainHand.RequestShow(true, this);
                    ReleaseCalculateShot();
                    //SetCueCameraPosition();

                    //BilliardsDataContainer.Instance.isFreeBallSubmited.CurrentData = false;
                    //BilliardsDataContainer.Instance.isFreeBallSubmited.OnDataChanged += IsFreeBallSubmited_OnDataChanged;

                    hand.gameObject.SetActive(true);

                    ballInHand = true;
                    //freeBallChecker.gameObject.SetActive(true);
                    OnSelectBall?.Invoke();
                    break;
                }

                case GameStateType.SelectShotDirection:
                {
                    var bridgeOn = BilliardsDataContainer.Instance.CueSnapState.Value;

                    LeftHandAnchor.GetChild(0).gameObject.SetActive(bridgeOn);
                    LeftHandAnchor.GetChild(1).gameObject.SetActive(!bridgeOn);

                    if (currentGameState != GameStateType.CameraFixAndWaitShot)
                    {
                        //First Setup
                        //.Calculate Ai

                        var diff = BilliardsDataContainer.Instance.StandardCueBallCameraSlot.Value.position - cuePivot.position;
                        if (SetToBestTarget(diff.magnitude, out var position))
                        {
                            MoveToPosition(position, cueBall.position);
                        }

                    }

                    AllowedSetCuePosition = true;

                    //reset Slider
                    cueSliderDisplacementZ = 0f;
                    cueSlider.localPosition = new Vector3(0.0f, 0.0f, cueSliderDisplacementZ);
                    cueSlider.localEulerAngles = Vector3.zero;

                    cueVerticalLocalRotationXAdd = 0;
                    cuePivotLocalRotationYAdd = 0;

                    force = Mathf.Clamp01(-cueSliderDisplacementZ / cueSlidingMaxDisplacement);
                    SetCueTargetingPosition(Vector3.zero);
                    break;
                }

                case GameStateType.CameraFixAndWaitShot:
                {
                    //.set step cueposition
                    AllowedSetCuePosition = false;

                    FixedRightHandPosition = MainHandController.Position.Value;
                    //SetSubCameraPosition();
                    //SetTableCameraPosition();

                    //CueSlider setup
                    MaxCueSlideZDisplacement = 0f;
                    //cueVerticalLocalRotationX = cueVertical.localRotation.eulerAngles.x;

                    break;
                }

                case GameStateType.Shot:
                {
                    if (!inShot && force > 0.03f)
                    {
                        //adapt force
                        force -= 0.2f;
                        force *= 1.25f;
                        force = ForceMultipler.Evaluate(force);

                        //20230120 DEV
                        force *= DummySettings.ForceValue;

                        BilliardsDataContainer.Instance.CueSnapState.Value = false;

                        inShot = true;
                        inMove = true;
                        var impulse = new Impulse(shotPoint, force * (maxVelocity * physicsManager.ballMass) * cueSlider.forward);
                        physicsManager.SetImpulse(impulse);
                        Debug.Log("in shot : impulse : " + impulse);
                        //StartCoroutine("WaitAndStartShot");
                        WaitAndStartShotRoutine.StartSingleton(WaitAndStartShot());
                        Debug.Log("Force : " + force);

                        TrySendHaptic(MainHandController.Controller.inputDevice, force*GameDataManager.instance.userInfo_mine.valueHaptic, 0.1f); // 손에 진동 주는 코드 

                        static bool TrySendHaptic(in UnityEngine.XR.InputDevice device, float amplitude, float duration)
                        {
                            if (!device.TryGetHapticCapabilities(out var capabilities))
                                return false;

                            if (!capabilities.supportsImpulse)
                                return false;

                            device.SendHapticImpulse(0u, Mathf.Clamp01(amplitude), duration);

                            return true;
                        }
                    }
                    break;
                }

                default:
                    break;
            }

            currentGameState = nextState;
        }

        private void SubControllerTrigger_OnDataChanged(bool isDown)
        {
            switch (currentGameState)
            {
                case GameStateType.SelectShotDirection:
                case GameStateType.CameraFixAndWaitShot:
                    AllowedSetCuePosition = true;
                    FixedRightHandPosition = MainHandController.Position.Value;
                    break;

            }

        }

        private void ControllerTrigger_OnDataChanged(bool isDown)
        {
            Debug.Log("[input]Trigger Changed : " + isDown);

            if (!BallPoolPlayer.mainPlayer.myTurn)
                return;

            switch (currentGameState)
            {
                //special case
                case GameStateType.SetBallPosition:
                    if (isDown)
                    {
                        UnselectBall();
                        SetState(GameStateType.SelectShotDirection);
                    }
                    break;

                case GameStateType.SelectShotDirection:
                    if (isDown)
                    {
                        SetState(GameStateType.CameraFixAndWaitShot);
                    }
                    break;

                case GameStateType.CameraFixAndWaitShot:
                    if (!isDown)
                    {
                        SetState(GameStateType.SelectShotDirection);
                    }
                    break;

                default:
                    break;
            }
        }

        private void ControllerTrackButton_OnDataChanged(bool isDown)
        {
            Debug.Log("[input]Track Changed : " + isDown);
            switch (currentGameState)
            {
                //case GameStateType.WaitingForOpponent:
                //    if (isDown) BilliardsDataContainer.Instance.TableBackgroundHider.CurrentData.ShowBlack();
                //    else BilliardsDataContainer.Instance.TableBackgroundHider.CurrentData.HideBlack();
                //    break;

                //case GameStateType.SetBallPosition:
                //    if (isDown) BilliardsDataContainer.Instance.TableBackgroundHider.CurrentData.ShowBlack();
                //    else BilliardsDataContainer.Instance.TableBackgroundHider.CurrentData.HideBlack();
                //    break;

                case GameStateType.SelectShotDirection:
                    if (isDown)
                    {
                        TouchInputTime = Time.time;
                        BilliardsDataContainer.Instance.TableBackgroundHider.Value.ShowBlack();

                        cueSlider.localPosition = new Vector3(0.0f, 0.0f, cueSliderDisplacementZ);
                    }
                    else
                    {
                        TouchInputTime = -1f;
                        BilliardsDataContainer.Instance.TableBackgroundHider.Value.HideBlack();
                    }
                    break;

                default:
                    if (!isDown)
                    {
                        TouchInputTime = -1f;
                        BilliardsDataContainer.Instance.TableBackgroundHider.Value.HideBlack();
                    }
                    break;

            }
        }

        void Update()
        {

#if UNITY_EDITOR
            //if (Input.GetKeyDown(KeyCode.I))
            //{
            //    SetState(GameStateType.MoveAroundTable);
            //}

            //if (Input.GetKey(KeyCode.A))
            //{
            //    Time.timeScale = 1f;
            //}
            //else
            //{
            //    Time.timeScale = 10f;
            //}

            if (Input.GetKeyDown(KeyCode.Space) /*|| (currentGameState == GameStateType.SetBallPosition || currentGameState == GameStateType.SelectShotDirection)*/)
            {
                force = 1.0f;

                if (currentGameState == GameStateType.SetBallPosition)
                {
                    UnselectBall();
                    SetState(GameStateType.SelectShotDirection);
                }
                else
                {
                    SetState(GameStateType.Shot);

                }
            }
#endif

            if (Appnori.XR.InputDeviceInfo.Instance.Allow2DAxisWithoutClick)
            {
                Appnori.XR.InputDeviceInfo.Instance.Primary2DAxisInput.Value = MainHandController.Primary2DAxisNotifier.Value.magnitude > 0.01f;
            }

            switch (currentGameState)
            {
                case GameStateType.WaitingForOpponent:
                {
                    if (BallPoolPlayer.mainPlayer.myTurn)
                    {
                        SetState(GameStateType.MoveAroundTable);
                    }

                    //RotateTableCameraCenter();
                    break;
                }

                case GameStateType.MoveAroundTable:
                {
                    if (inMove)
                        break;

                    if (!BallPoolPlayer.mainPlayer.myTurn)
                    {
                        Debug.LogError("Player's turn is unclear. MyTurn is false but state is MoveAroundTable");
                        SetState(GameStateType.WaitingForOpponent);
                    }
                    else if (AightBallPoolGameLogic.gameState.cueBallInHand && !AightBallPoolGameLogic.gameState.cueBallMoved)
                    {
                        SetState(GameStateType.SetBallPosition);
                    }
                    else
                    {
                        SetState(GameStateType.SelectShotDirection);
                    }

                    break;
                }

                case GameStateType.SetBallPosition:
                {
                    if (ballInHand)
                    {
                        TryingToMoveBall();
                    }
                    break;
                }

                case GameStateType.SelectShotDirection:
                {
                    RotateAroundCueball();
                    TryCalculateShot(false);

                    SetCuePosition();

                    if (BilliardsDataContainer.Instance.CueSnapState.Value)
                    {
                        var zRate = Mathf.Clamp(Mathf.Abs(LeftHandAnchor.localPosition.z), 0.5f, 1f);
                        LeftHandAnchor.localEulerAngles = new Vector3(Mathf.Lerp(-0.168f, 20f, (zRate - 0.5f) * 2), 0, 0);
                    }
                    break;
                }

                case GameStateType.CameraFixAndWaitShot:
                {
                    SetCueHitPosition(true);

                    //set Hand Anchor angle;
                    var zRate = Mathf.Clamp(Mathf.Abs(LeftHandAnchor.localPosition.z), 0.5f, 1f);
                    LeftHandAnchor.localEulerAngles = new Vector3(Mathf.Lerp(-0.168f, 20f, (zRate - 0.5f) * 2), 0, 0);

                    var controllerVelocity = MainHandController.VelocityNotifier.Value;

                    var distance = (cueBall.position - cuePivot.position).magnitude;

                    var projectedLocalForward = cueSlider.InverseTransformVector(Vector3.Project(controllerVelocity, cueSlider.forward));

                    //position

                    ShotDeltaForce = projectedLocalForward.z * Time.deltaTime;

                    cueSliderDisplacementZ += ShotDeltaForce;
                    cueSliderDisplacementZ = Mathf.Clamp(cueSliderDisplacementZ, -cueSlidingMaxDisplacement, distance);
                    cueSlider.localPosition = new Vector3(0.0f, 0.0f, cueSliderDisplacementZ);

                    if (MaxCueSlideZDisplacement <= Mathf.Abs(cueSliderDisplacementZ))
                    {
                        MaxCueSlideZDisplacement = Mathf.Abs(cueSliderDisplacementZ) + distance;
                    }


                    TryCalculateShot(false);

                    if (cueSliderDisplacementZ > distance - 0.05f && (MaxCueSlideZDisplacement / cueSlidingMaxDisplacement) > 0.1f)
                    {
                        //var velocity = (MaxCueSlideZDisplacement / cueSlidingMaxDisplacement);

                        force = ShotDeltaForce.Remap(0, 0.06f, 0.1f, 1f);

                        if (force > 0.03f)
                        {
                            //raycast
                            if (!Physics.Raycast(cueDisplacement.position, cueDisplacement.forward, out var cueballHit, 1.2f * clothSpace.lossyScale.x, cueBallLayer))
                            {
                                //fail To cast
                                Debug.Log("Break");
                                break;
                            }

                            //carom
                            if (LevelLoader.CurrentMatchInfo.gameType != GameType.PocketBilliards && cueballHit.rigidbody.TryGetComponent<BallListener>(out var ball))
                            {
                                if (CaromGameLogic.isCueBall(ball.id) == false)
                                    break;
                            }

                            shotPoint = cueballHit.point;

                            SetState(GameStateType.Shot);
                        }
                    }

                    break;
                }

                case GameStateType.Shot:
                {
                    if (AightBallPoolGameLogic.gameState.hasRightBallInPocket || !inMove)
                    {
                        SetState(GameStateType.MoveAroundTable);
                    }
                    break;
                }

                default:
                    break;
            }

            //CameraMovement();
        }

        [Obsolete("양손 모두 락이 있을 때 사용하던 코드")]
        private void RotateAroundPivot(LockMarker Target, float projectedLocalHorizontal, float projectedLocalVertical)
        {
            Target.InitializeAxis(cuePivot.position);

            var defaultEuler = cuePivot.eulerAngles;

            cuePivot.SetParent(Target.SimulationRoot, true);

            var x = projectedLocalVertical * 0.5f * GameConfig.ControllerVelocityRate;
            var y = -projectedLocalHorizontal * 0.5f * GameConfig.ControllerVelocityRate;

            Target.UpdateSimulationOffset(x, y);

            cuePivot.SetParent(null, true);

            var afterEuler = cuePivot.eulerAngles;
            var diff = afterEuler - defaultEuler;

            cuePivot.localEulerAngles = new Vector3(0, defaultEuler.y + diff.y, defaultEuler.z + diff.z);
            cueVertical.localEulerAngles = new Vector3(cueVertical.localEulerAngles.x + diff.x, cueVertical.localEulerAngles.y, cueVertical.localEulerAngles.z);
        }

        /// <summary>
        /// RightController Trackpad Control.
        /// </summary>
        private void RotateAroundCueball()
        {
            if (!Appnori.XR.InputDeviceInfo.Instance.isInitialized)
                Appnori.XR.InputDeviceInfo.Instance.Initialize();

            var isPressed = MainHandController[UnityEngine.XR.Interaction.Toolkit.InputHelpers.Button.Primary2DAxisClick].Value || Appnori.XR.InputDeviceInfo.Instance.Allow2DAxisWithoutClick;
            if (isPressed)
            {
                var xrRig = BilliardsDataContainer.Instance.XRRigid.Value;
                if (xrRig == null)
                    return;

                var cameraTransform = BilliardsDataContainer.Instance.MainCamera.Value.transform;
                var axisNotifierData = MainHandController.Primary2DAxisNotifier.Value;
                var angle = Mathf.Abs(Vector2.SignedAngle(Vector2.up, axisNotifierData.normalized));

                //y axis or x axis move
                Vector2 allowedAxis = angle < 45f || angle > 135f ? Vector2.up : Vector2.right;
                if (axisNotifierData.magnitude < GameConfig.CueMoveThreshold)
                    allowedAxis = Vector2.zero;

                axisNotifierData = axisNotifierData.Decrease(GameConfig.CueMoveThreshold) * 1f / (1f - GameConfig.CueMoveThreshold);

                //rotateSpeed
                var decreaseTime = GameConfig.CuePivotRotateSpeedDecreaseTime;
                var decreaseSpeedRate = Mathf.Pow(Mathf.Clamp01((Time.time - TouchInputTime) / decreaseTime), 2);
                var xAxisRotateSpeed = -axisNotifierData.x * GameConfig.TableCenterRotateSpeed * decreaseSpeedRate * allowedAxis.x;

                //move forward,back
                var yAxisMovementSpeed = axisNotifierData.y * GameConfig.CueDistanceMoveSpeed * decreaseSpeedRate * 0.5f * allowedAxis.y;

                //trackpad position => up is positive . so if sign of distance is positive, it gets closer
                var cueBallDistance = (cueBall.position - cameraTransform.position).magnitude;
                if (cueBallDistance < GameConfig.CueDistanceMin && Mathf.Sign(yAxisMovementSpeed) > 0) yAxisMovementSpeed = 0f;
                if (cueBallDistance > GameConfig.CueDistanceMax && Mathf.Sign(yAxisMovementSpeed) < 0) yAxisMovementSpeed = 0f;

                //rotation / position
                var beginEuler = Quaternion.LookRotation(cueBall.position - cameraTransform.position).eulerAngles;
                var beginEulerDiff = xrRig.Camera.transform.eulerAngles - beginEuler;

                var afterPosition = cueBall.position + Quaternion.Euler(beginEuler + Vector3.up * xAxisRotateSpeed) * Vector3.back * (cueBallDistance - yAxisMovementSpeed);
                var afterEuler = Quaternion.LookRotation(cueBall.position - afterPosition).eulerAngles;

                //set
                xrRig.RotateAroundCameraUsingOriginUp(afterEuler.y - xrRig.Camera.transform.eulerAngles.y + beginEulerDiff.y);
                xrRig.MoveCameraToWorldLocation(afterPosition.ToXZ().ToVector3FromXZ(xrRig.Camera.transform.position.y));
            }
        }

        //for last value save in SetCuePosition
        private Vector3 MainControllerTracker;

        /// <summary>
        /// <para> 큐 이동에 대한 전반적 로직. 자유이동, 스냅, 스핀, 브릿지(왼손) 이동까지 포함됨 </para>
        /// 
        /// 스냅은 트래커를 사용하지 않고 계산.
        /// 스냅 이후 각도 계산시에만 트래커를 사용하여 1차적 조준 완화
        /// </summary>
        private void SetCuePosition()
        {
            const float distance = CUE_LENGTH * 0.7f;

            MainControllerTracker = Vector3.Lerp(MainControllerTracker, MainHandController.Position.Value, GameConfig.MainControllerTrackingRate);

            BilliardsDataContainer.Instance.CueSnapState.OnDataChangedOnce += onCueSnapStateChanged;
            BilliardsDataContainer.Instance.CueSnapState.Value = GetControllerDirectionsAreSnap(out var d1, out var d2);
            BilliardsDataContainer.Instance.CueSnapState.OnDataChangedOnce -= onCueSnapStateChanged;

            var currentSnapState = BilliardsDataContainer.Instance.CueSnapState.Value;


            SetCuePivotPosition(currentSnapState, out var dir);

            SetLeftHandPosition();

            SetCueRotation(currentSnapState);

            return;

            //local functions
            void onCueSnapStateChanged(bool isSnap)
            {
                //reset targetingPosition
                if (isSnap)
                {
                    SetCueTargetingPosition(Vector2.zero);
                }

                LeftHandAnchor.GetChild(0).gameObject.SetActive(isSnap);
                LeftHandAnchor.GetChild(1).gameObject.SetActive(!isSnap);
            };

            void SetCuePivotPosition(in bool isSnap, out Vector3 lookDir)
            {
                if (isSnap)
                {
                    lookDir = cueBall.position - MainControllerTracker;
                    cuePivot.position = cueBall.position - ((lookDir * 100).normalized * 0.1f);

                    if (cuePivot.position.y < cueBallRadius + GameConfig.MinimumCuePositionHeight)
                        cuePivot.position = cuePivot.position.ToXZ().ToVector3FromXZ(cueBallRadius + GameConfig.MinimumCuePositionHeight);

                    //spin
                    SetCueHitPosition(true);
                }
                else
                {
                    lookDir = SubHandController.Position.Value - MainHandController.Position.Value;
                    cuePivot.position = MainHandController.Position.Value + (lookDir.normalized * distance);
                    FixedRightHandPosition = MainHandController.Position.Value;
                }
            }

            void SetLeftHandPosition()
            {
                LeftHandAnchor.position = SubHandController.Position.Value;
                LeftHandAnchor.localPosition = new Vector3(
                    GameConfig.PlayerLeftHandDefaultLocalPosition.x,
                    GameConfig.PlayerLeftHandDefaultLocalPosition.y,
                    Mathf.Clamp(LeftHandAnchor.localPosition.z, -0.75f, -0.2f));

            }

            void SetCueRotation(in bool isSnap)
            {
                if (isSnap)
                {
                    var rotation = Quaternion.LookRotation(dir, Vector3.up);
                    var x = rotation.eulerAngles.x > 90 ? GameConfig.MinimumCueAngle : Mathf.Clamp(rotation.eulerAngles.x, GameConfig.MinimumCueAngle, 90);
                    cueVertical.localRotation = Quaternion.Euler(x, 0, 0);
                    cuePivot.localRotation = Quaternion.Euler(0.0f, rotation.eulerAngles.y, 0.0f);

                }
                else
                {
                    var rotation = Quaternion.LookRotation(dir, Vector3.up);
                    cueVertical.localRotation = Quaternion.Euler(rotation.eulerAngles.x, 0, 0);
                    cuePivot.localRotation = Quaternion.Euler(0.0f, rotation.eulerAngles.y, 0.0f);
                }
            }
        }

        private bool GetControllerDirectionsAreSnap(out Vector3 d1, out Vector3 d2, in Vector3 overridiedRightPosition = default)
        {
            if (overridiedRightPosition != default)
            {
                d1 = SubHandController.Position.Value - overridiedRightPosition;
                d2 = cueBall.position - overridiedRightPosition;

                return Vector3.Angle(d1, d2) < GameConfig.SubControllerSnapAngle;
            }

            d1 = SubHandController.Position.Value - MainHandController.Position.Value;
            d2 = cueBall.position - MainHandController.Position.Value;

            return Vector3.Angle(d1, d2) < GameConfig.SubControllerSnapAngle;
        }

        private void SetCueHitPosition(in bool useFixedPosition = false)
        {
            if (useFixedPosition && !AllowedSetCuePosition)
                return;

            if (SubHandController[UnityEngine.XR.Interaction.Toolkit.InputHelpers.Button.Trigger].Value == false)
                return;

            GetControllerDirectionsAreSnap(out var d1, out var d2, useFixedPosition ? FixedRightHandPosition : default);

            var projectedLocalD1 = cueSlider.InverseTransformVector(d1).normalized;
            var projectedLocalD2 = cueSlider.InverseTransformVector(d2).normalized;

            SetCueTargetingPosition((projectedLocalD1 - projectedLocalD2).ToVector2() * 4.8f);
        }

        private void MoveOutOfTable(Action onComplete)
        {
            var playerPosition = BilliardsDataContainer.Instance.XRRigid.Value.Camera.transform.position;

            if (!Geometry.SphereInCube(playerPosition, -0.15f, TableSpace))
            {
                onComplete?.Invoke();
                return;
            }

            PermittedSpaceMoveRoutine.StartSingleton(DelayMove()).SetOnComplete(onComplete);

            IEnumerator DelayMove()
            {
                yield return new WaitWhile(isInMove);
                bool isInMove() => inMove;

                var cueBallPosition = cueBall.position;
                var outPos = Geometry.EdgeProjectionXZ((playerPosition.ToXZ() - cueBallPosition.ToXZ()) * 100, Vector3.Lerp(playerPosition, cueBallPosition, 0.5f).ToXZ(), TableSpace);

                MoveToPosition(outPos.ToVector3FromXZ() * 1.25f, cueBall.position);
            }

        }

        private void MoveToPosition(in Vector3 position, in Vector3 lookPosition)
        {
            var xrRig = BilliardsDataContainer.Instance.XRRigid.Value;

            //move
            xrRig.MoveCameraToWorldLocation(position.ToXZ().ToVector3FromXZ(xrRig.Camera.transform.position.y));

            //rotate
            var targetEulerAngles = Quaternion.LookRotation(lookPosition - xrRig.Camera.transform.position).eulerAngles;
            xrRig.RotateAroundCameraUsingOriginUp(targetEulerAngles.y - xrRig.Camera.transform.eulerAngles.y);
        }

        private bool SetToBestTarget(float distance, out Vector3 position)
        {
            switch (LevelLoader.CurrentMatchInfo.gameType)
            {
                case GameType.PocketBilliards:
                    return SetToBestTargetOfPocket(distance, out position);
                default:
                    return SetToBestTargetOfCarom(distance, out position);
            }
        }

        private bool SetToBestTargetOfCarom(in float distance, out Vector3 position)
        {
            position = Vector3.zero;

            position = cueBall.position - (gameManager.balls[2].position - cueBall.position);
            return true;


            var info = aiManager.GetCalculateInfo(cueBall, physicsManager.ballMaxVelocity);
            if (info.targetBall == null)
                return false;

            return true;
        }

        private bool SetToBestTargetOfPocket(in float distance, out Vector3 position)
        {

            position = Vector3.zero;

            var info = aiManager.GetCalculateInfo(cueBall, physicsManager.ballMaxVelocity);
            if (info.targetBall == null)
                return false;

            Vector3 impulseVector = info.impulse * (info.targetBall.position - info.shotBallPosition).normalized;

            var pos = info.shotBallPosition.ToXZ().ToVector3FromXZ(cueBall.position.y);
            var dir = Vector3.ProjectOnPlane(impulseVector.normalized, Vector3.up);

            position = pos + (-dir * distance);
            return true;
        }


        //AI
        private Coroutine AIAnimationRoutine = null;
        private IEnumerator AIStartAnimation(Action onComplete)
        {
            yield return StartCoroutine(AICueBallAnimation(runtime: 1));

            yield return StartCoroutine(AICueRotateAnimation(runtime: 0.8f));

            yield return StartCoroutine(AICueSliderAnimation(runtime: 2.2f));

            onComplete.Invoke();
        }

        private IEnumerator AICueBallAnimation(float runtime = 1f)
        {
            //first
            var defaultPosition = cueBall.position;
            var targetPosition = aiManager.info.shotBallPosition;

            if (defaultPosition != targetPosition)
            {
                float t = 0;
                while (t < runtime)
                {
                    cueBall.position = Vector3.Lerp(defaultPosition, targetPosition, t / runtime);
                    t += Time.deltaTime;
                    cueBall.OnState(BallState.SetState);
                    yield return null;
                }
            }

            cueBall.position = aiManager.info.shotBallPosition;
            cueBall.OnState(BallState.SetState);
        }

        private IEnumerator AICueRotateAnimation(float runtime = 1f)
        {
            //second
            ballChecker.position = aiManager.info.aimpoint;
            ballChecker.gameObject.SetActive(true);

            shotPoint = aiManager.info.shotPoint;

            cuePivot.position = aiManager.info.shotBallPosition;
            cueDisplacement.position = shotPoint;
            cueDisplacement.localPosition += new Vector3(0, 0, -0.09f);

            Vector3 impulseVector = aiManager.info.impulse * (aiManager.info.aimpoint - aiManager.info.shotBallPosition).normalized;
            Vector3 impulseVectorCustom = Vector3.ProjectOnPlane(impulseVector.normalized, Vector3.up) + new Vector3(0, -0.0875f, 0);

            var defaultPivotRotation = cuePivot.rotation;
            var targetPivotRotation = Quaternion.LookRotation(impulseVectorCustom);

            float t = 0;
            while (t < runtime)
            {
                cuePivot.rotation = Quaternion.Lerp(defaultPivotRotation, targetPivotRotation, t / runtime);
                TryCalculateShot(true);

                t += Time.deltaTime;
                yield return null;
            }

            cuePivot.LookAt(cuePivot.position + impulseVectorCustom);
            cueSlider.forward = impulseVectorCustom;

            TryCalculateShot(true);
            yield break;
        }

        private IEnumerator AICueSliderAnimation(float runtime = 3f)
        {
            //third
            float displacement = (aiManager.info.impulse / physicsManager.ballMaxVelocity) * cueSlidingMaxDisplacement;

            var defaultSliderDisZ = cueSliderDisplacementZ;
            var targetSliderDisZ = -displacement;

            var defaultSliderLocalPosition = cueSlider.localPosition;
            var targetSliderLocalPosition = new Vector3(0.0f, 0.0f, -displacement);

            float t = 0;
            while (t < runtime)
            {
                cueSliderDisplacementZ = Mathf.Lerp(defaultSliderDisZ, targetSliderDisZ, CueSliderAnimationCurve.Evaluate(t / runtime));
                cueSlider.localPosition = Vector3.Lerp(defaultSliderLocalPosition, targetSliderLocalPosition, CueSliderAnimationCurve.Evaluate(t / runtime));

                t += Time.deltaTime;
                yield return null;
            }

            cueSliderDisplacementZ = -displacement;
            cueSlider.localPosition = new Vector3(0.0f, 0.0f, -displacement);

            ResetChanged();
            TryCalculateShot(true);
            yield break;
        }

    }

}