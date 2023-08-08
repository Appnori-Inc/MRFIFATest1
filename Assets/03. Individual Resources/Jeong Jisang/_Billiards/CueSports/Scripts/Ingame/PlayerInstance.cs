using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BallPool;
using System.Linq;
using System;

namespace Billiards
{

    [Serializable]
    public class PlayerModel
    {
        public Transform Root;
        public Transform HeadRoot;
        public Transform LHandRoot;
        public Transform RHandRoot;
        public CustomModelSettingCtrl CustomSettings;
        //public List<MeshRenderer> renderers;
        //kpublic Color HandColor;

        //public void SetRenderer(Action<MeshRenderer> action)
        //{
        //    //using (var e = renderers.GetEnumerator())
        //    //    while (e.MoveNext()) action(e.Current);
        //}

        public void SettingsInit(CustomModelData _modelData)
        {
            CustomSettings.Init(_modelData);
        }
        public void SetActive(bool value)
        {
            Root.gameObject.SetActive(value);
            HeadRoot.gameObject.SetActive(value);
        }
    }



    public class PlayerInstance : MonoBehaviour
    {
        [SerializeField]
        private PlayerModel Model;

        [SerializeField]
        private AnimationCurve MoveCureve;

        [SerializeField]
        private AnimationCurve RotationCurve;

        [SerializeField]
        private Material HandMaterial;

        private int modelIdx = -1;
        private PlayerModel CurrentModel
        {
            get => Model;
        }

        public bool isInitialized { get; private set; }
        public bool isLateInitialized { get; private set; }

        public bool isMine { get; private set; }

        public int currentIdx { get; private set; }

        public string playerUuid { get; private set; }

        private bool dirty;
        public bool isDirty
        {
            get
            {
                if (Time.time - ShotTime < 2f)
                    return false;

                if (!dirty)
                    return false;

                dirty = false;
                return true;
            }

            private set => dirty = value;
        }

        public Vector3 worldPosition { get; private set; }

        public Quaternion worldRotation { get; private set; }

        //hands
        private int LHandPositionMissCount;
        private int LHandRotationMissCount;
        private int RHandPositionMissCount;
        private int RHandRotationMissCount;

        //local player data
        public Vector3 LHandPosition { get; private set; }
        public Quaternion LHandRotation { get; private set; }
        public Vector3 RHandPosition { get; private set; }
        public Quaternion RHandRotation { get; private set; }

        public Transform LHandRoot { get => CurrentModel.LHandRoot; }
        public Transform RHandRoot { get => CurrentModel.RHandRoot; }
        public Transform BodyRoot { get => CurrentModel.Root; }

        private List<Vector3> Positions = new List<Vector3>();
        private List<Quaternion> Rotations = new List<Quaternion>();

        //remote player data 
        public bool LHandActive { get; private set; }
        private bool LHandPositionUpdated;
        private bool LHandRotationUpdated;
        public bool RHandActive { get; private set; }
        private bool RHandPositionUpdated;
        private bool RHandRotationUpdated;

        //외부에서 건드리는 손 활성화값
        public bool MasterHandActive
        {
            set
            {
                LHandActive = value;
                RHandActive = value;

                LHandRoot.gameObject.SetActive((LHandPositionUpdated || LHandRotationUpdated) && LHandActive);
                RHandRoot.gameObject.SetActive((RHandPositionUpdated || RHandRotationUpdated) && RHandActive);
            }
        }

        public void UpdateHandActivation()
        {
            LHandRoot.gameObject.SetActive((LHandPositionUpdated || LHandRotationUpdated) && LHandActive);
            RHandRoot.gameObject.SetActive((RHandPositionUpdated || RHandRotationUpdated) && RHandActive);
        }

        private float MainOpacity;
        private float SubOpacity;
        private float ShotTime;


