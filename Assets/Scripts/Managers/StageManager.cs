using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    private int stageNumber = 0;
    private Dictionary<GameObject, GameObject> stageObjects; //object, prefab
    private LineRenderer lr;

    public static StageManager Instance;
    
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
    }

    void Start()
    {
        stageObjects = new Dictionary<GameObject, GameObject>();
        lr = GetComponent<LineRenderer>();
        //StartCoroutine(TestCoroutine());
    }

    void Update()
    {
        //test code
        if(Input.GetKeyDown(KeyCode.Space))
        {
            SaveStage();
            StartCoroutine(TestCoroutine());
        }
    }

    public int GetCurrentStage()
    {
        return stageNumber;
    }

    public void ClearStage()
    {
        foreach(GameObject obj in stageObjects.Keys)
        {
            Collider col = obj.GetComponent<Collider>();
            IDevice device = GameManager.Instance.SearchDevice(col);
            if(device != null)
            {
                GameManager.Instance.RemoveDevice(col);
                //destory all sibling objects
            }
            Destroy(obj);
        }
        stageObjects.Clear();
    }

    public void LoadStage(int number)
    {
        StageData data = GameManager.Instance.GetStageData(number);
        ClearStage();

        stageNumber = number;
        lr.SetPositions(data.Drawing);
        foreach(ObjectData objData in data.Objects)
        {
            GameObject obj = objData.ToInstance();
            stageObjects.Add(obj, objData.Prefab);
        }
    }

    public void SaveStage()
    {
        StageData data = new StageData();
        data.Drawing = GameManager.Instance.GetStageData(stageNumber).Drawing;

        List<ObjectData> objects = new List<ObjectData>();
        foreach(KeyValuePair<GameObject, GameObject> kvp in stageObjects)
        {
            ObjectData objData = new ObjectData();
            objData.Prefab = kvp.Value;
            objData.Position = kvp.Key.transform.position;
            objData.Rotation = kvp.Key.transform.rotation;
            objects.Add(objData);
        }
        data.Objects = objects.ToArray();
        GameManager.Instance.SetStageData(stageNumber, data);
    }

    public void ClearStageNew()
    {
        GameObject[] stageObjects = GameObject.FindGameObjectsWithTag("Object");
        foreach(GameObject obj in stageObjects)
        {
            Collider col = obj.GetComponent<Collider>();
            IDevice device = GameManager.Instance.SearchDevice(col);
            if(device != null)
            {
                GameManager.Instance.RemoveDevice(col);
                //destory all sibling objects
            }
            Destroy(obj);
        }
    }

    public void LoadStageNew(int number)
    {
        StageDataNew data = GameManager.Instance.GetStageDataNew(number);
        ClearStageNew();

        stageNumber = number;
        lr.SetPositions(data.drawing);
        foreach(ObjectDataNew objData in data.objects)
        {
            Instantiate(
                GameManager.Instance.GetPrefab(objData.prefab), 
                objData.position, 
                objData.rotation
            );
        }
    }

    public void SaveStageNew()
    {
        StageDataNew data = new StageDataNew();
        data.drawing = GameManager.Instance.GetStageDataNew(stageNumber).drawing;

        List<ObjectDataNew> objects = new List<ObjectDataNew>();
        GameObject[] stageObjects = GameObject.FindGameObjectsWithTag("Object");
        foreach(GameObject obj in stageObjects)
        {
            ObjectDataNew objData = new ObjectDataNew();
            objData.prefab = obj.name.Replace("(Clone)", "");
            objData.position = obj.transform.position;
            objData.rotation = obj.transform.rotation;
            objects.Add(objData);
        }
        data.objects = objects.ToArray();
        GameManager.Instance.SetStageDataNew(stageNumber, data);
    }

    public void AddObject(GameObject obj, GameObject prefab)
    {
        stageObjects.Add(obj, prefab);
        Debug.Log(stageObjects);
    }

    public void RemoveObject(GameObject obj, GameObject prefab)
    {
        stageObjects.Remove(obj);
    }

    private IEnumerator TestCoroutine()
    {
        //SaveStage(0);
        yield return new WaitForSeconds(3);
        ClearStage();
        yield return new WaitForSeconds(3);
        LoadStage(0);
    }
}
