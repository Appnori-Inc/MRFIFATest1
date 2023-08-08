using UnityEngine;
using System.Collections;

namespace Appnori
{

    public class SetLayerCollisionMatrix : MonoBehaviour
    {
        [SerializeField]
        private Layer.GameType type;

        private void Awake()
        {
            Layer.SetIgnoreLayer(type);
        }
    }
}