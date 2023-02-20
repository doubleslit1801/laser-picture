using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageSelectButtonGenerator : MonoBehaviour
{
    private GameObject button;

    void Start()
    {
        button = Resources.Load<GameObject>("Prefabs/StageSelectButton");

        //world1
        float startX = 55, startY = (float)-16.5, startZ = 3;
        int i = 0;
        for(int dz = 0; dz < 9; dz += 3)
        {
            for(int dx = 0; dx < 15; dx += 3)
            {
                GameObject newButton = Instantiate(button);
                newButton.transform.position = new Vector3(startX + dx, startY, startZ - dz);
                newButton.transform.GetChild(0).gameObject.GetComponent<StageSelectButton>().SetStageNumber(i);
                i += 1;
            }
        }
        startX = 85;
        startZ = 3;
        for(int dz = 0; dz < 9; dz += 3)
        {
            for(int dx = 0; dx < 15; dx += 3)
            {
                GameObject newButton = Instantiate(button);
                newButton.transform.position = new Vector3(startX + dx, startY, startZ - dz);
                newButton.transform.GetChild(0).gameObject.GetComponent<StageSelectButton>().SetStageNumber(i);
                i += 1;
            }
        }
        startX = 115;
        startZ = 3;
        for(int dz = 0; dz < 9; dz += 3)
        {
            for(int dx = 0; dx < 15; dx += 3)
            {
                GameObject newButton = Instantiate(button);
                newButton.transform.position = new Vector3(startX + dx, startY, startZ - dz);
                newButton.transform.GetChild(0).gameObject.GetComponent<StageSelectButton>().SetStageNumber(i);
                i += 1;
            }
        }
    }

    void Update()
    {
        
    }
}
