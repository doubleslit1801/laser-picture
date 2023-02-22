using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    private LineRenderer lr;
    private Light outputLight;

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
                outputLight = new Light(transform.position, -transform.right, lr, Color.red);
                StageManager.Instance.ReserveCount();
            }
            else
            {
                outputLight.Update(transform.position, -transform.right);
            }
            outputLight.Enable();
        }
    }

    public void DestroyAll()
    {
        outputLight.Disable();
        StageManager.Instance.ReserveCount();
    }
}
