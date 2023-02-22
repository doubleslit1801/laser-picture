using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonBehavior : MonoBehaviour
{
    GameObject SceneController;

    private void OnMouseDown()
    {
        gameObject.transform.Translate(new Vector3(0f, -0.5f, 0f) * transform.lossyScale.x);
        SoundManager.Instance.PlaySFXSound("ButtonDown");
    }

    private void OnMouseUp()
    {
        gameObject.transform.Translate(new Vector3(0f, 0.5f, 0f) * transform.lossyScale.x);
        SoundManager.Instance.PlaySFXSound("ButtonUp");
    }

    private void OnMouseUpAsButton()
    {
        SceneController ??= GameObject.Find("StageSelectionController");
        StageSelectSceneController Controller = SceneController.GetComponent<StageSelectSceneController>();

        if (gameObject.transform.parent.name.Contains("Initial"))
        {
            Controller.ToState(0);
        }
        else if (gameObject.transform.parent.name.Contains("Select"))
        {
            Controller.ToState(1);
        }
        else if (gameObject.transform.parent.name.Contains("World1"))
        {
            Controller.ToState(2);
        }
        else if (gameObject.transform.parent.name.Contains("World2"))
        {
            Controller.ToState(3);
        }
        else if (gameObject.transform.parent.name.Contains("World3"))
        {
            Controller.ToState(4);
        }
        else if (gameObject.transform.parent.name.Contains("Exit"))
        {
            Resources.FindObjectsOfTypeAll<ExitCheckPanel>()[0].gameObject.SetActive(true);
        }
        else
        {
            Debug.LogError("Undefined Behavior!");
        }
    }
}
