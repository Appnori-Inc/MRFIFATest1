using UnityEngine;
using System.Collections;

namespace Billiards
{
    public class AlwaysIdentity : MonoBehaviour
    {
        private void FixedUpdate()
        {
            transform.rotation = Quaternion.identity;
        }
    }
}