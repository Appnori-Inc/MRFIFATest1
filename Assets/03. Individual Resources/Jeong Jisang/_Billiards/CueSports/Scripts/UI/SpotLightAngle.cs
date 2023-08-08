using UnityEngine;
using System.Collections;

namespace Billiards
{

    public class SpotLightAngle : MonoBehaviour
    {
        [SerializeField]
        private Transform TargetPoint;
        [SerializeField]
        private Light SpotLight;

        private void Update()
        {
            var dist = (TargetPoint.position - SpotLight.transform.position).magnitude;
            SpotLight.range = dist + 0.02f;
            SpotLight.spotAngle = Mathf.Asin(GameConfig.HitPositionMarkerRadius / dist) * Mathf.Rad2Deg;
        }

    }

}