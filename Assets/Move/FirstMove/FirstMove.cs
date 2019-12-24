using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstMove : MonoBehaviour
{
    
    [SerializeField] PathContainer Paths;

    void Start()
    {
        var lineRenderer = new GameObject("LineRenderer", typeof(LineRenderer)).GetComponent<LineRenderer>();
        lineRenderer.positionCount = 160;
        lineRenderer.material = new Material(Shader.Find("Hidden/Internal-Colored"));
        lineRenderer.endWidth = lineRenderer.startWidth = 0.03f;

        for (int i = 0; i < 160; i++)
        {
            lineRenderer.SetPosition(i, CalcPos(i*2f/160));
        }
    }

    void Update()
    {
        var t = Time.time;
        var indexA = Mathf.FloorToInt(Mathf.Min(Paths.Length - 1, t));
        var indexB = Mathf.FloorToInt(Mathf.Min(Paths.Length - 1, indexA+1));

        if (indexA == indexB) return;

        transform.position = CalcPos(t);
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
