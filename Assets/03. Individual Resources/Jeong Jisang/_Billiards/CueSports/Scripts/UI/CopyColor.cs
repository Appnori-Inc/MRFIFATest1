using UnityEngine;
using System.Collections;

namespace Billiards
{
    using UnityEngine.UI;

    public class CopyColor : MonoBehaviour
    {
        [SerializeField]
        private Graphic Origin;

        [SerializeField]
        private Graphic Destination;

        void Update()
        {
            Destination.color = Origin.color;
        }
    }
}