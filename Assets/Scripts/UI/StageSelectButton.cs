using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageSelectButton : MonoBehaviour
{
    private int stageNumber = 0;
    private GameObject[] stars;

    void Awake()
    {
        stars = new GameObject[3];
        stars[0] = gameObject.transform.parent.GetChild(1).gameObject;
        stars[1] = gameObject.transform.parent.GetChild(2).gameObject;
        stars[2] = gameObject.transform.parent.GetChild(3).gameObject;
    }

    public void SetStageNumber(int number)
    {
        stageNumber = number;
        for(int i=GameManager.Instance.GetPlayerStar(stageNumber); i<3; i++)
        {
            stars[i].SetActive(false);
        }
    }

    private void OnMouseUpAsButton()
    {
        SceneManager.LoadScene("InGameUITestScene");
        GameManager.Instance.NowStage = stageNumber;
    }
}
