using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageSelectButton : MonoBehaviour
{
    private GameObject[] stars;

    void Start()
    {
        stars = new GameObject[3];
        stars[0] = gameObject.transform.parent.GetChild(1).gameObject;
        stars[1] = gameObject.transform.parent.GetChild(2).gameObject;
        stars[2] = gameObject.transform.parent.GetChild(3).gameObject;
        for(int i=GameManager.Instance.GetPlayerStar(0); i<3; i++)
        {
            stars[i].SetActive(false);
        }
    }

    private void OnMouseUpAsButton()
    {
        SceneManager.LoadScene("InGameUITestScene");
        GameManager.Instance.NowStage = 1;
    }
}
