using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageSelectSceneController : MonoBehaviour
{
    private readonly float MultiConst = 30f;

    public GameObject cameraObj;
    private bool isRunning = false;

    private void Start()
    {
        cameraObj = Camera.main.gameObject;

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
}
