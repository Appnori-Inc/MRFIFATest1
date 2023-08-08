using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace Billiards
{
    using BallPool;

    public class TableRaycastReactor : BaseRaycastReactor
    {
        private enum ReactType
        {
            None,
            HitOnly,
            HitToEdge
        }


        [SerializeField]
        private BoxCollider collider;

        [SerializeField]
        private Transform cameraCenter;

        [SerializeField]
        private Transform EdgePointerTransform;

        [SerializeField]
        private Transform RayHitPointerTransform;

        [SerializeField]
        private LineRenderer lineRenderer;

        //[SerializeField]
        //private LineRenderer EdgeLineRenderer;

        private ReactType currentReactionType = ReactType.HitToEdge;

        private Vector2 RectMax;
        private Vector2 RectMin;

        public Vector3 CurrentEdgeLocalPosition;
        public Vector3 LateEdgeLocalPosition;
        public Vector3 CurrentHitLocalPosition;

        private bool isNowTracking = false;
        private const float TABLE_EDGE_DISTANCE = 0.09610341f;

        private ReactType lastType;

        private void Awake()
        {
            currentReactionType = ReactType.None;
            RectMax = new Vector2(collider.center.x + (collider.size.x * 0.5f - TABLE_EDGE_DISTANCE), collider.center.z + (collider.size.z * 0.5f - TABLE_EDGE_DISTANCE));
            RectMin = new Vector2(collider.center.x - (collider.size.x * 0.5f - TABLE_EDGE_DISTANCE), collider.center.z - (collider.size.z * 0.5f - TABLE_EDGE_DISTANCE));
        }

        private void OnEnable()
        {
            cameraCenter = BilliardsDataContainer.Instance.TableCameraCenter.Value;

            BilliardsDataContainer.Instance.GameState.OnDataChanged += GameState_OnDataChanged;
            BilliardsDataContainer.Instance.TableCameraCenter.OnDataChanged += TableCameraCenter_OnDataChanged;
        }

        private void TableCameraCenter_OnDataChanged(Transform obj)
        {
            cameraCenter = obj;
        }

        private void GameState_OnDataChanged(ShotController.GameStateType obj)
        {
            switch (obj)
            {
                //case ShotController.GameStateType.WaitingForOpponent:
                //case ShotController.GameStateType.MoveAroundTable:
                //    currentReactionType = ReactType.HitToEdge;
                //    break;

                case ShotController.GameStateType.SetBallPosition:
                case ShotController.GameStateType.SelectShotDirection:
                    currentReactionType = ReactType.HitOnly;
                    break;

                //case ShotController.GameStateType.CameraFixAndWaitShot:
                //case ShotController.GameStateType.SetHitPosition:
                //case ShotController.GameStateType.Shot:
                //    currentReactionType = ReactType.None;
                //    break;

                default:
                    currentReactionType = ReactType.None;
                    break;
            }
        }

        private void OnDisable()
        {
            BilliardsDataContainer.Instance.TableCameraCenter.OnDataChanged -= TableCameraCenter_OnDataChanged;
            BilliardsDataContainer.Instance.GameState.OnDataChanged -= GameState_OnDataChanged;
        }

        public override void OnRay(RaycastHit hitInfo)
        {
            if (!isNowTracking)
            {
                isNowTracking = true;
                SetPresetByCurrentType();
            }

            SetEdgePositionByTablePivot(hitInfo.point);
        }

        private void SetEdgePositionByTablePivot(Vector3 hitPos)
        {
            //get Edge Position By Table Pivot
            var dir = transform.InverseTransformPoint(hitPos).ToXZ();
            dir = dir.normalized * 10f;

            float slope = dir.y / dir.x;
            dir.y = Mathf.Clamp(dir.y, RectMin.y, RectMax.y);
            dir.x = slope == 0 ? dir.x : (dir.y / slope);

            if (Mathf.Abs(dir.x) > RectMax.x)
            {
                dir.x = Mathf.Clamp(dir.x, RectMin.x, RectMax.x);
                dir.y = slope * dir.x;
            }

            CurrentHitLocalPosition = transform.InverseTransformPoint(hitPos);
            CurrentEdgeLocalPosition = new Vector3(dir.x, CurrentHitLocalPosition.y, dir.y);
        }

        public override void OnRayEnd(RaycastHit hitInfo)
        {
            isNowTracking = false;
            SetPresetByCurrentType(ReactType.None);
        }

        private void Update()
        {
            if (!isNowTracking || currentReactionType == ReactType.None)
            {
                return;
            }

            SetPresetByCurrentType();

            //hit
            RayHitPointerTransform.localPosition = Vector3.Lerp(RayHitPointerTransform.localPosition, CurrentHitLocalPosition, 0.5f);

            if (currentReactionType == ReactType.HitOnly)
                return;

            //edge

            LateEdgeLocalPosition = Vector3.Lerp(LateEdgeLocalPosition, CurrentEdgeLocalPosition, 0.5f);

            EdgePointerTransform.localPosition = LateEdgeLocalPosition;

            lineRenderer.SetPositions(ConvertToLinePosition(RayHitPointerTransform.localPosition, LateEdgeLocalPosition));

            SetEdgePointerLookDirection();


            //LineTextureFlow
            var flowSpeed = Vector2.left * Time.time * Mathf.Abs((RayHitPointerTransform.localPosition - LateEdgeLocalPosition).magnitude) * 0.2f;
            lineRenderer.sharedMaterial.SetTextureOffset("_MainTex", flowSpeed);
        }

        private void SetPresetByCurrentType(ReactType? overridedType = null)
        {
            var currentType = overridedType.HasValue ? overridedType.Value : currentReactionType;

            if (lastType == currentType)
                return;

            switch (currentType)
            {
                case ReactType.None:
                    lineRenderer.enabled = false;
                    lineRenderer.positionCount = 0;
                    RayHitPointerTransform.gameObject.SetActive(false);
                    EdgePointerTransform.gameObject.SetActive(false);
                    break;

                case ReactType.HitOnly:
                    lineRenderer.enabled = false;
                    lineRenderer.positionCount = 0;
                    RayHitPointerTransform.gameObject.SetActive(true);
                    EdgePointerTransform.gameObject.SetActive(false);
                    break;

                case ReactType.HitToEdge:
                    lineRenderer.enabled = true;
                    lineRenderer.positionCount = 2;
                    RayHitPointerTransform.gameObject.SetActive(true);
                    EdgePointerTransform.gameObject.SetActive(true);
                    break;
            }

            lastType = currentType;
        }

        public override bool RaycastReact(RaycastHit hitInfo)
        {
            if (currentReactionType == ReactType.HitToEdge)
            {
                var pos = -transform.TransformPoint(CurrentEdgeLocalPosition);
                pos.y = cameraCenter.position.y;

                cameraCenter.LookAt(pos, Vector3.up);
                return true;
            }

            return false;
        }

        //TO zup
        private Vector3[] ConvertToLinePosition(Vector3 start, Vector3 end)
        {
            var list = new List<Vector3>();
            //var dir = (end.ToXZ() - start.ToXZ()).normalized;

            list.Add(start.ToXZ());
            list.Add(end.ToXZ());

            return list.ToArray();
        }

        private void SetEdgePointerLookDirection()
        {
            var xzLocal = CurrentEdgeLocalPosition.ToXZ();

            if (Mathf.Approximately(xzLocal.x, RectMax.x))
                EdgePointerTransform.LookAt(EdgePointerTransform.position + Vector3.right);
            else if (Mathf.Approximately(xzLocal.x, RectMin.x))
                EdgePointerTransform.LookAt(EdgePointerTransform.position + Vector3.left);
            else if (Mathf.Approximately(xzLocal.y, RectMax.y))
                EdgePointerTransform.LookAt(EdgePointerTransform.position + Vector3.forward);
            else if (Mathf.Approximately(xzLocal.y, RectMin.y))
                EdgePointerTransform.LookAt(EdgePointerTransform.position + Vector3.back);
            else
                EdgePointerTransform.LookAt(transform.TransformPoint(CurrentHitLocalPosition).ToXZ().ToVector3FromXZ(EdgePointerTransform.position.y));
        }
    }
}
