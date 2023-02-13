using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScorePanel : MonoBehaviour
{
    public Sprite emptyStar, fullStar;

    private int simularity, starCnt;
    private ScoreJudgement scoreJudgement;

    void Update()
    {

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
        scoreJudgement = GameObject.Find("ScoringObject").GetComponent<ScoreJudgement>();
        simularity = 0;
        starCnt = 0;

        float tmpSim = 0f;
        (starCnt, tmpSim) = scoreJudgement.JudgeClear(1);

        simularity = (int)Mathf.Floor(tmpSim * 100);

        SetSimularityText(simularity);
        SetStar(starCnt);
    }

    public void ExitScorePanel()
    {
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
