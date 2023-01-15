using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage : MonoBehaviour
{
    [SerializeField]
    private int stageNumber;
    [SerializeField]
    private LineRenderer lr;

    private Vector3[] drawing;

    void Start()
    {
        drawing = GameManager.Instance.GetDrawing(stageNumber);
        lr.SetPositions(drawing);
    }

    void Update()
    {
        
    }
}
