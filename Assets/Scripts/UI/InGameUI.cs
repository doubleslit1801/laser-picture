using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InGameUI : MonoBehaviour
{
    public GameObject laserStart, oneSideMirror, doubleSideMirror, objSelPanel, trashCan, grabButton, rotateImage;
    public Sprite openTrash, closeTrash;

    private Camera cam;

    private bool isHide, isControlButtonExist;

    private GameObject preMouseUIObj;

    private Canvas m_canvas;
    private GraphicRaycaster m_gr;
    private PointerEventData m_ped;

    private Transform infoBoxSetTR;

    private List<GameObject> objControlButtonLst;

    private ObjectControl objControl;

    void Start()
    {
        cam = Camera.main;

        preMouseUIObj = null;

        isHide = false;
        isControlButtonExist = false;

        m_canvas = gameObject.GetComponent<Canvas>();
        m_gr = m_canvas.GetComponent<GraphicRaycaster>();
        m_ped = new PointerEventData(null);

        objControl = GameObject.Find("ObjectController").GetComponent<ObjectControl>();

        objControlButtonLst = new List<GameObject>();

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
        UIOverMouseEffect();

        if (objControl.curMouseState == ObjectControl.mouseState.Select)
        {
            if (!isControlButtonExist)
            {
                PopControlButton(objControl.selectedObj.transform.position);
                isControlButtonExist = true;
            }
        }
        else
        {
            if (isControlButtonExist)
            {
                ClearControlButton();
                isControlButtonExist = false;
            }
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

    private void PopControlButton(Vector3 objPos)
    {
        ClearControlButton();

        Vector3 selObjPos = cam.WorldToScreenPoint(objPos + new Vector3(0, 0.5f, 0)) + new Vector3(-Screen.width / 2, -Screen.height / 2, 0.0f);
        selObjPos.z = 0f;
        GameObject tmpObj;

        tmpObj = Instantiate(rotateImage);
        tmpObj.transform.SetParent(gameObject.transform);
        tmpObj.GetComponent<RectTransform>().localPosition = selObjPos;
        tmpObj.GetComponent<RectTransform>().localScale = Vector3.one;
        objControlButtonLst.Add(tmpObj);

        tmpObj = Instantiate(grabButton);
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
        objControl.InstantiateObj(laserStart);
    }

    public void Button2()
    {
        objControl.InstantiateObj(oneSideMirror);
    }

    public void Button3()
    {
        objControl.InstantiateObj(doubleSideMirror);
    }

    public void Trashcan() //선택된 월드 오브젝트 삭제.
    {
        objControl.DestroyMouseObj();
    }
}
