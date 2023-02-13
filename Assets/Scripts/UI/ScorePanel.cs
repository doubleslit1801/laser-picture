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
            float tmpSim = 0f;
            (starCnt, tmpSim) = scoreJudgement.GetScore();
            simularity = (int)Mathf.Floor(tmpSim * 100);

            UpdateScoreDisplay();

            isScoreReceived = true;
        }
    }

    private void UpdateScoreDisplay()
    {
        SetSimularityText(simularity);
        SetStar(starCnt);
    }

    private void SetSimularityText(float score)
    {
        gameObject.transform.Find("Simularity").GetComponent<TMP_Text>().text = score.ToString() + "%";
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

    public void OnActive()
    {
        simularity = 0;
        starCnt = 0;
        UpdateScoreDisplay();

        isScoreReceived = false;
    }

    public void ExitScorePanel()
    {
        canvasObj.GetComponent<InGameUI>().Resume();
        gameObject.SetActive(false);
    }

    public void ResetStage()
    {

    }

    public void ReturnStageSelectScene()
    {

    }

    public void NextStage()
    {

    }
}
