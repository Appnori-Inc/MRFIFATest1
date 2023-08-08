using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BallPool;

namespace Billiards
{
    using System;
    using System.Linq;

    public class PlayerManager : MonoBehaviour
    {
        [SerializeField]
        private PlayerInstance PlayerOrigin;
        [SerializeField]
        private PlayerHand playerHand;

        private Dictionary<string, PlayerInstance> PlayerDictionary = new Dictionary<string, PlayerInstance>();


        public event Action<PlayerInstance> OnPlayerInitialized;

        private PlayerInstance _mine;
        public PlayerInstance Mine
        {
            get
            {
                if (_mine != null)
                    return _mine;

                var instance = GameObject.Instantiate(PlayerOrigin.gameObject);
                _mine = instance.GetComponent<PlayerInstance>();
                _mine.Initialize(true,AightBallPoolPlayer.mainPlayer.uuid);
                //PlayerDictionary[AightBallPoolPlayer.mainPlayer.uuid] = _mine;

                return _mine;
            }
        }

        private static PlayerManager manager;

        private static event Action<PlayerManager> onInitialized;
        public static event Action<PlayerManager> OnInitialized
        {
            add
            {
                if (manager != null)
                {
                    value?.Invoke(manager);
                    return;
                }

                onInitialized += value;
            }
            remove
            {
                onInitialized -= value;
            }
        }

        public void Awake()
        {
            manager = this;

            onInitialized?.Invoke(this);
            onInitialized = null;
        }

        public void ForceSetOtherPlayerHand(bool value)
        {
            using (var e = PlayerDictionary.Values.ToList().GetEnumerator())
                while (e.MoveNext())
                    if (e.Current != Mine)
                        e.Current.MasterHandActive = value;
        }

        public void SetPlayerTransform(string uuid, List<Vector3> worldPosition, List<Quaternion> rotation)
        {
            if (!PlayerDictionary.TryGetValue(uuid, out var player))
            {
                //initialization
                var instance = GameObject.Instantiate(PlayerOrigin.gameObject);
                player = instance.GetComponent<PlayerInstance>();
                player.Initialize(false,uuid);

                //if(player.TryGetComponent<CustomModelSettingCtrl>(out var setter))
                //{
                //    setter.InitCPU(CustomModelViewState.HalfCut, playerHand.LeftCueBridgeHand, GameConfig.PlayerCharacterToneValue);
                //}

                //if (CheckPlayerNameIsAI(uuid))
                //{
                //    setter.InitCPU(CustomModelViewState.HalfCut, playerHand.LeftCueBridgeHand, GameConfig.PlayerCharacterToneValue);
                //}
                //else
                //{
                //    var target = GameDataManager.instance.userInfos.Where((info) => info.id == uuid).FirstOrDefault();
                //    if (string.IsNullOrEmpty(target.nick))
                //    {
                //        var name = AightBallPoolPlayer.players.Where((p) => p.uuid == uuid).FirstOrDefault().name;
                //        target = GameDataManager.instance.userInfos.Where((info) => info.nick == name).FirstOrDefault();
                //    }
                //    //setter.Init(target.id, CustomModelViewState.HalfCut, playerHand.LeftCueBridgeHand, GameConfig.PlayerCharacterToneValue);
                //    setter.Init(target.customModelData, CustomModelViewState.HalfCut, playerHand.LeftCueBridgeHand, GameConfig.PlayerCharacterToneValue);
                //}

                PlayerDictionary[uuid] = player;
                OnPlayerInitialized?.Invoke(player);
            }

            player.UpdatePositions(worldPosition).UpdateRotations(rotation);
        }

        private bool CheckPlayerNameIsAI(string uuid)
        {
            if (uuid.Equals("AI"))
                return true;

            return false;
        }

        private void Update()
        {
            using (var e = PlayerDictionary.Values.ToList().GetEnumerator())
            {
                while (e.MoveNext())
                {
                    if (e.Current.isMine)
                        continue;

                    var distance = (Mine.worldPosition - e.Current.worldPosition).magnitude;
                    e.Current.SetMainOpacity((distance * 1.5f) - 0.7f);
                }
            }
        }

        private void OnDestroy()
        {
            manager = null;
        }

    }
}