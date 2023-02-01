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

    public void RotateButtonL()
    {
        canvas.GetComponent<InGameUI>().RotateMouseObj(-45);
    }

    public void RotateButtonR()
    {
        canvas.GetComponent<InGameUI>().RotateMouseObj(45);
    }

    public void GrabButton()
    {
        canvas.GetComponent<InGameUI>().GrabSelectedObj();
    }
}
