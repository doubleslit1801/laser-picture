using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ObjectControl : MonoBehaviour
{
    public GameObject canvasObj;


    [HideInInspector] public GameObject mouseObject, selectedObj;
    [HideInInspector] public enum mouseState { Grab, Select, Neutral };
    [HideInInspector] public mouseState curMouseState;
    [HideInInspector] public bool isPause, isInit;

    private bool isMouseObjMovable, isMouseRightButtonDown;
    private Camera cam;
    private Vector3 initObjAngle, initMouseVector;
    private float rotateBoundDist, objHeight;

    void Start()
    {
        cam = Camera.main;

        Init();

        rotateBoundDist = 100.0f;

        objHeight = 0.0f;
    }
    
    void Update()
    {
        if (isPause && !isInit)
        {
            Init();
            isInit = true;
        }
        else
        {
            if (mouseObject != null)
            {
                if (isMouseObjMovable)
                {
                    MoveObjToMouse();

                    if (Input.GetMouseButtonUp(0) && !EventSystem.current.IsPointerOverGameObject())
                    {
                        PlaceObjAtMouse();
                    }

                    GameObject tmpObj = canvasObj.GetComponent<InGameUI>().GetUIObjUnderMouse();

                    if (Input.GetMouseButtonUp(0) && tmpObj != null && tmpObj.name == "Trashcan")
                    {
                        DestroyMouseObj();
                    }
                }

                if (Input.GetKeyDown(KeyCode.Delete))
                {
                    DestroyMouseObj();
                }
            }
            else if (selectedObj != null)
            {
                if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
                {
                    SelectObj();
                }
                if (selectedObj != null)
                {
                    RotateObjWithMouse();
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
    }

    private void Init()
    {
        mouseObject = null;
        selectedObj = null;
        isMouseObjMovable = false;
        isMouseRightButtonDown = false;
        isPause = false;

        initObjAngle = Vector3.zero;
        initMouseVector = Vector3.zero;

        curMouseState = mouseState.Neutral;
    }

    private GameObject GetObjUnderMouse() //마우스 아래 월드 오브젝트 반환
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits;

        hits = Physics.RaycastAll(ray);
        for (int i = 0; i < hits.Length; i++)
        {
            GameObject hitObj = hits[i].transform.gameObject;
            Vector2 diff = new Vector2(hitObj.transform.position.x - hits[i].point.x, hitObj.transform.position.z - hits[i].point.z);

            if (hitObj.CompareTag("Object") && diff.magnitude < 1)
            {
                return hitObj;
            }
        }

        return null;
    }

    private void MoveObjToMouse() //선택한 월드 오브젝트 마우스 위치로 이동.
    {
        mouseObject.transform.position = GetMousePos();
    }

    private void PlaceObjAtMouse() //선택한 월드 오브젝트 마우스 위치에 배치.
    {
        mouseObject.transform.position = GetMousePos();

        mouseObject = null;

        isMouseObjMovable = false;

        curMouseState = mouseState.Select;
    }

    private void SelectObj() //마우스에 위치한 월드 오브젝트 선택.
    {
        selectedObj = null;

        GameObject obj = GetObjUnderMouse();

        if (obj != null)
        {
            selectedObj = obj;

            curMouseState = mouseState.Select;
        }
        else
        {
            curMouseState = mouseState.Neutral;
        }
    }

    private void RotateObjWithMouse()
    {
        Vector3 mousePos = GetMousePos();
        Vector3 objPos = selectedObj.transform.position;
        Vector3 curMouseVector = new Vector3(mousePos.x - objPos.x, 0, mousePos.z - objPos.z);

        Vector2 mouseUIPos = Input.mousePosition;
        Vector2 objUIPos = cam.WorldToScreenPoint(objPos + new Vector3(0, 0.5f, 0));
        Vector2 curMouseUIV = mouseUIPos - objUIPos;

        if (Input.GetMouseButtonDown(1) && curMouseUIV.magnitude < rotateBoundDist)
        {
            isMouseRightButtonDown = true;
            initObjAngle = selectedObj.transform.eulerAngles;
            initMouseVector = new Vector3(mousePos.x - objPos.x, 0, mousePos.z - objPos.z);
        }

        if (isMouseRightButtonDown)
        {
            float setAngle = Quaternion.FromToRotation(initMouseVector, curMouseVector).eulerAngles.y + initObjAngle.y;

            SetRotationMouseObj(setAngle);
        }

        if (Input.GetMouseButtonUp(1))
        {
            isMouseRightButtonDown = false;
            initObjAngle = Vector3.zero;
            initMouseVector = Vector3.zero;
        }
    }

    private void SetRotationMouseObj(float angle) //선택된 월드 오브젝트 회전.
    {
        if (selectedObj != null)
        {
            selectedObj.transform.rotation = Quaternion.Euler(0, angle, 0);
        }
    }

    //====================================PUBLIC==================================

    public Vector3 GetMousePos() //마우스가 가리키는 월드 위치 반환.
    {
        Vector3 hitPos = Vector3.one;
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits;

        hits = Physics.RaycastAll(ray);
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].transform.gameObject.CompareTag("Plane"))
            {
                hitPos = new Vector3(hits[i].point.x, objHeight, hits[i].point.z);
            }
        }

        return hitPos;
    }

    public void InstantiateObj(GameObject obj, float height)
    {
        if (mouseObject != null)
        {
            Destroy(mouseObject);
            mouseObject = null;
        }

        if (selectedObj != null)
        {
            selectedObj = null;
            isMouseObjMovable = false;
        }

        objHeight = height;

        Vector3 point = GetMousePos();

        mouseObject = Instantiate(obj, point, Quaternion.identity);

        isMouseObjMovable = true;

        selectedObj = mouseObject;

        curMouseState = mouseState.Grab;
    }

    public void DestroyMouseObj() //선택된 월드 오브젝트 삭제.
    {
        if (mouseObject != null)
        {
            Destroy(mouseObject);
            mouseObject = null;
        }

        if (selectedObj != null)
        {
            Destroy(selectedObj);
            selectedObj = null;
            isMouseObjMovable = false;
        }

        curMouseState = mouseState.Neutral;
    }

    public void GrabSelectedObj()
    {
        if (selectedObj != null)
        {
            mouseObject = selectedObj;
            isMouseObjMovable = true;

            curMouseState = mouseState.Grab;
        }
    }
}
