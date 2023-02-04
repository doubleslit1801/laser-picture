using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InGameUI : MonoBehaviour
{
    public GameObject laserStart, oneSideMirror, doubleSideMirror, objSelPanel, trashCan, rotateButtonL, rotateButtonR, grabButton;
    public Sprite openTrash, closeTrash;

    private Camera cam;

    private bool isHide, isMouseObjMovable, isMouseRightButtonDown;

    private GameObject mouseObject, selectedObj, preMouseUIObj;

    private Canvas m_canvas;
    private GraphicRaycaster m_gr;
    private PointerEventData m_ped;

    private Transform infoBoxSetTR;

    private List<GameObject> objControlButtonLst;

    private Vector3 initObjAngle, initMouseVector;

    void Start()
    {
        cam = Camera.main;

        mouseObject = null;
        selectedObj = null;
        preMouseUIObj = null;

        isHide = false;
        isMouseObjMovable = false;
        isMouseRightButtonDown = false;

        m_canvas = gameObject.GetComponent<Canvas>();
        m_gr = m_canvas.GetComponent<GraphicRaycaster>();
        m_ped = new PointerEventData(null);

        objControlButtonLst = new List<GameObject>();

        initObjAngle = Vector3.zero;
        initMouseVector = Vector3.zero;

        infoBoxSetTR = objSelPanel.transform.Find("InfoBoxSet");

        for (int i = 0; i < infoBoxSetTR.childCount; i++)
        {
            Transform tr = infoBoxSetTR.GetChild(i);
            if (tr != null)
            {
                tr.gameObject.SetActive(false);
            }
        }
    }

    void Update()
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
            }

            if (Input.GetKeyDown(KeyCode.Delete))
            {
                DestroyMouseObj();
            }
        }
        else if(selectedObj != null)
        {
            if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                SelectObj();
            }

            RotateObjWithMouse();

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

        UIOverMouseEffect();
    }

    private void RotateObjWithMouse()
    {
        if (Input.GetMouseButtonDown(1) && (GetObjUnderMouse() == selectedObj))
        {
            isMouseRightButtonDown = true;
            initObjAngle = selectedObj.transform.eulerAngles;

            Vector3 mousePos = GetMousePos();
            Vector3 objPos = selectedObj.transform.position;
            initMouseVector = new Vector3(mousePos.x - objPos.x, 0, mousePos.z - objPos.z);
        }

        if (isMouseRightButtonDown)
        {
            Vector3 mousePos = GetMousePos();
            Vector3 objPos = selectedObj.transform.position;
            Vector3 curMouseVector = new Vector3(mousePos.x - objPos.x, 0, mousePos.z - objPos.z);

            SetRotationMouseObj(Quaternion.FromToRotation(initMouseVector, curMouseVector).eulerAngles.y);
        }

        if (Input.GetMouseButtonUp(1))
        {
            isMouseRightButtonDown = false;
            initObjAngle = Vector3.zero;
            initMouseVector = Vector3.zero;
        }
    }

    private bool CheckTagExist(GameObject obj, string[] tags) //tags중 obj의 tag에 해당하는 tag가 있는지 확인.
    {
        for (int i = 0; i < tags.Length; i++)
        {
            if (obj.CompareTag(tags[i]))
            {
                return true;
            }
        }
        return false;
    }

    private void UIOverMouseEffect() //마우스 호버링 이펙트 모음.
    {
        GameObject uiObjUnderMouse = GetUIObjUnderMouse();
        string[] tagMask = { "SelectButton", "Trashcan" };

        if (uiObjUnderMouse != null && CheckTagExist(uiObjUnderMouse, tagMask))
        {
            if (uiObjUnderMouse.CompareTag("SelectButton"))
            {
                uiObjUnderMouse.transform.GetChild(0).Rotate(new Vector3(10f, 20f, 30f) * Time.deltaTime);

                
                Transform tr = null;
                if (infoBoxSetTR.childCount > uiObjUnderMouse.transform.GetSiblingIndex())
                {
                    tr = infoBoxSetTR.GetChild(uiObjUnderMouse.transform.GetSiblingIndex());
                }

                if (tr != null)
                {
                    if (tr.gameObject.activeSelf == false)
                    {
                        tr.gameObject.SetActive(true);
                    }

                    RectTransform uiRT = tr.gameObject.GetComponent<RectTransform>();
                    RectTransform buttonRT = uiObjUnderMouse.GetComponent<RectTransform>();
                    Vector2 mousePos = Input.mousePosition;

                    Vector3 pivotPos = new Vector3(0f, Screen.height / 2, 0f);
                    Vector3 uiLocalPos = new Vector3(mousePos.x + uiRT.rect.width / 2, mousePos.y - uiRT.rect.height / 2, -100f) - pivotPos;

                    uiRT.localPosition = uiLocalPos;
                }

                preMouseUIObj = uiObjUnderMouse;
            }

            if (uiObjUnderMouse.CompareTag("Trashcan"))
            {
                trashCan.GetComponent<UnityEngine.UI.Image>().sprite = openTrash;

                preMouseUIObj = uiObjUnderMouse;
            }
        }
        else if (preMouseUIObj != null)
        {
            if (preMouseUIObj.CompareTag("Trashcan"))
            {
                trashCan.GetComponent<UnityEngine.UI.Image>().sprite = closeTrash;
            }

            if (preMouseUIObj.CompareTag("SelectButton"))
            {
                preMouseUIObj.transform.GetChild(0).localRotation = Quaternion.Euler(0f, 45f, 45f);

                Transform tr = null;
                if (infoBoxSetTR.childCount > preMouseUIObj.transform.GetSiblingIndex())
                {
                    tr = infoBoxSetTR.GetChild(preMouseUIObj.transform.GetSiblingIndex());
                }
                if (tr != null && tr.gameObject.activeSelf == true)
                {
                    tr.gameObject.SetActive(false);
                }
            }

            preMouseUIObj = null;
        }
    }

    private GameObject GetUIObjUnderMouse() //마우스에 위치한 UI 오브젝트 반환.
    {
        m_ped.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        m_gr.Raycast(m_ped, results);

        if (results.Count > 0)
        {
            return results[0].gameObject;
        }

        return null;
    }

    private void MoveObjToMouse() //선택한 월드 오브젝트 마우스 위치로 이동.
    {
        mouseObject.transform.position = GetMousePos();
    }

    private Vector3 GetMousePos() //마우스가 가리키는 월드 위치 반환.
    {
        Vector3 hitPos = Vector3.one;
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            hitPos = new Vector3(hit.point.x, 0.5f, hit.point.z);
        }

        return hitPos;
    }

    private GameObject GetObjUnderMouse()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits;

        hits = Physics.RaycastAll(ray);
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].transform.gameObject.CompareTag("Object"))
            {
                return hits[i].transform.gameObject;
            }
        }

        return null;
    }

    private void SelectObj() //마우스에 위치한 월드 오브젝트 선택.
    {
        selectedObj = null;
        ClearControlButton();

        GameObject obj = GetObjUnderMouse();

        if (obj != null)
        {
            selectedObj = obj;

            PopControlButton();
        }
    }

    private void PopControlButton()
    {
        ClearControlButton();

        Vector3 selObjPos = cam.WorldToScreenPoint(selectedObj.transform.position + new Vector3(0, 0.5f, 0)) + new Vector3(-Screen.width / 2, -Screen.height / 2, 0.0f);
        selObjPos.z = 0f;

        GameObject tmpObj = Instantiate(grabButton);
        tmpObj.transform.SetParent(gameObject.transform);
        tmpObj.GetComponent<RectTransform>().localPosition = selObjPos;
        tmpObj.GetComponent<RectTransform>().localScale = Vector3.one;
        objControlButtonLst.Add(tmpObj);
    }

    private void ClearControlButton()
    {
        for (int i = 0; i < objControlButtonLst.Count;i++)
        {
            Destroy(objControlButtonLst[i]);
        }

        objControlButtonLst.Clear();
    }

    private void InstantiateObj(GameObject obj)
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

        ClearControlButton();

        Vector3 point = GetMousePos();

        mouseObject = Instantiate(obj, point, Quaternion.identity);

        isMouseObjMovable = true;

        selectedObj= mouseObject;
    }

    private IEnumerator SlideUI(GameObject obj, Vector3 destination, float duration)
    {
        RectTransform rt = obj.GetComponent<RectTransform>();
        Vector3 start = rt.anchoredPosition;
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            rt.anchoredPosition = Vector3.Lerp(start, destination, elapsed / duration);
            yield return null;
        }

        rt.anchoredPosition = destination;
    }

    //===========================Public==============================

    public void SetRotationMouseObj(float angle) //선택된 월드 오브젝트 회전.
    {
        if (selectedObj != null)
        {
            selectedObj.transform.rotation = Quaternion.Euler(0, angle, 0);
        }
    }

    public void PopHideButton() //월드 오브젝트 선택 패널 팝업버튼.
    {
        RectTransform rectT = objSelPanel.GetComponent<RectTransform>();
        if (isHide)
        {
            StartCoroutine(SlideUI(objSelPanel, new Vector3(0, 0, 0), 0.5f));
            isHide = false;
        }
        else
        {
            StartCoroutine(SlideUI(objSelPanel, new Vector3(-rectT.sizeDelta.x, 0, 0), 0.5f));
            isHide = true;
        }
    }

    public void Button1()
    {
        InstantiateObj(laserStart);
    }

    public void Button2()
    {
        InstantiateObj(oneSideMirror);
    }

    public void Button3()
    {
        InstantiateObj(doubleSideMirror);
    }

    public void DestroyMouseObj() //선택된 월드 오브젝트 삭제.
    {
        if(mouseObject != null)
        {
            Destroy(mouseObject);
            mouseObject = null;
        }

        if(selectedObj != null)
        {
            Destroy(selectedObj);
            selectedObj = null;
            isMouseObjMovable = false;
        }

        ClearControlButton();
    }

    public void GrabSelectedObj()
    {
        if (selectedObj != null)
        {
            mouseObject = selectedObj;
            isMouseObjMovable = true;
            ClearControlButton();
        }
    }

    public void PlaceObjAtMouse() //선택한 월드 오브젝트 마우스 위치에 배치.
    {
        mouseObject.transform.position = GetMousePos();

        mouseObject = null;

        isMouseObjMovable = false;

        PopControlButton();
    }
}
