using UnityEngine;
using System.Collections;

namespace Billiards
{
    using BallPool;
    public class FollowPosition : MonoBehaviour
    {
        [SerializeField]
        private Transform Target;

        public Vector3 TargetPosition { get => Target.position; }

        private void FixedUpdate()
        {
            transform.position = Target.position;
        }

        public void ForceUpdate()
        {
            transform.position = Target.position;
        }
    }
}