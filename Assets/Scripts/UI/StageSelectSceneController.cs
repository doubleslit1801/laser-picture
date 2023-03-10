using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageSelectSceneController : MonoBehaviour
{
    private readonly float MultiConst = 30f;

    public GameObject cameraObj;
    private bool isRunning = false;

    private static GameObject stageSelectButton;

    private void Start()
    {
        cameraObj = Camera.main.gameObject;

        stageSelectButton ??= Resources.Load<GameObject>("Prefabs/UI/StageSelect");

        int[] worldStageNumber = new int[3];

        for (int world = 0; world < 3; world++)
        {
            worldStageNumber[world] = GameManager.Instance.GetStageNumberInWorld(world + 1);
            for (int row = 0; row < 3; row++)
            {
                for (int col = 0; col < 5; col++)
                {
                    int localStage = row * 5 + col;
                    int stage = localStage;
                    for (int i = 0; i < world; i++)
                    {
                        stage += worldStageNumber[i];
                    }
                    if (localStage >= worldStageNumber[world])
                    {
                        break;
                    }
                    GameObject button = MonoBehaviour.Instantiate(stageSelectButton);
                    Vector3 pos = new Vector3(30 * (world + 2) + 3 * col - 5, 0, 3 - 3 * row);
                    button.transform.position = pos;
                    button.transform.GetChild(0).GetComponent<StageSelectButton>().Stage = stage;
                }
            }
        }
    }

    public void ToState(int StateID)
    {
        if (!isRunning)
        {
            _ = StartCoroutine(ChangeState((State)StateID));
        }
        return;
    }

    private IEnumerator ChangeState(State destState)
    {
        Vector3 initPos = cameraObj.transform.position;
        float saturation = 0f;
        Vector3 tempPos;
        while (true)
        {
            if (saturation > 1f)
            {
                cameraObj.transform.position = new Vector3(MultiConst * (int)destState, 11f, 0f);
                break;
            }
            tempPos = Vector3.Slerp(initPos, new Vector3(MultiConst * (int)destState, 11f, 0f), saturation);
            cameraObj.transform.position = new Vector3(tempPos.x, 11f, 0f);
            saturation += Time.deltaTime;
            isRunning = true;
            yield return null;
        }
        isRunning = false;
        yield return null;
    }

    public enum State : int
    {
        Initial,
        WorldSelect,
        World1,
        World2,
        World3,
        Count
    }

    public void SetCameraToWorld(int world)
    {
        cameraObj.transform.position = new(30 * (world + 1), 11, 0);
    }
}
