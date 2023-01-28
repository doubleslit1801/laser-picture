using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.EventSystems;

public class InGameUI : MonoBehaviour
{
    public GameObject laserStart, mirror1, objSelPanel;

    Camera cam;

    bool isHide;
    GameObject mouseObject;

    public Sprite openTrash, closeTrash;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;

        mouseObject = null;
        isHide = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (mouseObject != null)
        {
            MoveObjToMouse();

            UpdateSelObjUI();

            if(Input.GetAxis("Mouse ScrollWheel") != 0)
            {
                RotateMouseObj();
            }

            if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                PlaceObjAtMouse();
            }

            if (Input.GetKeyDown(KeyCode.Delete))
            {
                DestroyMouseObj();
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                SelectObj();
            }
        }
        
    }

    public void OnPointerUp()
    {

    }

    private void MoveObjToMouse()
    {
        mouseObject.transform.position = GetMousePos();
    }

    private void PlaceObjAtMouse()
    {
        mouseObject.transform.position = GetMousePos();

        mouseObject = null;
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

    private void UpdateSelObjUI()
    {

    }

    private void SelectObj()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit) && !hit.transform.gameObject.CompareTag("Plane"))
        {
            mouseObject = hit.transform.gameObject;
        }
    }

    private void RotateMouseObj()
    {
        float rotSpeed = 5000f;

        if (mouseObject != null)
        {
            mouseObject.transform.Rotate(new Vector3(0, Input.GetAxis("Mouse ScrollWheel") * rotSpeed * Time.deltaTime, 0));
        }
    }

    public void PopHideButton()
    {
        RectTransform rectT = objSelPanel.GetComponent<RectTransform>();
        if (isHide)
        {
            rectT.anchoredPosition = new Vector3(0, 0, 0);
            isHide = false;
        }
        else
        {
            rectT.anchoredPosition = new Vector3(-rectT.sizeDelta.x, 0, 0);
            isHide = true;
        }
    }

    public void Button1()
    {
        DestroyMouseObj();

        Vector3 point = GetMousePos();

        mouseObject = Instantiate(laserStart, point, Quaternion.identity);
    }

    public void Button2()
    {
        DestroyMouseObj();

        Vector3 point = GetMousePos();

        mouseObject = Instantiate(mirror1, point, Quaternion.identity);
    }

    public void DestroyMouseObj()
    {
        if(mouseObject != null)
        {
            Destroy(mouseObject);
            mouseObject = null;
        }
    }
}
