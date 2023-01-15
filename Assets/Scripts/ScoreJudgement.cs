using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ScoreJudgement : MonoBehaviour
{
    void Start()
    {

    }
    void Update()
    {

    }

    public Vector3[] GetUserDrawing()
    {
        return new Vector3[]
        {
            new Vector3(0, 0, 0),
            new Vector3(10, 0, 0)
        };
    }

    public Vector3[] GetAnswerDrawing()
    {
        return new Vector3[]
        {
            new Vector3(0, 0, 0),
            new Vector3(10, 0, 0)
        };
    }

    //Scores drawing according to simularity
    public float ScoreDrawing(Vector3[] userDrawing)
    {
        Vector3[] answer = GetAnswerDrawing();
        float score = 0.0f; //0~1.0

        //그림 형태 비교 알고리즘 개선 필요(꼭지점만이 아닌 선으로서 비교 필요)
        //for (int dotI = 0; (dotI < userDrawing.Count) && (dotI < answer.Count); dotI++)
        //{
        //    if (answer[dotI] == userDrawing[dotI])
        //    {
        //        score += 1 / answer.Count;
        //    }
        //}

        return score;
    }

    //Judges if the score satisfies the threshold of each stage
    public bool JudgeClear()
    {
        float currentScore = ScoreDrawing(/*유저드로잉*/GetUserDrawing()); //별 등 다양한 점수들 종합적으로 계산 필요
        float threshold = 0.8f;

        if (currentScore > threshold)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
