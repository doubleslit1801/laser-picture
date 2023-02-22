using System.Collections.Generic;
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
        }
    }

    void LateUpdate()
    {
        foreach (var i in lights)
        {
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
            Vector3 outputDirection = Vector3.Reflect(inputLight.Direction, transform.forward);
            Light outputLight = new(hitPos, outputDirection, lr, inputLight.LightColor);
            lights.Add(inputLight, new LightInfo(outputLight, hitPos, laserChild));
            print(lights.Count);
            StageManager.Instance.ReserveCount();
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
        StageManager.Instance.ReserveCount();
    }

    public void DestroyAll()
    {
        foreach (var light in lights)
        {
            light.Value.outputLight.Disable();
        }
    }
}
