using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopControl : MonoBehaviour
{
    private GameObject canvas;

    void Start()
    {
        canvas = GameObject.Find("Canvas");
    }

    public void GrabButtonDown()
    {
        if (Input.GetMouseButtonDown(0))
        {
            canvas.GetComponent<InGameUI>().GrabSelectedObj();
        }
    }
}
