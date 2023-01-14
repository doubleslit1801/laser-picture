using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDevice : MonoBehaviour, IDevice
{
    private Light inputLight;
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
            Light outputLight = new Light(transform.position, transform.forward, lr, Color.green);
            outputLight.Raycast();
        }
    }

    public void HandleInput(Light light, Vector3 hitPos) 
    {
        inputLight = light;
        inputPos = hitPos;
    }
}
