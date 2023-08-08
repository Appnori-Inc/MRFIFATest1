using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Billiards
{
    using BallPool;
    using System;

    [Obsolete]
    public class UISubCameraView : MonoBehaviour
    {
        [SerializeField]
        private RectTransform RenderTextureRoot;

        [SerializeField]
        private Canvas RenderCanvas;

        [SerializeField]
        private Camera RenderCamera;

        [SerializeField]
        private float openTime;

        private bool IsBallHit;
        private RaycastHit CurrentHitInfo;

        private bool AllowedShow = false;

        private Coroutine OpenRoutine;
        private Coroutine CloseRoutine;

        //private bool isDirty = false;
        private float TargetRenderTextureRootScale;

        private static readonly string BALL = "Ball";
        private static readonly string LINE = "Line";

        public enum HitType
        {
            None,
            Ball,
            Table
        }

        private void Awake()
        {
            //BilliardsDataContainer.Instance.GameState.OnDataChanged += GameState_OnDataChanged;
            //BilliardsDataContainer.Instance.TryCalculatedHit.OnDataChanged += OnHitChanged;

            RenderCamera.orthographicSize = GameConfig.SubScreenViewSize;
            RenderCanvas.gameObject.SetActive(false);
        }

        private void GameState_OnDataChanged(ShotController.GameStateType obj)
        {
            switch (obj)
            {
                case ShotController.GameStateType.WaitingForOpponent:
                case ShotController.GameStateType.MoveAroundTable:
                case ShotController.GameStateType.SetBallPosition:
                    AllowedShow = false;
                    //isDirty = true;
                    Close();
                    break;

                case ShotController.GameStateType.SelectShotDirection:
                    AllowedShow = true;
                    Show();
                    break;

                case ShotController.GameStateType.CameraFixAndWaitShot:
                    AllowedShow = false;
                    Close();
                    break;

                case ShotController.GameStateType.Shot:
                    AllowedShow = false;
                    //isDirty = true;
                    Close();
                    break;
                default:
                    break;
            }
        }

        public void OnHitChanged(RaycastHit current)
        {
            if (current.transform == null)
                return;

            if (!AllowedShow)
            {
                CurrentHitInfo = current;
                return;
            }

            if (current.transform.name.Contains(BALL))
            {
                IsBallHit = true;
            }
            else if (current.transform.name.Contains(LINE))
            {
                IsBallHit = false;
            }
            else
            {
                Close();
                return;
            }

            var lastTransform = CurrentHitInfo.transform;

            CurrentHitInfo = current;
            //isDirty = false;

            if (current.transform != lastTransform)
                Show();
        }

        public void Show()
        {
            if (CurrentHitInfo.transform == null /*|| isDirty*/)
            {
                return;
            }

            if (CurrentHitInfo.transform.name.Contains(BALL))
            {
                IsBallHit = true;
            }
            else if (CurrentHitInfo.transform.name.Contains(LINE))
            {
                IsBallHit = false;
            }

            if (CloseRoutine != null)
                StopCoroutine(CloseRoutine);

            if (OpenRoutine != null)
                StopCoroutine(OpenRoutine);

            OpenRoutine = StartCoroutine(OpenInternal());
        }


        private IEnumerator OpenInternal()
        {
            if (IsBallHit)
                InitializeSubScreen(CurrentHitInfo.transform, Vector3.zero, GameConfig.SubScreenViewAdditionalRange);
            else
                InitializeSubScreen(null, CurrentHitInfo.point, 0f);

            //animation
            float t = 0;
            Vector3 startScale = new Vector3(0.04f, 0.04f, 1);
            Vector3 middleScale = new Vector3(1f, 0.1f, 1) * TargetRenderTextureRootScale;
            while (t < (openTime * 0.5f))
            {
                RenderTextureRoot.transform.localScale = Vector3.Lerp(startScale, middleScale, t / (openTime * 0.5f));
                t += Time.deltaTime;
                yield return null;
            }

            while (t < openTime)
            {
                RenderTextureRoot.transform.localScale = Vector3.Lerp(middleScale, Vector3.one * TargetRenderTextureRootScale, (t - (openTime * 0.5f)) / (openTime * 0.5f));
                t += Time.deltaTime;
                yield return null;
            }

            RenderTextureRoot.transform.localScale = Vector3.one * TargetRenderTextureRootScale;
            OpenRoutine = null;
        }

        private void InitializeSubScreen(Transform parent, Vector3 canvasLocalPosition, float RenderCameraAdditionalPosition)
        {
            RenderCanvas.transform.SetParent(parent, false);
            RenderCanvas.transform.localPosition = canvasLocalPosition;

            var dir = (CurrentHitInfo.transform.position - BilliardsDataContainer.Instance.MainCamera.Value.transform.position).ToXZ();
            RenderCanvas.gameObject.SetActive(true);
            RenderCamera.transform.localEulerAngles = new Vector3(RenderCamera.transform.localEulerAngles.x, 0, Vector2.SignedAngle(Vector2.up, dir));
            RenderCamera.transform.position = RenderCanvas.transform.position + Vector3.up + (dir.ToVector3FromXZ().normalized * RenderCameraAdditionalPosition);

            var size = GetSizeFormMagnitude(dir.magnitude);
            TargetRenderTextureRootScale = size;
            RenderTextureRoot.localPosition = new Vector2(0, 800f * (1f - ((size - 1) * 0.5f)));
        }

        public void Close()
        {
            if (OpenRoutine != null)
                StopCoroutine(OpenRoutine);

            if (CloseRoutine != null)
                StopCoroutine(CloseRoutine);

            CloseRoutine = StartCoroutine(CloseInternal());

        }

        private IEnumerator CloseInternal()
        {
            float t = 0;
            Vector3 defaultScale = RenderTextureRoot.transform.localScale;
            Vector3 startScale = new Vector3(0.04f, 0.04f, 1);
            Vector3 middleScale = new Vector3(1f, 0.1f, 1) * TargetRenderTextureRootScale;

            while (t < (openTime * 0.5f))
            {
                RenderTextureRoot.transform.localScale = Vector3.Lerp(defaultScale, middleScale, t / (openTime * 0.5f));
                t += Time.deltaTime;
                yield return null;
            }

            while (t < openTime)
            {
                RenderTextureRoot.transform.localScale = Vector3.Lerp(middleScale, startScale, (t - (openTime * 0.5f)) / (openTime * 0.5f));
                t += Time.deltaTime;
                yield return null;
            }

            RenderCanvas.gameObject.SetActive(false);
            CloseRoutine = null;
        }


        private float GetSizeFormMagnitude(float directionMagnitude)
        {
            return 1 + Mathf.Clamp(((directionMagnitude - 1) * 0.33f), 0f, 1f);
        }

        private void OnDestroy()
        {
            //BilliardsDataContainer.Instance.GameState.OnDataChanged -= GameState_OnDataChanged;

            //BilliardsDataContainer.Instance.TryCalculatedHit.OnDataChanged -= OnHitChanged;
        }
    }
}