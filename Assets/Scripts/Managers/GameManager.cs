using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public readonly int MaxStage = 100;

    private string playerDataPath;
    private string stageDataPath;
    private PlayerData playerData;
    private StageData[] stageData;
    private Dictionary<string, GameObject> prefabDict;
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

        prefabDict = new Dictionary<string, GameObject>();
        deviceDict = new Dictionary<Collider, IDevice>();
        LoadPrefabs();

        playerDataPath = Path.Combine(Application.dataPath, "PlayerData.json");
        LoadPlayerData();

        stageDataPath = Path.Combine(Application.dataPath, "StageData.json");
        LoadStageData();
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public GameObject GetPrefab(string name)
    {
        GameObject prefab;

        if(prefabDict.TryGetValue(name, out prefab))
        {
            return prefab;
        }
        else
        {
            return null;
        }
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
        return stageData[stageNumber];
    }

    public void SetStageData(int stageNumber, StageData data)
    {
        stageData[stageNumber] = data;
        SaveStageData();
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
        SavePlayerData();
    }

    private void LoadPrefabs()
    {
        prefabDict["LaserStart"] = Resources.Load<GameObject>("Prefabs/LaserStart");
    }

    private void SaveStageData()
    {
        string json = JsonHelper.ToJson(stageData, true);
        File.WriteAllText(stageDataPath, json);
    }

    private void LoadStageData()
    {
        if(File.Exists(stageDataPath))
        {
            string json = File.ReadAllText(stageDataPath);
            stageData = JsonHelper.FromJson<StageData>(json);
        }
        else
        {
            stageData = new StageData[MaxStage];
            for(int i=0; i<MaxStage; i++) 
            {
                stageData[i] = new StageData();
                stageData[i].drawing = new Vector3[2];
                stageData[i].objects = new ObjectData[1];
                stageData[i].objects[0] = new ObjectData();
                stageData[i].objects[0].prefab = "LaserStart";
            }
            SaveStageData();
        }
    }

    private void SavePlayerData()
    {
        string json = JsonUtility.ToJson(playerData, true);
        File.WriteAllText(playerDataPath, json);
    }

    private void LoadPlayerData()
    {
        if(File.Exists(playerDataPath))
        {
            string json = File.ReadAllText(playerDataPath);
            playerData = JsonUtility.FromJson<PlayerData>(json);
        }
        else
        {
            playerData = new PlayerData();
            playerData.stars = new int[MaxStage];
            Array.Clear(playerData.stars, 0, MaxStage);
            SavePlayerData();
        }
    }
}
