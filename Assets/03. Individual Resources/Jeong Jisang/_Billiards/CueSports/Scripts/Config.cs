using UnityEngine;
using System.Collections;


namespace Billiards
{

    public static class GameConfig
    {
        //World offset y : -0.91607
        #region Scene
        public const string TableSceneName = "Home_map";
        #endregion

        #region Game
        public static readonly int[] CaromSingleTargetScore = { 5, 10, 15, 20, 25 };
        public static int CurrentCaromSingleTargetScore { get => CaromSingleTargetScore[LevelLoader.CurrentMatchInfo.level - 1]; }
        public const int CaromMultiplayerTargetScore = 5;
        public const int CaromSinglePlayerLife = 5;
        public const int CaromMaxPlayerTargetScore = 5;


        public const float MaxTurnTime = 50f;
        public const float TurnEndDelay = 1.2f;

        //
        public const float MinBallMoveEnergy = 0.09f;
        public const float MinContactThresholdVelocity = 0.00001f;

        //0.02829273
        public const float CueSliderRadius = 0.02829273f;
        public const float CuePivotRotateSpeed = 1.45f;
        public const float CuePivotRotateSpeedDecreaseTime = 2f;
        public const float TableCenterRotateSpeed = 2f;
        public const float CueDistanceMoveSpeed = 0.05f;
        public const float CueMoveThreshold = 0.25f;
        public const float CueDistanceMin = 0.8f;
        public const float CueDistanceMax = 1.3f;

        public const float LineLengthPerLevel = 0.07f;
        public const float DefaultLineLength = 0.25f;
        public const float MultiplayerLineLength = 0.075f;

        public const float SpinBallRadiusRate = 0.825f;
        public const float SpinSliderPositionZ = -0.02f;
        public const float TableSurfaceOffset = 0.91607f;

        public const float SubControllerSnapAngle = 15f;
        public const float MainControllerTrackingRate = 0.075f;

        public const float IgnoreTwoTouchTime = 0.05f;
        #endregion

        #region Controller
        public const float MaxControllerXAxisAngularVelocity = 7f;
        public const float ControllerXAxisAngularVelocityRate = 0.35f;
        public const float ControllerVelocityRate = 0.55f;

        public static readonly Vector2 TouchRectMin = new Vector2(50, 50);
        public static readonly Vector2 TouchRectMax = new Vector2(200, 160);
        #endregion

        #region AI
        public const float AIMinimumImpulseRate = 0.5f;
        public const float AIMaximumImpulseRate = 0.7f;

        public const float AIDumbPerLevel = 0.1225f;
        public const float AIMinimumDumb = 0.2f;
        #endregion

        #region UI
        public const float SubScreenViewSize = 1.6f;
        public const float SubScreenViewAdditionalRange = 0.2f;
        public const float SubScreenViewAdditionalScale = 2f;

        public const float HitPositionMarkerRadius = 0.008f;

        public const float TutorialExposeTime = 5f;

        public const int MaxCushionTrace = 2;
        #endregion

        #region Network
        public const float SendGameInterval = 0.10f;
        public const float SendEnvironmentInterval = 0.075f;
        #endregion Network

        #region Player
        public const float PlayerSnapAngle = 22.5f;
        public const int PlayerHandUpdateThreshold = 5;
        public const float PlayerCharacterToneValue = 0.18f;
        public static readonly Vector3 PlayerLeftHandDefaultLocalPosition = new Vector3(-0.0031f, -0.0004f, -0.5079f);
        public static readonly Vector3 PlayerLeftHandDefaultLocalEuler = new Vector3(-0.168f, 0, 0);
        public const float MinimumCuePositionHeight = 0.008715574f; // 5도, normalized * 0.1f 기준
        public const float MinimumCueAngle = 5f; // 5도, normalized * 0.1f 기준
        #endregion

        #region Sound
        public const string FxSoundGroupName = "Fx";
            #endregion
    }

}