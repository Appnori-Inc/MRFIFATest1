using Meta.Numerics.Analysis;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Billiards;
using Appnori.Util;

public class SobolTest : MonoBehaviour
{
    [SerializeField]
    private int SampleCount = 100;
    

    // Start is called before the first frame update
    void OnEnable()
    {
       StartCoroutine( CaromResetBall());
    }

    public IEnumerator CaromResetBall()
    {
        yield return new WaitForSeconds(1f);

        var d = 2;
        var seq = new SobolSequence[d];
        for (int i = 0; i < d; ++i)
        {
            var p = SobolSequenceParameters.sobolParameters[i];
            seq[i] = new SobolSequence(p.Dimension, p.Coefficients, p.Seeds);
        }

        var skip = Random.Range(100, 300);

        for (int i = 0; i < skip + SampleCount; ++i)
        {
            var vec = new Vector2((float)seq[0].Current, (float)seq[1].Current);


            Debug.Log(i + " : " + vec);

            seq[0].MoveNext();
            seq[1].MoveNext();

            if (i < skip)
                continue;

            Spawn(i, vec);

            yield return null;
            yield return null;
            yield return null;
        }
    }

    public void Spawn(int i,in Vector2 pos)
    {
        var instance = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        instance.transform.position = pos.ToVector3FromXZ();
        instance.name = i.ToString();
        instance.transform.localScale = Vector3.one * 0.04f;
    }


    public void Generate(in int count, out List<Vector2> values)
    {
        values = new List<Vector2>();

        var d = 2;
        var seq = new SobolSequence[d];
        for (int i = 0; i < d; ++i)
        {
            var p = SobolSequenceParameters.sobolParameters[i];
            seq[i] = new SobolSequence(p.Dimension, p.Coefficients, p.Seeds);
        }

        var skip = Random.Range(50, 120);

        for (int i = 0; i < skip + count; ++i)
        {
            seq[0].MoveNext();
            seq[1].MoveNext();

            if (i < skip)
                continue;

            values.Add(new Vector2((float)seq[0].Current, (float)seq[1].Current));
        }
    }
}
