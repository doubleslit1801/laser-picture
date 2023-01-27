using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public readonly int MaxStage = 100;

    private StageMap[] stageMaps;
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

        stageMaps = new StageMap[MaxStage];
        LoadStageMaps();

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
        if(stageNumber < 0 && stageNumber >= MaxStage)
        {
            //error
        }
        return stageMaps[stageNumber].Drawing;
    }

    private void LoadStageMaps()
    {
        stageMaps[0] = new StageMap();
        stageMaps[0].Drawing = new Vector3[] 
        {
            new Vector3(0, 0, 0),
            new Vector3(10, 0, 0)
        };

        stageMaps[1] = new StageMap();
        stageMaps[1].Drawing = new Vector3[] 
        {
            new Vector3(0, 0, 0),
            new Vector3(0, 0, 10)
        };
    }
}
