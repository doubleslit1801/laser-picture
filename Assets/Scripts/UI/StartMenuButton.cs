using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartMenuButton : MonoBehaviour
{
    public GameObject pauseBlock, settingsPanel;

    private void OnMouseDown()
    {
        gameObject.transform.Translate(new Vector3(0f, -0.5f, 0f));
        SoundManager.Instance.PlaySFXSound("ButtonDown");
    }

    private void OnMouseUp()
    {
        gameObject.transform.Translate(new Vector3(0f, 0.5f, 0f));
        SoundManager.Instance.PlaySFXSound("ButtonUp");
    }

    private void OnMouseUpAsButton()
    {
        if (gameObject.transform.parent.name.Contains("Setting"))
        {
            pauseBlock.SetActive(true);
            settingsPanel.SetActive(true);
        }
    }
}