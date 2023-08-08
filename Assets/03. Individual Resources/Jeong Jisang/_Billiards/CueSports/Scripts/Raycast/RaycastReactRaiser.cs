using UnityEngine;
using System.Collections;


namespace Billiards
{

    public class RaycastReactRaiser : BaseRaycastReactor
    {
        [SerializeField]
        private BaseRaycastReactor Target;

        public override void OnRay(RaycastHit hitInfo)
        {
            Target?.OnRay(hitInfo);
        }

        public override void OnRayEnd(RaycastHit hitInfo)
        {
            Target?.OnRayEnd(hitInfo);
        }

        public override bool RaycastReact(RaycastHit hitInfo)
        {
            if (Target != null)
            {
                return Target.RaycastReact(hitInfo);
            }

            return false;
        }
    }
}