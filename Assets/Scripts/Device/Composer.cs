using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Composer : MonoBehaviour, IDevice
{
    private Light outputLight;
    private List<Light> inputLights;
    private LineRenderer lr;
    [SerializeField]
    private Color lightColor;

    void Start()
    {
        GameManager.Instance.registerDevice(GetComponent<Collider>(), this);
        inputLights = new List<Light>();
        GameObject laserObject = new("laser");
        laserObject.transform.parent = transform;

        lr = laserObject.AddComponent<LineRenderer>();
    }

    void Update()
    {
        if (inputLights.Count > 0)
        {
            Vector3 outputPos = transform.position + transform.up * 0.5f + transform.forward;
            Color newColor = new(0, 0, 0);
            foreach (var light in inputLights)
            {
                newColor += light.LightColor;
            }

            if (lightColor == newColor)
            {
                outputLight.Update(outputPos, transform.forward);
            }
            else
            {
                outputLight?.Disable();
                outputLight = new(outputPos, transform.forward, lr, newColor);
                lightColor = newColor;
            }
        }
        else
        {
            outputLight?.Disable();
            outputLight = null;
            lightColor = new(0, 0, 0, 0);
        }
    }

    void LateUpdate()
    {
        outputLight?.Enable();
    }

    public void HandleInput(Light inputLight, Vector3 hitPos)
    {
        if (!inputLights.Contains(inputLight))
        {
            inputLights.Add(inputLight);
            print("Added");
        }
    }

    public void HandleInputStop(Light light)
    {
        inputLights.Remove(light);
    }

    public void DestroyAll()
    {
        
    }
}
