using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class InGameUI : MonoBehaviour
{
    public GameObject laserStart, mirror1;

    Camera cam;

    bool isMouseFull;
    GameObject mouseObject;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;

        isMouseFull= false;
        mouseObject = null;
    }

    // Update is called once per frame
    void Update()
    {
        if (isMouseFull)
        {
            UpdateObjPosToMouse();

            if (Input.GetMouseButtonDown(0))
            {
                Vector3 mousePoint = GetMousePos();

                mouseObject.transform.position = mousePoint;

                isMouseFull = false;
            }
        }
    }

    private void UpdateObjPosToMouse()
    {
        Vector3 mousePoint = GetMousePos();

        mouseObject.transform.position = mousePoint;
    }

    private Vector3 GetMousePos()
    {
        Vector3 mousePos = Vector3.one;
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            mousePos = hit.point;
        }

        mousePos.y = 0.33f;

        return mousePos;
    }

    public void Button1()
    {
        if (!isMouseFull)
        {
            Vector3 point = GetMousePos();

            mouseObject = Instantiate(laserStart, point, Quaternion.identity);

            isMouseFull= true;
        }
    }

    public void Button2()
    {
        if (!isMouseFull)
        {
            Vector3 point = GetMousePos();

            mouseObject = Instantiate(mirror1, point, Quaternion.identity);

            isMouseFull = true;
        }
    }
}
