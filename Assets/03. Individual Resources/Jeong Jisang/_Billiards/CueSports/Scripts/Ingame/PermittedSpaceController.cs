using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.XR.CoreUtils;

namespace Billiards
{
    public class PermittedSpaceController : MonoBehaviour
    {
        [Tooltip("if autoSimulate is false, use manual mode")]
        [SerializeField]
        private bool isManualMode;

        [SerializeField]
        private XROrigin xrRig;

        [SerializeField]
        private Collider self;

        [SerializeField]
        private List<Collider> Targets;

        [SerializeField]
        private Material TargetMaterial;

        private Queue<Vector3> lastPositions = new Queue<Vector3>();
        public bool isEnter { get; private set; }

        private List<Collider> EnteredColliders = new List<Collider>();

        public event Action<(Collider, Vector3, float)> OnEnableCheckedCollider;

        private CoroutineWrapper MoveCameraRoutine;
        private CoroutineWrapper MaterialAnimationRoutine;

        private void Awake()
        {
            MoveCameraRoutine = CoroutineWrapper.Generate(this);
            MaterialAnimationRoutine = CoroutineWrapper.Generate(this);

            TargetMaterial.SetFloat("_Distance", 0);
        }

        /// <summary>
        /// if already exist, invoke event
        /// </summary>
        private void OnEnable()
        {
            List<(Collider, Vector3, float)> hits = new List<(Collider, Vector3, float)>();
            foreach (var target in Targets)
            {
                if (Physics.ComputePenetration(self, self.transform.position, self.transform.rotation, target, target.transform.position, target.transform.rotation, out var dir, out var dis))
                {
                    hits.Add((target, dir, dis));
                }
            }

            foreach (var hit in hits)
            {
                OnEnableCheckedCollider?.Invoke(hit);
            }

            //clear and init position
            lastPositions.Clear();
            lastPositions.Enqueue(xrRig.Camera.transform.position);
        }

        private void OnDisable()
        {
            TargetMaterial.SetFloat("_Distance", 0);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!enabled)
                return;

            if (!Targets.Contains(other))
                return;

            isEnter = true;

            MaterialAnimationRoutine.StartSingleton(FielActive(0.6f));

            IEnumerator FielActive(float runtime)
            {
                float t = 0;
                while (t < runtime)
                {
                    t += Time.fixedDeltaTime;
                    TargetMaterial.SetFloat("_Distance", 1 - (t / runtime));
                    yield return new WaitForFixedUpdate();
                }

                TargetMaterial.SetFloat("_Distance", 0);
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (!enabled)
                return;

            if (!Targets.Contains(other))
                return;

            var delta = lastPositions.Peek() - xrRig.Camera.transform.position;

            MoveCameraRoutine.StartSingleton(MoveCamera(xrRig.Camera.transform.position + delta * 1.16f, 0.2f));

            IEnumerator MoveCamera(Vector3 target, float runtime)
            {
                float t = 0;
                Vector3 defaultPosition = xrRig.Camera.transform.position;
                while (t < runtime)
                {
                    t += Time.fixedDeltaTime;
                    xrRig.MoveCameraToWorldLocation(Vector3.Lerp(defaultPosition, target, t / runtime).ToXZ().ToVector3FromXZ(xrRig.Camera.transform.position.y));
                    yield return new WaitForFixedUpdate();
                }

                xrRig.MoveCameraToWorldLocation(Vector3.Lerp(defaultPosition, target, 1).ToXZ().ToVector3FromXZ(xrRig.Camera.transform.position.y));
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (!enabled)
                return;

            if (!Targets.Contains(other))
                return;

            isEnter = false;
        }

        //For manual operation
        private void Update()
        {
            if (!isManualMode)
                return;

            //checkCollision
            foreach (var target in Targets)
            {
                if (Physics.ComputePenetration(self, self.transform.position, self.transform.rotation, target, target.transform.position, target.transform.rotation, out var dir, out var dis))
                {
                    if (EnteredColliders.Contains(target))
                    {
                        OnTriggerStay(target);
                    }
                    else
                    {
                        EnteredColliders.Add(target);
                        OnTriggerEnter(target);
                    }
                }
                else
                {
                    if (EnteredColliders.Contains(target))
                    {
                        EnteredColliders.Remove(target);
                        OnTriggerExit(target);
                    }
                }
            }

            //.tracking
            if (!isEnter)
            {
                lastPositions.Enqueue(xrRig.Camera.transform.position);
                if (lastPositions.Count > 4)
                {
                    lastPositions.Dequeue();
                }
            }
        }

    }

}