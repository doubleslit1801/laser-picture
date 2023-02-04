using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public readonly int MaxStage = 100;

    private PlayerData playerData;
    private StageData[] stages;
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

        playerData = new PlayerData();
        playerData.stars = new int[MaxStage];
        stages = new StageData[MaxStage];
        LoadStageData();

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

    public IDevice SearchDevice(Collider col)
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

    public void RemoveDevice(Collider col)
    {
        deviceDict.Remove(col);
    }

    public StageData GetStageData(int stageNumber)
    {
        if(stageNumber < 0 && stageNumber >= MaxStage)
        {
            //error
        }
        return stages[stageNumber];
    }

    public void SetStageData(int stageNumber, StageData data)
    {
        stages[stageNumber] = data;
    }

    public int GetPlayerStar(int stageNumber)
    {
        if(stageNumber < 0 && stageNumber >= MaxStage)
        {
            //error
        }
        return playerData.stars[stageNumber];
    }

    public void SetPlayerStar(int stageNumber, int star)
    {
        playerData.stars[stageNumber] = star;
    }

    private void LoadStageData()
    {
        //test data
        stages[0] = new StageData();
        stages[0].Drawing = new Vector3[]
        {
            new Vector3(0, 0, 0),
            new Vector3(0, 0, 10)
        };
    }

    public void SaveData()
    {

    }

    public void LoadData()
    {
        
    }
}
