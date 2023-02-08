using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopControl : MonoBehaviour
{
    private ObjectControl objControl;

    void Start()
    {
        objControl = GameObject.Find("ObjectController").GetComponent<ObjectControl>();
    }

    public void GrabButtonDown()
    {
        if (Input.GetMouseButtonDown(0))
        {
            objControl.GrabSelectedObj();
        }
    }
}
