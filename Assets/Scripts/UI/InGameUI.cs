using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class InGameUI : MonoBehaviour
{
    public GameObject laserStart, oneSideMirror, doubleSideMirror, prism, blackhole, composer;
    public GameObject grabButton, rotateImage, blackholePowerSlider, blackholeRange;
    public GameObject objSelPanel, optionPanel, settingsPanel, scorePanel, loadingPanel, pauseBlock, trashCan;
    public Sprite openTrash, closeTrash;

    #region Private Variables
    private Camera cam;

    private bool isHide, isControlButtonExist, isLaserCountOn, isPause, isInit;

    private GameObject preMouseUIObj, selectedObjUI, preSelectedObj;

    private Canvas m_canvas;
    private GraphicRaycaster m_gr;
    private PointerEventData m_ped;

    private Transform infoBoxSetTR;

    private List<GameObject> objControlButtonLst;

    private ObjectControl objControl;
    private ScoreJudgement scoreJudgement;

    private string[] objNameLst = { "LaserStart(Clone)", "LaserStart(Clone)", "LaserStart(Clone)", "OnesideMirror(Clone)", "DoublesideMirror(Clone)", "Prism(Clone)", "Blackhole(Clone)", "Composer(Clone)" };
    private int selectedObjUIIdx;

    private Slider blackholeSlider;
    private GameObject blackholeRangeSaveObj;

    #endregion

    void Start()
    {
        cam = Camera.main;

        preMouseUIObj = null;
        preSelectedObj = null;
        blackholeSlider = null;
        blackholeRangeSaveObj = null;

        isHide = false;
        isControlButtonExist = false;
        isLaserCountOn = false;
        isPause = false;

        m_canvas = gameObject.GetComponent<Canvas>();
        m_gr = m_canvas.GetComponent<GraphicRaycaster>();
        m_ped = new PointerEventData(null);

        objControl = GameObject.Find("ObjectController").GetComponent<ObjectControl>();
        scoreJudgement = GameObject.Find("ScoringObject").GetComponent<ScoreJudgement>();

        objControlButtonLst = new List<GameObject>();

        infoBoxSetTR = objSelPanel.transform.Find("InfoBoxSet");

        selectedObjUIIdx = -1;

        settingsPanel.SetActive(false);
        scorePanel.SetActive(false);
        loadingPanel.SetActive(false);
        pauseBlock.SetActive(false);

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
        if (isPause && isInit)
        {
            UpdateSelectedObjUI();
            ClearControlButton();
            isInit = true;
        }
        else
        {
            UIOverMouseEffect();

            UpdateSelectedObjUI();

            if (!isLaserCountOn)
            {
                StartCoroutine(UpdateLaserCount());
            }

            if (objControl.curMouseState == ObjectControl.mouseState.Select)
            {
                if (!isControlButtonExist || preSelectedObj != objControl.selectedObj)
                {
                    PopControlButton(objControl.selectedObj);
                    isControlButtonExist = true;
                    preSelectedObj = objControl.selectedObj;
                }

                if (blackholeSlider != null)
                {
                    CapsuleCollider blackholeCol = objControl.selectedObj.transform.Find("Range").GetComponent<CapsuleCollider>();
                    blackholeCol.radius = blackholeSlider.value;
                    float radius = blackholeCol.radius;
                    blackholeRangeSaveObj.transform.localScale = radius * Vector3.one;
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


        if (loadingPanel.activeSelf)
        {
            UpdateLoadingScreen();
        }
    }

    private IEnumerator UpdateLaserCount()
    {
        isLaserCountOn = true;

        int cnt = StageManager.Instance.LaserCnt;
        int maxLaser = StageManager.Instance.StageMaxLaser;

        optionPanel.transform.Find("LaserCount").GetComponent<TMP_Text>().text = cnt + " / " + maxLaser;

        yield return null;

        isLaserCountOn = false;
    }

    private void UpdateLoadingScreen()
    {
        loadingPanel.transform.Find("LoadingCircle").Rotate(10 * Time.deltaTime * Vector3.forward);
        if (scoreJudgement.progressState != null)
        {
            loadingPanel.transform.Find("LoadingText").GetComponent<TMP_Text>().text = "Now Loading... " + scoreJudgement.progressState;
        }
        else
        {
            loadingPanel.transform.Find("LoadingText").GetComponent<TMP_Text>().text = "Now Loading... ";
        }
    }

    private void UIOverMouseEffect() //마우스 호버링 이펙트 모음.
    {
        GameObject uiObjUnderMouse = GetUIObjUnderMouse();

        if (uiObjUnderMouse != null && uiObjUnderMouse.CompareTag("SelectButton"))
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

                if (uiLocalPos.y + pivotPos.y < uiRT.rect.height / 2)
                {
                    uiLocalPos.y = uiRT.rect.height / 2 - pivotPos.y;
                }

                uiRT.localPosition = uiLocalPos;
            }

            preMouseUIObj = uiObjUnderMouse;
        }
        else if (preMouseUIObj != null)
        {
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

    private void PopControlButton(GameObject targetObj)
    {
        ClearControlButton();

        Vector3 selObjPos = cam.WorldToScreenPoint(targetObj.transform.position) + new Vector3(-Screen.width / 2, -Screen.height / 2, 0.0f);
        selObjPos.z = 0f;
        GameObject tmpObj;

        if (targetObj.layer == LayerMask.NameToLayer("Blackhole"))
        {
            tmpObj = InstantiateUIObj(blackholePowerSlider, selObjPos + new Vector3(80, 0, 0), Vector3.one);
            blackholeSlider = tmpObj.GetComponent<Slider>();
            blackholeSlider.value = targetObj.transform.Find("Range").GetComponent<CapsuleCollider>().radius;
            objControlButtonLst.Add(tmpObj);

            tmpObj = Instantiate(blackholeRange, targetObj.transform.position, Quaternion.identity);
            blackholeRangeSaveObj = tmpObj;
            objControlButtonLst.Add(tmpObj);
        }
        else
        {
            tmpObj = InstantiateUIObj(rotateImage, selObjPos, Vector3.one);
            objControlButtonLst.Add(tmpObj);
        }

        tmpObj = InstantiateUIObj(grabButton, selObjPos, Vector3.one);
        objControlButtonLst.Add(tmpObj);
    }

    private void ClearControlButton()
    {
        for (int i = 0; i < objControlButtonLst.Count; i++)
        {
            Destroy(objControlButtonLst[i]);
        }

        objControlButtonLst.Clear();
    }

    private GameObject InstantiateUIObj(GameObject obj, Vector3 localPos, Vector3 localScale)
    {
        if (obj.GetComponent<RectTransform>() != null)
        {
            GameObject tmpObj = null;
            tmpObj = Instantiate(obj);
            tmpObj.transform.SetParent(gameObject.transform);
            tmpObj.GetComponent<RectTransform>().localPosition = localPos;
            tmpObj.GetComponent<RectTransform>().localScale = localScale;
            return tmpObj;
        }
        else
        {
            UnityEngine.Debug.LogWarning("InGameUI.InstantiateUIObj Got Object With No RectTransform Component!");
            return null;
        }
    }

    private void UpdateSelectedObjUI()
    {
        if (objControl.selectedObj != null)
        {
            for (int i = 0; i < objNameLst.Length; i++)
            {
                if (objControl.selectedObj.name == objNameLst[i])
                {
                    if (selectedObjUIIdx != i)
                    {
                        selectedObjUIIdx = i;

                        if (selectedObjUI != null)
                        {
                            Destroy(selectedObjUI);
                            selectedObjUI = null;
                        }

                        GameObject targetUIObj = objSelPanel.transform.GetChild(0).GetChild(i).GetChild(0).gameObject;
                        Vector3 uiPos = gameObject.transform.Find("OptionPanel").gameObject.GetComponent<RectTransform>().localPosition + new Vector3(0, 0, -30);
                        uiPos = uiPos + gameObject.transform.Find("OptionPanel").Find("SelectObjUI").gameObject.GetComponent<RectTransform>().localPosition;
                        selectedObjUI = InstantiateUIObj(targetUIObj, uiPos, targetUIObj.GetComponent<RectTransform>().localScale);
                    }
                    break;
                }
            }


        }
        else
        {
            Destroy(selectedObjUI);
            selectedObjUI = null;
            selectedObjUIIdx = -1;
        }
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

    private void ActivateScorePanel()
    {
        scorePanel.SetActive(true);
        scorePanel.GetComponent<RectTransform>().localPosition = Vector3.zero;
        scorePanel.GetComponent<ScorePanel>().OnActive();
    }

    private void ActivateLoadingPanel()
    {
        loadingPanel.SetActive(true);
        loadingPanel.GetComponent<RectTransform>().localPosition = Vector3.zero;
    }

    private IEnumerator GetScore()
    {
        ActivateLoadingPanel();

        yield return StartCoroutine(scoreJudgement.StartJudge(StageManager.Instance.GetCurrentStage()));

        loadingPanel.SetActive(false);
        ActivateScorePanel();
    }

    //===========================Public==============================

    public GameObject GetUIObjUnderMouse() //마우스에 위치한 UI 오브젝트 반환.
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

    public void Pause()
    {
        pauseBlock.SetActive(true);
        objControl.isPause = true;
        objControl.isInit = false;
        isPause = true;
        isInit = false;
    }

    public void Resume()
    {
        pauseBlock.SetActive(false);
        objControl.isPause = false;
        isPause = false;
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
        objControl.InstantiateObj(laserStart, 0.5f);
    }

    public void Button2()
    {
        objControl.InstantiateObj(laserStart, 0.5f);
    }

    public void Button3()
    {
        objControl.InstantiateObj(laserStart, 0.5f);
        objControl.ColorLaser(Color.red);
    }

    public void Button4()
    {
        objControl.InstantiateObj(oneSideMirror, 0.5f);
        objControl.ColorLaser(Color.green);
    }

    public void Button5()
    {
        objControl.InstantiateObj(doubleSideMirror, 0.5f);
        objControl.ColorLaser(Color.blue);
    }

    public void Button6()
    {
        objControl.InstantiateObj(prism, 0.0f);
    }

    public void Button7()
    {
        objControl.InstantiateObj(blackhole, 0.5f);
    }

    public void Button8()
    {
        objControl.InstantiateObj(composer, 0.0f);
    }

    public void Trashcan() //선택된 월드 오브젝트 삭제.
    {
        SoundManager.Instance.PlaySFXSound("Trash", 0.7f);
        objControl.DestroyMouseObj();
    }

    public void SettingsButton()
    {
        settingsPanel.SetActive(true);
        settingsPanel.GetComponent<RectTransform>().localPosition = Vector3.zero;

        Pause();
    }

    public void SubmitButton()
    {
        StartCoroutine(GetScore());

        Pause();
    }

    public void SaveStageButton()
    {
        StageManager.Instance.SaveStage();
    }

    public void ClearStageButton()
    {
        StageManager.Instance.ClearStage();
    }

    private IEnumerator MoveToSelect()
    {
        //Camera.main.gameObject.transform.Translate(new Vector3(30f, 0, 0));
        while (GameObject.Find("StageSelectionController") == null)
        {
            yield return null;
        }
        StageSelectSceneController Controller = GameObject.Find("StageSelectionController").GetComponent<StageSelectSceneController>();
        Controller.ToState(1);
        Destroy(gameObject);
    }

    public void MoveCameraAfterSceneChange()
    {
        DontDestroyOnLoad(gameObject);
        SceneManager.LoadScene("StageSelection");
        StartCoroutine(MoveToSelect());
    }
}
