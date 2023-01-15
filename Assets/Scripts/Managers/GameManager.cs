using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private Dictionary<Collider, IDevice> deviceDict;

    public static GameManager Instance;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);

        deviceDict = new Dictionary<Collider, IDevice>();
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IDevice searchDevice(Collider col)
    {
        IDevice device;

        if(deviceDict.TryGetValue(col, out device))
        {
            return device;
        }
        else
        {
            return null;
        }
    }

    public void registerDevice(Collider col, IDevice device) 
    {
        deviceDict[col] = device;
    }

    public Vector3[] GetDrawing(int stageNumber)
    {
        return new Vector3[] 
        {
            new Vector3(0, 0, 0),
            new Vector3(10, 0, 0)
        };
    }
}
