using Appnori.Util;
using Appnori.XR;
using BallPool;
using BallPool.Mechanics;
using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using XRControllerState = Appnori.XR.XRControllerState;
namespace Billiards
{

    public class InteractableTable : XRBaseInteractable
    {
        [SerializeField]
        private XROrigin xrRig;

        [SerializeField]
        private Transform TableSpace;

        [SerializeField]
        private Transform EdgePointerTransform;

        [SerializeField]
        private Transform RayHitPointerTransform;

        [SerializeField]
        private LineRenderer lineRenderer;

        [SerializeField]
        private XRNode ReactTarget = XRNode.RightHand;

        private Vector3 CurrentEdgePosition;
        private Vector3 LateEdgePosition;
        private Vector3 CurrentHitPosition;

        private Vector3 ReticlePosition;

        private const float TABLE_EDGE_DISTANCE = 0.09610341f;

        private bool isNowTracking;

        private List<XRBaseInteractor> cached = new List<XRBaseInteractor>();

        private XRControllerState currentControllerState
        {
            get
            {
                switch (ReactTarget)
                {
                    case XRNode.LeftHand: return BallPool.ShotController.SubHandController;
                    case XRNode.RightHand: return BallPool.ShotController.MainHandController;
                    default: return null;
                }
            } 
        }

        private Notifier<XRReticleProvider> currentReticleProvider
        {
            get
            {
                switch (ReactTarget)
                {
                    case XRNode.LeftHand:
                        if (BallPool.ShotController.SubHandController == BilliardsDataContainer.Instance.XRLeftControllerState)
                            return BilliardsDataContainer.Instance.XRLeftReticleProvider;
                        else
                            return BilliardsDataContainer.Instance.XRRightReticleProvider;

                    case XRNode.RightHand:
                        if (BallPool.ShotController.MainHandController == BilliardsDataContainer.Instance.XRRightControllerState)
                            return BilliardsDataContainer.Instance.XRLeftReticleProvider;
                        else
                            return BilliardsDataContainer.Instance.XRRightReticleProvider;

                    default: return null;
                }
            }
        }

        protected override void Awake()
        {
            base.Awake();

            isNowTracking = false;

            SetActiveLine(false);

            if (xrRig == null)
            {
                if (BilliardsDataContainer.Instance.XRRigid.Value == null)
                {
                    BilliardsDataContainer.Instance.XRRigid.OnDataChangedOnce += (xrRig) => this.xrRig = xrRig;
                }
                else
                {
                    xrRig = BilliardsDataContainer.Instance.XRRigid.Value;
                }
            }

            if (currentReticleProvider.Value == null)
            {
                currentReticleProvider.OnDataChangedOnce += (provider) => provider.ReticleMovementevent.AddListener(OnReticlePositionUpdate);
            }
            else
            {
                currentReticleProvider.Value.ReticleMovementevent.AddListener(OnReticlePositionUpdate);
            }

        }

        public void OnReticlePositionUpdate(XRNode node, Vector3 position)
        {
            if (node != ReactTarget)
                return;

            if (BilliardsDataContainer.Instance.GameState.Value != BallPool.ShotController.GameStateType.WaitingForOpponent)
                return;

            ReticlePosition = position;

            if (isNowTracking)
            {
                var rayOrigin = (currentControllerState.ControllerRayOrigin.Value);
                var dir = (ReticlePosition.ToXZ() - rayOrigin.ToXZ()) * 100f;
                var edgePosition = Geometry.EdgeProjectionXZ(dir, rayOrigin.ToXZ(), TableSpace);

                CurrentHitPosition = (ReticlePosition);
                CurrentEdgePosition = edgePosition.ToVector3FromXZ(-0.00016f).Round(0.02f);
            }
        }

        protected override void OnHoverEntered(XRBaseInteractor interactor)
        {
            base.OnHoverEntered(interactor);

            cached.Add(interactor);

            if (CacheManager.Get<XRController>(interactor).controllerNode != ReactTarget)
                return;

            isNowTracking = true;

            SetActiveLine(true);
        }

