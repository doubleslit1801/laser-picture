using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    private int stageNumber;
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
        StartCoroutine(TestCoroutine());
    }

    void Update()
    {
        
    }

    public int GetCurrentStage()
    {
        return stageNumber;
    }

    public void LoadStage(int number)
    {
        Vector3[] newDrawing = GameManager.Instance.GetDrawing(number);
        stageNumber = number;
        lr.SetPositions(newDrawing);
    }

    private IEnumerator TestCoroutine()
    {
        LoadStage(0);
        yield return new WaitForSeconds(5);
        LoadStage(1);
    }
}
