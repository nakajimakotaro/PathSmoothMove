using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CinemachineMove : MonoBehaviour
{
    [SerializeField] CinemachinePath Path;

    void Update()
    {
        var t = Time.time * Path.PathLength / 2;
        transform.position = Path.EvaluatePositionAtUnit(t, CinemachinePathBase.PositionUnits.Distance);
    }
}
