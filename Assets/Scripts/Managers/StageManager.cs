using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    [SerializeField]
    private int stageNumber = 0;
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

    public void LoadStage(int number)
    {
        StageData data = GameManager.Instance.GetStageData(number);
        ClearStage();

        stageNumber = number;
        lr.SetPositions(data.drawing);
        foreach(ObjectData objData in data.objects)
        {
            Instantiate(
                GameManager.Instance.GetPrefab(objData.prefab), 
                objData.position, 
                objData.rotation
            );
        }
    }

    public void SaveStage()
    {
        StageData data = new StageData();
        data.drawing = GameManager.Instance.GetStageData(stageNumber).drawing;

        List<ObjectData> objects = new List<ObjectData>();
        GameObject[] stageObjects = GameObject.FindGameObjectsWithTag("Object");
        foreach(GameObject obj in stageObjects)
        {
            ObjectData objData = new ObjectData();
            objData.prefab = obj.name.Replace("(Clone)", "");
            objData.position = obj.transform.position;
            objData.rotation = obj.transform.rotation;
            objects.Add(objData);
        }
        data.objects = objects.ToArray();
        GameManager.Instance.SetStageData(stageNumber, data);
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
