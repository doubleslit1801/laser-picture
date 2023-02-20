using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScorePanel : MonoBehaviour
{
    public Sprite emptyStar, fullStar;
    public GameObject canvasObj;

    private int simularity, starCnt;
    private ScoreJudgement scoreJudgement;
    private bool isScoreReceived;

    void Start()
    {
        isScoreReceived = false;
        simularity = 0;
        starCnt = 0;
        scoreJudgement = GameObject.Find("ScoringObject").GetComponent<ScoreJudgement>();
    }

    void Update()
    {
        if (scoreJudgement.isScoreLoad && !isScoreReceived)
        {
            SetStageNumber(StageManager.Instance.GetCurrentStage());

            float tmpSim = 0f;
            (starCnt, tmpSim) = scoreJudgement.GetScore();
            simularity = (int)Mathf.Floor(tmpSim * 100);

            StartCoroutine(UpdateScoreDisplay());

            isScoreReceived = true;
        }
    }

    private IEnumerator UpdateScoreDisplay()
    {
        int tmpStarCnt = 0;
        string tmpJudgement = "Fail";

        for (int tmpSim = 0; tmpSim <= simularity; tmpSim++)
        {
            if (tmpSim > 95)
            {
                tmpStarCnt = 3;
                tmpJudgement = "Excellent!";
            }
            else if (tmpSim > 90)
            {
                tmpStarCnt = 2;
                tmpJudgement = "Good";
            }
            else if (tmpSim > 80)
            {
                tmpStarCnt = 1;
                tmpJudgement = "Not Bad";
            }

            SetSimularityText(tmpSim);
            SetStar(tmpStarCnt);

            if (tmpSim > 80)
            {
                yield return new WaitForSeconds(0.1f);
            }
            else if (tmpSim > 70)
            {
                yield return new WaitForSeconds(0.05f);
            }
            else
            {
                yield return new WaitForSeconds(0.01f);
            }
        }

        SetJudgement(tmpJudgement);
    }

    private void SetSimularityText(float score)
    {
        gameObject.transform.Find("Simularity").GetComponent<TMP_Text>().text = score.ToString() + "%";
    }

    private void SetStageNumber(int stageNumber)
    {
        gameObject.transform.Find("Title").GetComponent<TMP_Text>().text = "Stage " + stageNumber;
    }

    private void SetJudgement(string judgement)
    {
        gameObject.transform.Find("Judgement").GetComponent<TMP_Text>().text = judgement;
    }

    private void SetStar(int cnt)
    {
        if (cnt == 3)
        {
            gameObject.transform.Find("Stars").GetChild(0).GetComponent<UnityEngine.UI.Image>().sprite = fullStar;
            gameObject.transform.Find("Stars").GetChild(1).GetComponent<UnityEngine.UI.Image>().sprite = fullStar;
            gameObject.transform.Find("Stars").GetChild(2).GetComponent<UnityEngine.UI.Image>().sprite = fullStar;
        }
        else if (cnt == 2)
        {
            gameObject.transform.Find("Stars").GetChild(0).GetComponent<UnityEngine.UI.Image>().sprite = fullStar;
            gameObject.transform.Find("Stars").GetChild(1).GetComponent<UnityEngine.UI.Image>().sprite = fullStar;
            gameObject.transform.Find("Stars").GetChild(2).GetComponent<UnityEngine.UI.Image>().sprite = emptyStar;
        }
        else if (cnt == 1)
        {
            gameObject.transform.Find("Stars").GetChild(0).GetComponent<UnityEngine.UI.Image>().sprite = fullStar;
            gameObject.transform.Find("Stars").GetChild(1).GetComponent<UnityEngine.UI.Image>().sprite = emptyStar;
            gameObject.transform.Find("Stars").GetChild(2).GetComponent<UnityEngine.UI.Image>().sprite = emptyStar;
        }
        else
        {
            gameObject.transform.Find("Stars").GetChild(0).GetComponent<UnityEngine.UI.Image>().sprite = emptyStar;
            gameObject.transform.Find("Stars").GetChild(1).GetComponent<UnityEngine.UI.Image>().sprite = emptyStar;
            gameObject.transform.Find("Stars").GetChild(2).GetComponent<UnityEngine.UI.Image>().sprite = emptyStar;
        }

    }

    private int GetCurrentStageNum()
    {
        return StageManager.Instance.GetCurrentStage();
    }

    public void OnActive()
    {
        simularity = 0;
        starCnt = 0;
        UpdateScoreDisplay();

        isScoreReceived = false;
    }

    public void ResetStage()
    {
        canvasObj.GetComponent<InGameUI>().Resume();
        StageManager.Instance.ClearStage();
        gameObject.SetActive(false);
    }

    public void ReturnStageSelectScene()
    {
        transform.root.GetComponent<InGameUI>().MoveCameraAfterSceneChange();
    }

    public void NextStage()
    {
        canvasObj.GetComponent<InGameUI>().Resume();
        StageManager.Instance.ClearStage();
        StageManager.Instance.LoadStage(GetCurrentStageNum());
        gameObject.SetActive(false);
    }
}
