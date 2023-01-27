using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Mirror : MonoBehaviour, IDevice
{
    private class LightInfo
    {
        public LightInfo(Light inputLight, Light outputLight, Vector3 inputPos, GameObject laserObject)
        {
            this.inputLight = inputLight;
            this.outputLight = outputLight;
            this.inputPos = inputPos;
            this.laserObject = laserObject;
        }

        public Light inputLight;
        public Light outputLight;
        public Vector3 inputPos;
        public GameObject laserObject;
    }
    private List<LightInfo> lights = new();
    public Material laserMaterial;

    void Start()
    {
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
            GameObject laserChild = new("laser");
            laserChild.transform.parent = transform;
            LineRenderer lr = laserChild.AddComponent<LineRenderer>();
            lr.startWidth = 0.1f;
            lr.endWidth = 0.1f;
            lr.material = laserMaterial;
            Vector3 outputDirection = Vector3.Reflect(inputLight.Direction, transform.forward);
            Light outputLight = new(hitPos, outputDirection, lr, Color.green);
            lights.Add(new LightInfo(inputLight, outputLight, hitPos, laserChild));
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
        Destroy(stopLight.laserObject);
        lights.Remove(stopLight);
    }
}
