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

    public void ToState0()
    {
        if (!isRunning)
        {
            _ = StartCoroutine(ChangeState(State.Initial));
        }
        return;
    }
    public void ToState1()
    {
        if (!isRunning)
        {
            _ = StartCoroutine(ChangeState(State.WorldSelect));
        }
        return;
    }
    public void ToState2()
    {
        if (!isRunning)
        {
            _ = StartCoroutine(ChangeState(State.StageSelect));
        }
        return;
    }
    public void ToState3()
    {
        if (!isRunning)
        {
            _ = StartCoroutine(ChangeState(State.EndState));
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

    private enum State : int
    {
        Initial,
        WorldSelect,
        StageSelect,
        EndState,
        Count
    }
}
