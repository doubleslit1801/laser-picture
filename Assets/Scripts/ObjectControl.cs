using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class ObjectControl : MonoBehaviour
{
    public GameObject canvasObj;
    public float arrowKeyMoveRate;

    [HideInInspector] public GameObject mouseObject, selectedObj;
    [HideInInspector] public enum mouseState { Grab, Select, Neutral };
    [HideInInspector] public mouseState curMouseState;
    [HideInInspector] public bool isPause, isInit;

    private bool isMouseObjMovable, isMouseRightButtonDown;
    private Camera cam;
    private Vector3 initObjAngle, initMouseVector;
    private float rotateBoundDist, objHeight;
    private bool isPlayed = false;

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

                if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    selectedObj.transform.position += arrowKeyMoveRate * Vector3.right;
                    canvasObj.GetComponent<InGameUI>().RepopControlUI();
                }
                else if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    selectedObj.transform.position += arrowKeyMoveRate * Vector3.left;
                    canvasObj.GetComponent<InGameUI>().RepopControlUI();
                }
                else if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    selectedObj.transform.position += arrowKeyMoveRate * Vector3.back;
                    canvasObj.GetComponent<InGameUI>().RepopControlUI();
                }
                else if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    selectedObj.transform.position += arrowKeyMoveRate * Vector3.forward;
                    canvasObj.GetComponent<InGameUI>().RepopControlUI();
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

            if (SettingData.Instance.arrowMoveRate != arrowKeyMoveRate)
            {
                arrowKeyMoveRate = SettingData.Instance.arrowMoveRate;
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

            if (hitObj.CompareTag("Object"))
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
            if (obj.name == "LaserStart(Clone)" || obj.name == "Blackhole(Clone)" || obj.name.Contains("Mirror"))
            {
                objHeight = 0.5f;
            }
            else
            {
                objHeight = 0.0f;
            }
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
            float angleDiff = Quaternion.FromToRotation(initMouseVector, curMouseVector).eulerAngles.y;
            float setAngle = angleDiff + initObjAngle.y;

            SetRotationMouseObj(setAngle);
            PlayClickSound(angleDiff);
        }

        if (Input.GetMouseButtonUp(1))
        {
            isMouseRightButtonDown = false;
            initObjAngle = Vector3.zero;
            initMouseVector = Vector3.zero;
        }
    }

    private void PlayClickSound(float angleDiff)
    {
        switch ((int)angleDiff % 7)
        {
            case 0:
                if (!isPlayed)
                {
                    SoundManager.Instance.PlaySFXSound("Click_new", 0.7f);
                    isPlayed = true;
                }
                break;
            default:
                isPlayed = false;
                break;
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

    //임시 테스트용
    public void ColorLaser(Color color)
    {
        if (mouseObject != null)
        {
            mouseObject.GetComponent<ColorLaser>().outputColor = color;
        }
    }

    public void DestroyMouseObj() //선택된 월드 오브젝트 삭제.
    {
        if (mouseObject != null || selectedObj != null)
        {
            SoundManager.Instance.PlaySFXSound("Trash", 0.7f);
        }

        if (mouseObject != null)
        {
            DisableObject(mouseObject);
            Destroy(mouseObject);
            mouseObject = null;
        }

        if (selectedObj != null)
        {
            DisableObject(selectedObj);
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

    void DisableObject(GameObject obj)
    {
        if (obj.name == "LaserStart(Clone)")
        {
            obj.GetComponent<ColorLaser>()?.DestroyAll();
            obj.GetComponent<Laser>()?.DestroyAll();
        }
        else if (obj.name == "Prism(Clone)")
        {
            foreach (Transform child in obj.transform)
            {
                child.GetComponent<Prism>()?.DestroyAll();
            }
        }
        else if (obj.name == "Composer(Clone)")
        {
            obj.GetComponent<Composer>().DestroyAll();
        }
        else if (obj.name.Contains("Mirror"))
        {
            obj.GetComponent<Mirror>().DestroyAll();
        }
    }
}
