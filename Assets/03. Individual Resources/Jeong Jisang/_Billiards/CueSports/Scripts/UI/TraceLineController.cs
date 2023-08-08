using BallPool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Billiards
{
    public class TraceLineController : MonoBehaviour
    {
        [SerializeField]
        private LineRenderer lineRenderer;

        private bool isSnap { get => BilliardsDataContainer.Instance.CueSnapState.Value; }
        private bool isMyTurn { get => BallPoolPlayer.mainPlayer.myTurn; }

        private bool isEnabled { get => isSnap && isMyTurn; }

        private void Awake()
        {
            lineRenderer.positionCount = 0;
            lineRenderer.enabled = false;

            BallPoolPlayer.OnTurnChanged += OnStateChanged;
            BilliardsDataContainer.Instance.CueSnapState.OnChanged += OnStateChanged;
        }

        private void OnStateChanged()
        {
            if (isEnabled == false)
                lineRenderer.positionCount = 0;

            lineRenderer.enabled = isEnabled;
        }


        private void OnDestroy()
        {
            BilliardsDataContainer.Instance.CueSnapState.OnChanged -= OnStateChanged;
            BallPoolPlayer.OnTurnChanged -= OnStateChanged;
        }
    }

}