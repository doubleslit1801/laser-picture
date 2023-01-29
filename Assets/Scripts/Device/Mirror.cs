using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

public class Mirror : MonoBehaviour, IDevice
{
    private class LightInfo
    {
        public LightInfo(Light outputLight, Vector3 inputPos, GameObject laserObject)
        {
            this.outputLight = outputLight;
            this.inputPos = inputPos;
            this.laserObject = laserObject;
        }

        public Light outputLight;
        public Vector3 inputPos;
        public GameObject laserObject;
    }
    private Dictionary<Light, LightInfo> lights = new();
    public Material laserMaterial;

    void Start()
    {
        GameManager.Instance.registerDevice(GetComponent<Collider>(), this);
    }

    void Update()
    {
        foreach (var i in lights)
        {
            Vector3 outputDirection = Vector3.Reflect(i.Key.Direction, transform.forward);
            i.Value.outputLight.Update(i.Value.inputPos, outputDirection);
            i.Value.outputLight.Enable();
        }
    }

    public void HandleInput(Light inputLight, Vector3 hitPos)
    {
        try
        {
            lights[inputLight].inputPos = hitPos;
        }
        catch
        {
            print("new input: " + inputLight.GetHashCode());
            GameObject laserChild = new("laser");
            laserChild.transform.parent = transform;
            LineRenderer lr = laserChild.AddComponent<LineRenderer>();
            lr.startWidth = 0.1f;
            lr.endWidth = 0.1f;
            lr.material = laserMaterial;
            Vector3 outputDirection = Vector3.Reflect(inputLight.Direction, transform.forward);
            Light outputLight = new(hitPos, outputDirection, lr, Color.green);
            lights.Add(inputLight, new LightInfo(outputLight, hitPos, laserChild));
            print(lights.Count);
        }
    }

    public void HandleInputStop(Light light)
    {
        print("stop input: " + light.GetHashCode());
        var stopLight = lights[light];
        stopLight.outputLight.Disable();
        Destroy(stopLight.laserObject);
        lights.Remove(light);
        print(lights.Count);
    }
}
