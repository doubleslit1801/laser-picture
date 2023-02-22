using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorLaser : MonoBehaviour
{
    private LineRenderer lr;
    private Light outputLight;
    [SerializeField]
    public Color outputColor;

    void Start()
    {
        lr = transform.GetChild(0).gameObject.GetComponent<LineRenderer>();
    }

    void Update()
    {
        if (lr != null)
        {
            if (outputLight == null)
            {
                outputLight = new Light(transform.position - transform.right * 0.5f, -transform.right, lr, outputColor);
                StageManager.Instance.ReserveCount();
            }
            else
            {
                outputLight.Update(transform.position - transform.right * 0.5f, -transform.right);
            }
            outputLight.Enable();
        }
    }
}