        protected override void OnSelectEntered(XRBaseInteractor interactor)
        {
            base.OnSelectEntered(interactor);

            cached.Add(interactor);

            if (CacheManager.Get<XRController>(interactor).controllerNode != ReactTarget)
                return;

            if (xrRig == null)
                return;

            if (BilliardsDataContainer.Instance.GameState.Value != BallPool.ShotController.GameStateType.WaitingForOpponent)
                return;

            if (AightBallPoolPlayer.mainPlayer.myTurn)
                return;

            //move
            xrRig.MoveCameraToWorldLocation((CurrentEdgePosition * 1.3f).ToXZ().ToVector3FromXZ(xrRig.Camera.transform.position.y));
            //rotate
            var targetEulerAngles = Quaternion.LookRotation(transform.position - xrRig.Camera.transform.position).eulerAngles;
            xrRig.RotateAroundCameraUsingOriginUp(targetEulerAngles.y - xrRig.Camera.transform.eulerAngles.y);
        }

        private void Update()
        {
            if (!isNowTracking)
            {
                return;
            }

            var allow =
            BilliardsDataContainer.Instance.XRLeftControllerState[InputHelpers.Button.Trigger].Value == false &&
            BilliardsDataContainer.Instance.XRRightControllerState[InputHelpers.Button.Trigger].Value == false &&
            BilliardsDataContainer.Instance.GameState.Value == BallPool.ShotController.GameStateType.WaitingForOpponent;

            if (!allow)
            {
                SetActiveLine(false);
                return;
            }
            else
            {
                SetActiveLine(true);
            }

            //hit
            RayHitPointerTransform.position = Vector3.Lerp(RayHitPointerTransform.position, CurrentHitPosition, 0.5f);

            //edge
            LateEdgePosition = Vector3.Lerp(LateEdgePosition, CurrentEdgePosition, 0.5f);

            EdgePointerTransform.position = LateEdgePosition;

            lineRenderer.positionCount = 2;
            lineRenderer.SetPositions(ConvertToLinePosition(RayHitPointerTransform.localPosition, EdgePointerTransform.localPosition));

            SetEdgePointerLookDirection();

            //LineTextureFlow
            //var flowSpeed = Vector2.left * Time.time * Mathf.Abs((RayHitPointerTransform.localPosition - LateEdgePosition).magnitude) * 0.2f;
            //lineRenderer.sharedMaterial.SetTextureOffset("_MainTex", flowSpeed);
        }


        protected override void OnSelectExited(XRBaseInteractor interactor)
        {
            if (CacheManager.Get<XRController>(interactor).controllerNode != ReactTarget)
                return;

            base.OnSelectExited(interactor);

            if (gameObject == null)
                return;
        }

        protected override void OnHoverExited(XRBaseInteractor interactor)
        {
            base.OnHoverExited(interactor);

            if (CacheManager.Get<XRController>(interactor).controllerNode != ReactTarget)
                return;

            if (gameObject == null)
                return;

            isNowTracking = false;
            SetActiveLine(false);
        }

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
            var xzLocal = CurrentEdgePosition.ToXZ();

            if (Mathf.Approximately(xzLocal.x, -TableSpace.lossyScale.x * 0.5f))
                EdgePointerTransform.LookAt(EdgePointerTransform.position + Vector3.right);
            else if (Mathf.Approximately(xzLocal.x, TableSpace.lossyScale.x * 0.5f))
                EdgePointerTransform.LookAt(EdgePointerTransform.position + Vector3.left);
            else if (Mathf.Approximately(xzLocal.y, TableSpace.lossyScale.y * 0.5f))
                EdgePointerTransform.LookAt(EdgePointerTransform.position + Vector3.forward);
            else if (Mathf.Approximately(xzLocal.y, -TableSpace.lossyScale.y * 0.5f))
                EdgePointerTransform.LookAt(EdgePointerTransform.position + Vector3.back);
            else
                EdgePointerTransform.LookAt(transform.TransformPoint(CurrentHitPosition).ToXZ().ToVector3FromXZ(EdgePointerTransform.position.y));
        }

        private void SetActiveLine(bool isActive)
        {
            EdgePointerTransform.gameObject.SetActive(isActive);
            RayHitPointerTransform.gameObject.SetActive(isActive);
            lineRenderer.enabled = isActive;
        }

        private void OnDestroy()
        {
            using (var e = cached.GetEnumerator())
            {
                while (e.MoveNext())
                {
                    CacheManager.Remove<XRController>(e.Current);
                }
            }

            cached.Clear();
        }
    }

}