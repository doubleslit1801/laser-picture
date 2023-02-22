using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RGBKlevv : MonoBehaviour
{
    private float eTime = 0f;
    private TextMeshPro text = null;

    private void Update()
    {
        eTime += Time.deltaTime/5f;
        if (eTime > 1f) eTime = 0f;
        if (text == null)
        {
            text = this.gameObject.GetComponent<TextMeshPro>();
        }
        else
        {
            text.color = Color.HSVToRGB(eTime, 1f, 1f);
        }
    }
}
