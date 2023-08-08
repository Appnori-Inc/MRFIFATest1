using Appnori.Util;
using BallPool;
using BallPool.Mechanics;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBoardMarker : MonoBehaviour
{
    [SerializeField]
    private AnimationCurve curve;

    [SerializeField]
    private GameObject Marker;

    [SerializeField]
    private Vector2 HeightRange;

    [SerializeField]
    private float runtime;

    [SerializeField]
    private PhysicsManager physicsManager;

    private readonly CoroutineWrapper wrapper;

    private readonly Notifier<bool> ActiveState = new Notifier<bool>();

    public UIBoardMarker()
    {
        wrapper = new CoroutineWrapper(this);
    }

    private void Awake()
    {
        Marker.transform.localPosition = new Vector3(0, HeightRange.x, 0);
        Marker.SetActive(false);
        ActiveState.Value = false;

        ActiveState.OnDataChanged += ActiveState_OnDataChanged;
    }


    private void OnEnable()
    {
        BallListener.OnBoardHit += BallListener_OnBoardHit;
        physicsManager.OnEndShot += PhysicsManager_OnEndShot;
    }


    private void BallListener_OnBoardHit(BallListener ball, GameObject obj)
    {
        if (obj != gameObject)
            return;

        if (!CaromGameLogic.isCueBall(ball.id))
            return;

        ActiveState.Value = true;
    }

    private void ActiveState_OnDataChanged(bool active)
    {
        if (active)
        {
            ActivateMarker();
        }
        else
        {
            DeactivateMarker();
        }

    }

    private void ActivateMarker()
    {
        wrapper.StartSingleton(Activate());

        IEnumerator Activate()
        {
            var defaultHeight = Marker.transform.localPosition.y;
            Marker.SetActive(true);

            float t = 0;
            while(t < runtime)
            {
                Marker.transform.localPosition = new Vector3(0, Mathf.Lerp(defaultHeight, HeightRange.y, curve.Evaluate(t / runtime)), 0);
                t += Time.deltaTime;
                yield return null;
            }
            Marker.transform.localPosition = new Vector3(0, Mathf.Lerp(defaultHeight, HeightRange.y, curve.Evaluate(1)), 0);
        }
    }


    private void PhysicsManager_OnEndShot(string _)
    {
        ActiveState.Value = false;
    }

    private void DeactivateMarker()
    {
        wrapper.StartSingleton(Deactivate());

        IEnumerator Deactivate()
        {
            var defaultHeight = Marker.transform.position.y;

            float t = 0;
            while (t < runtime)
            {
                Marker.transform.localPosition = new Vector3(0, Mathf.Lerp(defaultHeight, HeightRange.x, curve.Evaluate(t / runtime)), 0);
                t += Time.deltaTime;
                yield return null;
            }

            Marker.transform.localPosition = new Vector3(0, Mathf.Lerp(defaultHeight, HeightRange.x, curve.Evaluate(1)), 0);
            Marker.SetActive(false);
        }
    }


    private void OnDisable()
    {
        physicsManager.OnEndShot -= PhysicsManager_OnEndShot;
        BallListener.OnBoardHit -= BallListener_OnBoardHit;
    }
}
