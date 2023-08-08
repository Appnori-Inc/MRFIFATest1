using UnityEngine;
using System.Collections;
using BallPool.Mechanics;
using System.Collections.Generic;

namespace Billiards
{

    public class GeometryTest : MonoBehaviour
    {
        [SerializeField]
        private Transform SimulateSpace;

        [SerializeField]
        private Transform Pivot;

        [SerializeField]
        private Transform Target;

        [SerializeField]
        private Transform CalculatedMarker;

        [SerializeField]
        private List<Transform> markers;
        
        // Update is called once per frame
        void Update()
        {
            Debug.DrawRay(Pivot.position, (Target.position - Pivot.position) * 10f, Color.magenta, Time.deltaTime);
            CalculatedMarker.transform.position = Geometry.EdgeProjectionXZ((Target.position - Pivot.position).ToXZ(), Pivot.position.ToXZ(), SimulateSpace/*, out var vector2s*/).ToVector3FromXZ();

            //for (int i = 0; i < vector2s.Count && i < markers.Count; ++i)
            //{
            //    markers[i].position = vector2s[i].ToVector3FromXZ();
            //}
        }
    }

}