using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Mirror : MonoBehaviour, IDevice
{
    private class LightInfo
    {
        public LightInfo(Light inputLight, Light outputLight, Vector3 inputPos)
        {
            this.inputLight = inputLight;
            this.outputLight = outputLight;
            this.inputPos = inputPos;
        }

        public Light inputLight;
        public Light outputLight;
        public Vector3 inputPos;
    }
    private List<LightInfo> lights = new();
    private LineRenderer lr;

    void Start()
    {
        lr = GetComponent<LineRenderer>();
        GameManager.Instance.registerDevice(GetComponent<Collider>(), this);
    }

    void Update()
    {
        foreach (var i in lights)
        {
            Vector3 outputDirection = Vector3.Reflect(i.inputLight.Direction, transform.forward);
            i.outputLight.Update(i.inputPos, outputDirection);
            i.outputLight.Enable();
        }
    }

    public void HandleInput(Light inputLight, Vector3 hitPos)
    {
        var findRes = lights.Find(x => x.inputLight == inputLight);
        if (findRes == null)
        {
            Vector3 outputDirection = Vector3.Reflect(inputLight.Direction, transform.forward);
            Light outputLight = new(hitPos, outputDirection, lr, Color.green);
            lights.Add(new LightInfo(inputLight, outputLight, hitPos));
            print(lights.Count);
        }
        else
        {
            findRes.inputPos = hitPos;
        }
    }

    public void HandleInputStop(Light light)
    {
        var stopLight = lights.Find(x => x.inputLight == light);
        stopLight.outputLight.Disable();
        lights.Remove(stopLight);
    }
}
