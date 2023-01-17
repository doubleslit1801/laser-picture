using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

//Activates Noramlly When "GameManager"is Activated
public class ScoreJudgement : MonoBehaviour
{
    void Start()
    {
        
    }
    void Update()
    {
        UnityEngine.Debug.Log(JudgeClear());
    }

    //유저 드로잉 인식 필요
    public Vector3[] GetUserDrawing()
    {
        return new Vector3[]
        {
            new Vector3(0, 0, 0),
            new Vector3(10, 0, 0),
            new Vector3(15, 5, 0),
        };
    }

    public Vector3[] GetAnswerDrawing()
    {
        return new Vector3[]
        {
            new Vector3(0, 0, 0),
            new Vector3(10, 0, 0),
            new Vector3(15,5, 0)
        };

        //return Instance.GetDrawing(1);
    }

    //Scores drawing according to simularity
    public (int correctCnt, int maxCnt) ScoreDrawing(Vector3[] answer,Vector3[] userDrawing)
    {
        int correctCnt = 0; 
        int maxCnt = answer.Length - 1;

        //그림 형태 비교 알고리즘 개선 필요
        //임시로 벡터가 일치해야 점수 오르게 해둠.
        for (int dotI = 0; (dotI < (userDrawing.Length - 1)) && (dotI < (answer.Length - 1)); dotI++)
        {
            Vector3 aV = answer[dotI + 1] - answer[dotI];
            Vector3 uV = userDrawing[dotI + 1] - userDrawing[dotI];

            if (aV == uV)
            {
                correctCnt += 1;
            }
        }

        return (correctCnt, maxCnt);
    }

    //Judges if the score satisfies the threshold of each stage
    public bool JudgeClear()
    {
        var (curCnt, maxCnt) = ScoreDrawing(GetAnswerDrawing() ,GetUserDrawing());

        UnityEngine.Debug.Log("Current Correct Count : " + curCnt);
        UnityEngine.Debug.Log("Max Correct Count : " + maxCnt);

        float threshold = 0.8f;
        float curPer = (float)curCnt / (float)maxCnt;

        if (curPer > threshold)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
