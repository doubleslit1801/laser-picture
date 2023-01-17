using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    private LineRenderer lr;
    private Light outputLight;
    
    void Start()
    {
        lr = GetComponent<LineRenderer>();
    }

    void Update()
    {
        if(outputLight == null)
        {
            outputLight = new Light(transform.position, -transform.right, lr, Color.red);
        }
        else
        {
            outputLight.Update(transform.position, -transform.right);
        }
        outputLight.Enable();
    }
}
