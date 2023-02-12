using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonBehavior : MonoBehaviour
{
    GameObject SceneController;

    private void OnMouseDown()
    {
        gameObject.transform.Translate(new Vector3(0f, -0.5f, 0f));
    }

    private void OnMouseUp()
    {
        gameObject.transform.Translate(new Vector3(0f, 0.5f, 0f));
    }

    private void OnMouseUpAsButton()
    {
        SceneController ??= GameObject.Find("StageSelectionController");
        StageSelectSceneController Controller = SceneController.GetComponent<StageSelectSceneController>();
        if (gameObject.transform.parent.name.Contains("Initial"))
        {
            Controller.ToState0();
        }
        else if (gameObject.transform.parent.name.Contains("World"))
        {
            Controller.ToState1();
        }
        else if (gameObject.transform.parent.name.Contains("Stage"))
        {
            Controller.ToState2();
        }
        else if (gameObject.transform.parent.name.Contains("End"))
        {
            Controller.ToState3();
        }
        else
        {
            Debug.LogError("Undefined Behavior!");
        }
    }
}
