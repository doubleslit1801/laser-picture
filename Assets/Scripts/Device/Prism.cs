using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class Prism : MonoBehaviour, IDevice
{
    private class LightInfo
    {

        public LightInfo(Vector3 inputPos, (Light, Light) outputLight, (GameObject, GameObject) laserObject)
        {
            this.inputPos = inputPos;
            this.outputLight = outputLight;
            this.laserObject = laserObject;
        }

        public Vector3 inputPos;
        public (Light, Light) outputLight;
        public (GameObject, GameObject) laserObject;
    }
    private Dictionary<Light, LightInfo> lights = new();
    private Vector3[] normals = new Vector3[2];
    public Material laserMaterial;

    void Start()
    {
        GameManager.Instance.registerDevice(GetComponent<Collider>(), this);
        for (int i = 0; i < 2; i++)
        {
            normals[i] = Quaternion.AngleAxis(120 * (i + 1), transform.up) * transform.forward;
            print(gameObject.name + "//" + "normals[" + i + "]: " + normals[i]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var i in lights)
        {
            for(int j = 0; j < 2; j++)
            {
                Quaternion rotation = Quaternion.AngleAxis(120 * (j + 1), transform.up);
                Vector3 outputPos = rotation * (i.Value.inputPos - transform.parent.position) + transform.parent.position;
                Vector3 outputDirection = Vector3.Reflect((rotation * i.Key.Direction) * -1, normals[j]) * -1;
                if (j == 0)
                {
                    i.Value.outputLight.Item1.Update(outputPos, outputDirection);
                    i.Value.outputLight.Item1.Enable();
                }
                else
                {
                    i.Value.outputLight.Item2.Update(outputPos, outputDirection);
                    i.Value.outputLight.Item2.Enable();
                }
            }
        }
    }

    public void HandleInput(Light inputLight, Vector3 inputPos)
    {
        try
        {
            lights[inputLight].inputPos = inputPos;
        }
        catch
        {
            Light[] outputLight = new Light[2];
            GameObject[] laserObject = new GameObject[] { new("laser0"), new("laser1") };
            for (int i = 0; i < 2; i++)
            {
                laserObject[i].transform.parent = transform;
                LineRenderer lr = laserObject[i].AddComponent<LineRenderer>();
                lr.startWidth = 0.1f;
                lr.endWidth = 0.1f;
                lr.material = laserMaterial;
                Quaternion rotation = Quaternion.AngleAxis(120 * (i + 1), transform.up);
                Vector3 outputPos = rotation * (inputPos - transform.parent.position) + transform.parent.position;
                Vector3 outputDirection = Vector3.Reflect((rotation * inputLight.Direction) * -1, normals[i]) * -1;
                outputLight[i] = new(outputPos, outputDirection, lr, inputLight.LightColor);
                outputLight[i].Enable();
            }
            lights.Add(inputLight, new LightInfo(inputPos, (outputLight[0], outputLight[1]), (laserObject[0], laserObject[1])));
        }
    }

    public void HandleInputStop(Light light)
    {
        var stopLight = lights[light];
        stopLight.outputLight.Item1.Disable();
        stopLight.outputLight.Item2.Disable();
        Destroy(stopLight.laserObject.Item1);
        Destroy(stopLight.laserObject.Item2);
        lights.Remove(light);
    }
}