        public void Initialize(bool mine, string uuid, bool force = false)
        {
            if (isInitialized && !force)
                return;

            isMine = mine;

            currentIdx = mine ? 0 : 1;
            if (isMine)
                Model.SettingsInit(GameDataManager.instance.userInfo_mine.customModelData);
            else
                Model.SettingsInit(GameDataManager.instance.userInfos[0].customModelData);

            playerUuid = uuid;
            

            //for (int i = 0; i < AightBallPoolPlayer.players.Length; ++i)
            //{
            //    if (AightBallPoolPlayer.players[i].uuid.Equals(uuid))
            //    {
            //        isMine = AightBallPoolPlayer.players[i] == AightBallPoolPlayer.mainPlayer;

            //        currentIdx = i;
            //        playerUuid = AightBallPoolPlayer.players[i].uuid;

            //        break;
            //    }
            //}


            if (isMine)
            {
                transform.SetParent(BilliardsDataContainer.Instance.MainCamera.Value.transform, false);
                transform.localPosition = Vector3.zero;
                transform.localEulerAngles = Vector3.zero;


                BilliardsDataContainer.Instance.XRLeftControllerState.Position.OnDataChanged += LHand_Position_OnDataChanged;
                BilliardsDataContainer.Instance.XRLeftControllerState.Rotation.OnDataChanged += LHand_Rotation_OnDataChanged;
                BilliardsDataContainer.Instance.XRRightControllerState.Position.OnDataChanged += RHand_Position_OnDataChanged;
                BilliardsDataContainer.Instance.XRRightControllerState.Rotation.OnDataChanged += RHand_Rotation_OnDataChanged;
            }
            else
            {
                //SetPlayerHandColor();
            }

            LHandRoot.gameObject.SetActive(false);
            RHandRoot.gameObject.SetActive(false);

            worldPosition = transform.position;
            worldRotation = transform.rotation;

            LHandPosition = new Vector3(-1.7f, 0.109f, -1.718f);
            RHandPosition = new Vector3(-1.2489f, 0.069f, -1.718f);

            LHandRotation = Quaternion.identity;
            RHandRotation = Quaternion.identity;

            isDirty = true;
            isInitialized = true;

            MainOpacity = 1;
            SubOpacity = 1;
            //BilliardsDataContainer.Instance.TableBackgroundHiderCutoffValue.OnDataChanged += SetAdditionalOpacity;
            BilliardsDataContainer.Instance.GameState.OnDataChanged += GameState_OnDataChanged;
            //SubOpacity = BilliardsDataContainer.Instance.TableBackgroundHiderCutoffValue.CurrentData;

            Invoke("LateInitialize", Time.deltaTime);

            void SetPlayerHandColor()
            {
                var opponentIndex = GameDataManager.instance.userInfos.Select((info) => info.nick).ToList().IndexOf(BallPoolPlayer.players[1].name);

                if (BallPoolGameLogic.playMode == BallPool.PlayMode.PlayerAI)
                    opponentIndex = 1;

                var opponentData = GameDataManager.instance.GetCustomModelData(opponentIndex);

                if (ColorUtility.TryParseHtmlString("#" + opponentData.Hex_Skin_C, out var otherSkinColor))
                    HandMaterial.SetColor("_BaseColor", otherSkinColor);
            }
        }


        private void LHand_Position_OnDataChanged(Vector3 obj) { LHandPosition = obj; isDirty = true; }
        private void LHand_Rotation_OnDataChanged(Quaternion obj) { LHandRotation = obj; isDirty = true; }

        private void RHand_Position_OnDataChanged(Vector3 obj) { RHandPosition = obj; isDirty = true; }
        private void RHand_Rotation_OnDataChanged(Quaternion obj) { RHandRotation = obj; isDirty = true; }


        private void LateInitialize()
        {
            worldPosition = transform.position;
            worldRotation = transform.rotation;
            isDirty = true;

            if (!isMine)
            {
                CurrentModel.SetActive(true);

                bool isMyTurn = AightBallPoolPlayer.mainPlayer.myTurn;

                LHandRoot.gameObject.SetActive(isMyTurn);
                RHandRoot.gameObject.SetActive(isMyTurn);
            }

            isLateInitialized = true;
        }


        private void GameState_OnDataChanged(ShotController.GameStateType state)
        {
            if (state == ShotController.GameStateType.Shot)
            {
                ShotTime = Time.time;
            }
        }

