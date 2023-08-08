using UnityEngine;
using System.Collections;

namespace Billiards
{
    using UnityEngine.UI;

    [RequireComponent(typeof(Text))]
    public class BuildTestText : MonoBehaviour
    {
        [SerializeField]
        private Text text;

        [SerializeField]
        private string desc;

        private void Awake()
        {
            text.text = desc + 10;
        }
    }
}