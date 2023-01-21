using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Mirror : MonoBehaviour, IDevice
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
            Vector3 outputDirection = Vector3.Reflect(inputLight.Direction, transform.forward);
            if(outputLight == null)
            {
                outputLight = new Light(inputPos, outputDirection, lr, Color.green);
            }
            else
            {
                outputLight.Update(inputPos, outputDirection);
            }
            outputLight.Enable();
        }
        else
        {
            outputLight?.Render(0);
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
    }
}
