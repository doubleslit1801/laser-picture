using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageSelectButton : MonoBehaviour
{
    private GameObject[] stars;
    private GameObject stageImage;
    private int _stage;
    public int Stage
    {
        set
        {
            _stage = value;
            stars = new GameObject[3];
            stars[0] = gameObject.transform.parent.GetChild(1).gameObject;
            stars[1] = gameObject.transform.parent.GetChild(2).gameObject;
            stars[2] = gameObject.transform.parent.GetChild(3).gameObject;
            for (int i = GameManager.Instance.GetPlayerStar(_stage); i < 3; i++)
            {
                stars[i].GetComponent<SpriteRenderer>().sprite = emptyStar;
            }

            //Texture2D Image = GameManager.Instance.GetStageData(stage).drawing; // �������� �����Ϳ� �̹��� �����ϵ��� ���� �� ����
            var image = Resources.Load<Texture2D>("Art/LoadingCircle");
            stageImage = transform.GetChild(0).gameObject;
            stageImage.GetComponent<Renderer>().material.mainTexture = image;
        }
    }

    private static Sprite emptyStar;

    private void OnMouseDown()
    {
        gameObject.transform.Translate(new Vector3(0f, -0.5f, 0f));
    }

    private void OnMouseUp()
    {
        gameObject.transform.Translate(new Vector3(0f, 0.5f, 0f));
    }

    void Awake()
    {
        if (emptyStar is null)
        {
            var temp = Resources.Load<Texture2D>("Art/Star(Empty)");
            emptyStar = Sprite.Create(temp, new Rect(0, 0, temp.width, temp.height), new Vector2(0.5f, 0.5f));
        }
    }

    private void OnMouseUpAsButton()
    {
        SceneManager.LoadScene("InGameUITestScene");
        GameManager.Instance.NowStage = _stage;
    }
}
