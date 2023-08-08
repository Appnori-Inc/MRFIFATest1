using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TestAnim : MonoBehaviour
{
    [SerializeField] private Image me;
    [SerializeField] private bool isPoint;
   
  

    // Start is called before the first frame update
    void Start()
    {
        PointAnim(me);
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void PointAnim(Image target)
    {
        //point_my
        //point_other

        // 현재 점수 받아서 이전 점수 보다 많은지 적은지 체크 하고
        // 많으면 왼쪽 칩을 오른쪽으로 적으면 오른쪽칩을 왼쪽으로

        // 왼쪽 자신 
        // x -121.5 -> - 365
        // -243.5
        // 한칸 +10


        // 오른쪽 다른 사람
        // x 417.5 -> 174
        // -243.5
        // 한칸 +10

        StartCoroutine(update());

        IEnumerator update()
        {
            float initx = target.rectTransform.anchoredPosition.x;
            float addx = 0;

            while (true)
            {
                if (isPoint && target.rectTransform.anchoredPosition.x - initx <= -243.5f)
                {
                    Debug.Log("End");
                    yield break;
                }
                if (!isPoint && target.rectTransform.anchoredPosition.x - initx >= 243.5f)
                {
                    Debug.Log("End");
                    yield break;
                }

                if (isPoint) addx -= Time.deltaTime;
                else addx += Time.deltaTime;

                Vector2 addVec = new Vector2(target.rectTransform.anchoredPosition.x + addx, 0);

                target.rectTransform.anchoredPosition = addVec;

                yield return null;
            }
        }
    }
}
