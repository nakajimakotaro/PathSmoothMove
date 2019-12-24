using UnityEngine;
using UnityEngine.Assertions;

public class MyMove : MonoBehaviour
{
    [SerializeField] int Segment;
    [SerializeField] PathContainer Paths;

    float PathLength;
    
    //セグメントの総数
    int NumKeys;
    float[] DistanceToTArray;
    float DistanceStepSize;

    void Start()
    {
        Build();
        var lineRenderer = new GameObject("LineRenderer", typeof(LineRenderer)).GetComponent<LineRenderer>();
        lineRenderer.positionCount = NumKeys;
        lineRenderer.material = new Material(Shader.Find("Hidden/Internal-Colored"));
        lineRenderer.endWidth = lineRenderer.startWidth = 0.03f;

        for (int i = 0; i < NumKeys; i++)
        {
            lineRenderer.SetPosition(i, CalcPos(DistanceToT(i*DistanceStepSize)));
        }
    }

    void Build()
    {
        Assert.IsTrue(Paths.Length>0);
        PathLength = 0;
        NumKeys = (Paths.Length-1) * Segment+1;

        var tToDistance = CalcTToDistance();
        DistanceToTArray = CalcDistanceToT(tToDistance);
    }

    void Update()
    {
        transform.position = CalcPos(DistanceToT(Time.time*PathLength/2));
    }

    float DistanceToT(float distance)
    {
        float d = distance / DistanceStepSize;
        int index = Mathf.FloorToInt(d);
        if(index>=DistanceToTArray.Length-1)return DistanceToTArray[DistanceToTArray.Length-1];
        float t = d - index;
        return Mathf.Lerp(DistanceToTArray[index], DistanceToTArray[index+1], t);
    }

    float[] CalcTToDistance()
    {
        var tToDistance = new float[NumKeys];
        
        var pp = Paths[0].Pos;
        float t = 0;
        for (int n = 1; n < NumKeys; n++)
        {
            t += 1f / Segment;
            Vector3 p = CalcPos(t);
            float d = Vector3.Distance(pp, p);
            PathLength += d;
            pp = p;
            tToDistance[n] = PathLength;
        }

        return tToDistance;
    }

    float[] CalcDistanceToT(float[] tToDistance)
    {
        var distanceToT = new float[NumKeys];
        distanceToT[0] = 0;
        DistanceStepSize = PathLength/(NumKeys-1);
        float distance = 0;
        int tIndex=1;
        for (int i = 1; i < NumKeys; i++)
        {
            distance += DistanceStepSize;
            var d = tToDistance[tIndex];
            while (d < distance && tIndex < NumKeys - 1)
            {
                tIndex++;
                d = tToDistance[tIndex];
            }

            var prevD = tToDistance[tIndex - 1];
            float delta = d - prevD;
            float t = (distance - prevD) / delta;
            distanceToT[i] = (1f/Segment)*(t + tIndex - 1);
        }

        return distanceToT;
    }

    Vector3 CalcPos(float t)
    {
        var indexA = Mathf.FloorToInt(Mathf.Min(Paths.Length - 1, t));
        var indexB = Mathf.FloorToInt(Mathf.Min(Paths.Length - 1, indexA+1));
        return Bezier3(
            t-indexA,
            Paths[indexA].Pos,
            Paths[indexA].Pos + Paths[indexA].Tangent,
            Paths[indexB].Pos - Paths[indexB].Tangent,
            Paths[indexB].Pos);
    }
    
    Vector3 Bezier3(float t, Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4)
    {
        var d = 1 - t;
        return     d * d * d * p1 +
                   3 * d * d * t * p2 +
                   3 * d * t * t * p3 +
                   t * t * t * p4;
    }
}