        private void Update()
        {
            if (!isLateInitialized)
                return;

            if (isMine)
            {
                if (transform.position != worldPosition)
                {
                    worldPosition = transform.position;
                    isDirty = true;
                }

                if (transform.rotation != worldRotation)
                {
                    worldRotation = transform.rotation;
                    isDirty = true;
                }
            }
            else
            {
                worldPosition = transform.position;
                worldRotation = transform.rotation;


                //update HandPosition
                if (Positions.Count == 3)
                {
                    transform.position = Vector3.Lerp(transform.position, Positions[0], 5.0f * Time.deltaTime);
                    LHandRoot.position = Vector3.Lerp(LHandRoot.position, Positions[1], 5.0f * Time.deltaTime);
                    RHandRoot.position = Vector3.Lerp(RHandRoot.position, Positions[2], 5.0f * Time.deltaTime);
                }


                //update hand rotation
                if (Rotations.Count == 3)
                {
                    float targetBodyAngle = 0;
                    var diff = Rotations[0].eulerAngles.y - CurrentModel.Root.rotation.eulerAngles.y;
                    if (diff < -180) diff += 360;
                    if (diff > 180) diff -= 360;
                    if (Mathf.Abs(diff) > GameConfig.PlayerSnapAngle)
                    {
                        targetBodyAngle = Mathf.Sign(diff) * (Mathf.Abs(diff) - GameConfig.PlayerSnapAngle);
                    }

                    var bodyRotation = Quaternion.Euler(0, CurrentModel.Root.rotation.eulerAngles.y + targetBodyAngle, 0);

                    CurrentModel.Root.rotation = Quaternion.Lerp(CurrentModel.Root.rotation, bodyRotation, 5.0f * Time.deltaTime);
                    CurrentModel.HeadRoot.rotation = Quaternion.Lerp(CurrentModel.HeadRoot.rotation, Rotations[0], 5.0f * Time.deltaTime);
                    LHandRoot.rotation = Quaternion.Lerp(LHandRoot.rotation, Rotations[1], 5.0f * Time.deltaTime);
                    RHandRoot.rotation = Quaternion.Lerp(RHandRoot.rotation, Rotations[2], 5.0f * Time.deltaTime);
                }
            }
        }


        public void SetMainOpacity(float opacity)
        {
            MainOpacity = opacity;
            SetOpacity();
        }

        //public void SetAdditionalOpacity(float opacity)
        //{
        //    SubOpacity = opacity;
        //    SetOpacity();
        //}

        private void SetOpacity()
        {
            if (MainOpacity * SubOpacity < 0.5f)
                CurrentModel.SetActive(false && isLateInitialized && !isMine);
            else
                CurrentModel.SetActive(true && isLateInitialized && !isMine);
        }

        //network
        /// <param name="worldPositions">[0]worldPosition, [1]lHandWorldPosition, [2]rHandWorldPosition</param>
        public PlayerInstance UpdatePositions(List<Vector3> worldPositions)
        {
            if (worldPositions.Count != 3)
                return this;

            if(Positions.Count == 3)
            {
                CheckHandPositionUpdate(ref LHandPositionMissCount, ref LHandPositionUpdated, Positions[1], worldPositions[1]);
                CheckHandPositionUpdate(ref RHandPositionMissCount, ref RHandPositionUpdated, Positions[2], worldPositions[2]);
            }
            else
            {
                LHandPositionMissCount = 0;
                RHandPositionMissCount = 0;
                LHandPositionUpdated = true;
                RHandPositionUpdated = true;
            }

            UpdateHandActivation();

            Positions = worldPositions;

            return this;

            void CheckHandPositionUpdate(ref int count, ref bool updated, in Vector3 lastPosition, in Vector3 currentPosition)
            {
                count = lastPosition == currentPosition ? count + 1 : 0;
                updated = count < GameConfig.PlayerHandUpdateThreshold;
            }
        }

        /// <param name="rotations">[0]rotations,[1]  lHandWorldRotation, [2] rHandWorldRotation</param>
        public PlayerInstance UpdateRotations(List<Quaternion> rotations)
        {
            if (rotations.Count != 3)
                return this;

            if (Rotations.Count == 3)
            {
                CheckHandRotationUpdate(ref LHandRotationMissCount, ref LHandRotationUpdated, Rotations[1], rotations[1]);
                CheckHandRotationUpdate(ref RHandRotationMissCount, ref RHandRotationUpdated, Rotations[2], rotations[2]);
            }
            else
            {
                LHandRotationMissCount = 0;
                RHandRotationMissCount = 0;
                LHandRotationUpdated = true;
                RHandRotationUpdated = true;
            }

            UpdateHandActivation();

            Rotations = rotations;

            return this;

            void CheckHandRotationUpdate(ref int count, ref bool updated, in Quaternion lastPosition, in Quaternion currentPosition)
            {
                var qdot = Quaternion.Dot(lastPosition, currentPosition);
                count = Mathf.Approximately(Mathf.Abs(qdot), 1f) ? count + 1 : 0;
                updated = count < GameConfig.PlayerHandUpdateThreshold;
            }
        }


        private void OnDestroy()
        {
            BilliardsDataContainer.Instance.XRLeftControllerState.Position.OnDataChanged -= LHand_Position_OnDataChanged;
            BilliardsDataContainer.Instance.XRRightControllerState.Position.OnDataChanged -= RHand_Position_OnDataChanged;

            BilliardsDataContainer.Instance.GameState.OnDataChanged -= GameState_OnDataChanged;
            //BilliardsDataContainer.Instance.TableBackgroundHiderCutoffValue.OnDataChanged -= SetAdditionalOpacity;
        }

    }

}