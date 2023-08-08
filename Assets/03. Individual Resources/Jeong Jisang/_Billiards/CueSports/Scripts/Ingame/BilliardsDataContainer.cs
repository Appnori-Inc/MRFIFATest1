using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Billiards
{
    using Appnori.Util;
    using Appnori.XR;
    using BallPool;
    using Unity.XR.CoreUtils;
    using UnityEngine.XR.Interaction.Toolkit;
    using XRControllerState = Appnori.XR.XRControllerState;

    public class BilliardsDataContainer : Singleton<BilliardsDataContainer>
    {
        //game
        #region Game
        public readonly Notifier<XROrigin> XRRigid = new Notifier<XROrigin>();
        public readonly Notifier<XRReticleProvider> XRLeftReticleProvider = new Notifier<XRReticleProvider>();
        public readonly Notifier<XRReticleProvider> XRRightReticleProvider = new Notifier<XRReticleProvider>();

        public readonly XRControllerState XRLeftControllerState = new XRControllerState();
        public readonly XRControllerState XRRightControllerState = new XRControllerState();

        public readonly Notifier<LockMarker> LeftLockMarker = new Notifier<LockMarker>();
        public readonly Notifier<LockMarker> RightLockMarker = new Notifier<LockMarker>();

        public readonly Notifier<Transform> TableCameraCenter = new Notifier<Transform>();
        public readonly Notifier<Transform> TableCameraSlot = new Notifier<Transform>();
        public readonly Notifier<Transform> CueBallCameraSlot = new Notifier<Transform>();
        public readonly Notifier<Transform> StandardCameraSlot = new Notifier<Transform>();
        public readonly Notifier<Transform> StandardCueBallCameraSlot = new Notifier<Transform>();
        public readonly Notifier<Transform> WorldTempCameraSlot = new Notifier<Transform>();

        public readonly Notifier<FollowPosition> CueBallCameraRootFollower = new Notifier<FollowPosition>();
        public readonly Notifier<bool> CueSnapState = new Notifier<bool>();
        public readonly Notifier<bool> AllowedSetCuePositionState = new Notifier<bool>();

        //uiEvent
        public readonly Notifier<bool> MainHandLineActivation = new Notifier<bool>();
        public readonly Notifier<bool> SubHandLineActivation = new Notifier<bool>();
        public readonly Notifier<bool> isFoulVoicePlaying = new Notifier<bool>();

        //legacy
        public readonly Notifier<ShotController.CueStateType> CueState = new Notifier<ShotController.CueStateType>();

        public readonly Notifier<ShotController.GameStateType> GameState = new Notifier<ShotController.GameStateType>();
        public readonly Notifier<ShotController.GameStateType> OpponentGameState = new Notifier<ShotController.GameStateType>();
        public readonly Notifier<bool> OpponentMainHanded = new Notifier<bool>();

        //Home-RaycastCamera
        public readonly Notifier<Camera> MainCamera = new Notifier<Camera>();

        public readonly Notifier<int> TargetBallIds = new Notifier<int>();
        public readonly Notifier<int> HittedBallIds = new Notifier<int>();

        //Carom
        public readonly Notifier<int> CaromLife = new Notifier<int>();

        public readonly Notifier<int> TurnCount = new Notifier<int>(); // for achievement
        #endregion Game


        //subScreen
        #region SubScreen
        /// <summary>
        /// calculatedHitInfo
        /// </summary>
        [Obsolete]
        public Notifier<RaycastHit> TryCalculatedHit = new Notifier<RaycastHit>();

        public Notifier<BackgroundHider> TableBackgroundHider = new Notifier<BackgroundHider>();
        public Notifier<float> TableBackgroundHiderCutoffValue = new Notifier<float>();

        public Notifier<float> NormalizedFadeTime = new Notifier<float>();
        #endregion SubScreen
    }
}