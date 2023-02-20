using System.Collections.Generic;
using UnityEngine;

public class Prism : MonoBehaviour, IDevice
{
    private class LightInfo
    {

        public LightInfo(Vector3 inputPos, (Light, Light) outputLight, (GameObject, GameObject) laserObject, bool isEnabled)
        {
            this.inputPos = inputPos;
            this.outputLight = outputLight;
            this.laserObject = laserObject;
            this.isEnabled = isEnabled;
        }

        public Vector3 inputPos;
        public (Light, Light) outputLight;
        public (GameObject, GameObject) laserObject;
        public bool isEnabled;
    }
    private Dictionary<Light, LightInfo> lights = new();
    private bool enableChanged;

    void Start()
    {
        GameManager.Instance.registerDevice(GetComponent<Collider>(), this);
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var i in lights)
        {
            //print(Vector3.Angle(i.Key.Direction, transform.forward));
            if (Vector3.Angle(i.Key.Direction, transform.forward) <= 30.0f)
            {
                if (!i.Value.isEnabled)
                {
                    enableChanged = true;
                }
                i.Value.isEnabled = true;
                for (int j = 0; j < 2; j++)
                {
                    Quaternion rotation = Quaternion.AngleAxis(120 * (j + 1), transform.up);
                    Vector3 outputPos = rotation * (transform.position - transform.parent.position) + transform.parent.position;
                    Vector3 outputDirection = Vector3.Reflect((rotation * i.Key.Direction) * -1, Normal(j)) * -1;
                    if (j == 0)
                    {
                        i.Value.outputLight.Item1.Update(outputPos, outputDirection);
                    }
                    else
                    {
                        i.Value.outputLight.Item2.Update(outputPos, outputDirection);
                    }
                }
            }
            else
            {
                if (i.Value.isEnabled)
                {
                    enableChanged = true;
                }
                i.Value.isEnabled = false;
            }
        }
    }

    public void LateUpdate()
    {
        foreach (var i in lights)
        {
            if (i.Value.isEnabled)
            {
                i.Value.outputLight.Item1.Enable();
                i.Value.outputLight.Item2.Enable();
            }
            else
            {
                i.Value.outputLight.Item1.Disable();
                i.Value.outputLight.Item2.Disable();
            }
        }
        if (enableChanged)
        {
            StageManager.Instance.ReserveCount();
            enableChanged = false;
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
            print(gameObject.name + "\tstart: " + inputLight);
            Light[] outputLight = new Light[2];
            GameObject[] laserObject = new GameObject[] { new("laser0"), new("laser1") };
            for (int i = 0; i < 2; i++)
            {
                laserObject[i].transform.parent = transform;
                LineRenderer lr = laserObject[i].AddComponent<LineRenderer>();
                Quaternion rotation = Quaternion.AngleAxis(120 * (i + 1), transform.up);
                Vector3 outputPos = rotation * (inputPos - transform.parent.position) + transform.parent.position;
                Vector3 outputDirection = Vector3.Reflect((rotation * inputLight.Direction) * -1, Normal(i)) * -1;
                outputLight[i] = new(outputPos, outputDirection, lr, inputLight.LightColor);
            }
            bool isEnabled = Vector3.Angle(inputLight.Direction, transform.forward) <= 30.0f;
            lights.Add(inputLight, new LightInfo(inputPos, (outputLight[0], outputLight[1]), (laserObject[0], laserObject[1]), isEnabled));
            if (isEnabled)
            {
                StageManager.Instance.ReserveCount();
            }
        }
    }

    public void HandleInputStop(Light light)
    {
        print(gameObject.name + "\tstop: " + light);
        var stopLight = lights[light];
        stopLight.outputLight.Item1.Disable();
        stopLight.outputLight.Item2.Disable();
        Destroy(stopLight.laserObject.Item1);
        Destroy(stopLight.laserObject.Item2);
        lights.Remove(light);
        StageManager.Instance.ReserveCount();
    }

    private Vector3 Normal(int i)
    {
        return Quaternion.AngleAxis(120 * (i + 1), transform.up) * transform.forward;
    }

    public void DestroyAll()
    {
        
    }
}
