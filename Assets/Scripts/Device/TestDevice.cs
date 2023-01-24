using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDevice : MonoBehaviour, IDevice
{
    private Light inputLight;
    private Light outputLight;
    private Vector3 inputPos;
    private LineRenderer lr;

    void Start()
    {
        lr = GetComponent<LineRenderer>();
        GameManager.Instance.registerDevice(GetComponent<Collider>(), this);
    }

    void Update()
    {
        if(inputLight != null)
        {
            if(outputLight == null)
            {
                outputLight = new Light(transform.position, transform.forward, lr, Color.green);
            }
            else
            {
                outputLight.Update(transform.position, transform.forward);
            }
            outputLight.Enable();
        }
        else
        {
            outputLight?.Disable();
        }
    }

    public void HandleInput(Light light, Vector3 hitPos) 
    {
        inputLight = light;
        inputPos = hitPos;
    }

    public void HandleInputStop()
    {
        inputLight = null;
        outputLight?.Disable();
    }
}
