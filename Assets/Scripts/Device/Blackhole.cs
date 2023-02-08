using System;
using System.Collections.Generic;
using UnityEngine;

public class Blackhole : MonoBehaviour, IDevice
{
    private class LightInfo
    {
        public LightInfo(Vector3 inputPos, Light outputLight, GameObject outputObject, LightCurve innerLight, GameObject innerObject)
        {
            this.inputPos = inputPos;
            this.outputLight = outputLight;
            this.outputObject = outputObject;
            this.innerLight = innerLight;
            this.innerObject = innerObject;
        }

        public Vector3 inputPos;
        public Light outputLight;
        public GameObject outputObject;
        public LightCurve innerLight;
        public GameObject innerObject;
    }
    private Dictionary<Light, LightInfo> lights = new();
    public Material laserMaterial;
    private float _radius = 4.0f;
    public float Radius
    {
        get => _radius;
        set
        {
            _radius = value;
            var col = GetComponent<CapsuleCollider>();
            transform.parent.localScale = Vector3.one * 0.25f * _radius;
        }
    }
    public float InnerRadius
    {
        get => _radius / 8;
    }

    void Start()
    {
        GameManager.Instance.registerDevice(GetComponent<Collider>(), this);
    }

    void Update()
    {
        foreach (var i in lights)
        {
            i.Value.innerLight.Update(i.Value.inputPos, i.Key.Direction);
        }
    }

    void LateUpdate()
    {
        foreach (var i in lights)
        {
            var outLightInfo = i.Value.innerLight.Enable();
            if (outLightInfo is null)
            {
                i.Value.outputLight.Disable();
            }
            else
            {
                i.Value.outputLight.Update(outLightInfo.Value.origin, outLightInfo.Value.direction);
                i.Value.outputLight.Enable();
            }
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
            GameObject outputObject = new("laser");
            outputObject.transform.parent = transform;
            LineRenderer outLr = outputObject.AddComponent<LineRenderer>();
            outLr.startWidth = 0.1f;
            outLr.endWidth = 0.1f;
            outLr.material = laserMaterial;
            GameObject innerObject = new("laserCurve");
            innerObject.transform.parent = transform;
            LineRenderer inLr = innerObject.AddComponent<LineRenderer>();
            inLr.startWidth = 0.1f;
            inLr.endWidth = 0.1f;
            inLr.material = laserMaterial;
            LightCurve innerLight = new(hitPos, inputLight.Direction, inLr, inputLight.LightColor);
            Light outputLight = new(Vector3.zero, Vector3.zero, outLr, inputLight.LightColor);
            lights.Add(inputLight, new LightInfo(hitPos, outputLight, outputObject, innerLight, innerObject));
        }
    }

    public void HandleInputStop(Light light)
    {
        print("stop input: " + light.GetHashCode());
        var stopLight = lights[light];
        stopLight.outputLight.Disable();
        stopLight.innerLight.Disable();
        Destroy(stopLight.innerObject);
        Destroy(stopLight.outputObject);
        lights.Remove(light);
        print(lights.Count);
    }
}
