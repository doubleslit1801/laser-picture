using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitCheckPanel: MonoBehaviour
{
    public void ExitGame()
    {
        Application.Quit();
    }

    public void NoButton()
    {
        gameObject.SetActive(false);
    }
}
