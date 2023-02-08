using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEngine;

//Activates Noramlly When "GameManager"is Activated
public class ScoreJudgement : MonoBehaviour
{
    public Camera screenCaptureCamera;

    private int resWidth;
    private int resHeight;

    string SCPath = "Assets/Resources/ScreenCapture/";

    void Start()
    {
        resWidth = Screen.width;
        resHeight = Screen.height;

        Instantiate(screenCaptureCamera);
    }
    void Update() //테스트용 지워도 무관
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ExtractAnswerMap(1);
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            UnityEngine.Debug.Log("ClearJudgement : " + JudgeClear(1));
        }

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            ResetScreenCaptureDirectory(SCPath);
        }
    }

    private Texture2D GetAnswerTexture(int stageNum)
    {
        return ReadPNGAsTexture("Answer_Stage" + stageNum);
    }

    private Texture2D GetUserDrawingTexture()
    {
        Texture2D screenCapture = CaptureScreenAsTexture();

        List<List<int>> positiveLst = new List<List<int>>();

        (screenCapture, positiveLst) = BinarizeTexture(screenCapture, resWidth, resHeight);

        SaveTextureAsPNG(screenCapture, "UserDrawing");

        return screenCapture;
    }

    private Texture2D CaptureScreenAsTexture()
    {
        int renderWidth = resHeight;
        int renderHeight = resHeight;

        RenderTexture rt = new RenderTexture(renderWidth, renderHeight, 24);
        screenCaptureCamera.targetTexture = rt;
        Texture2D screenShot = new Texture2D(renderWidth, renderHeight, TextureFormat.RGB24, false);
        Rect rec = new Rect(0, 0, screenShot.width, screenShot.height);
        screenCaptureCamera.Render();
        RenderTexture.active = rt;
        screenShot.ReadPixels(new Rect(0, 0, screenShot.width, screenShot.height), 0, 0);
        screenShot.Apply();

        screenCaptureCamera.targetTexture = null;

        return screenShot;
    }

    private void SaveTextureAsPNG(Texture2D tx, string fileName)
    {
        //string timestamp = System.DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
        //string fileName = "SCREENSHOT-" + timestamp + ".png";
        if (!fileName.EndsWith(".png"))
        {
            fileName = fileName + ".png";
        }

        byte[] bytes = tx.EncodeToPNG();

        if (!Directory.Exists(SCPath))
        {
            Directory.CreateDirectory(SCPath);
        }

        File.WriteAllBytes(SCPath + fileName, bytes);

        UnityEngine.Debug.Log("Saved Texture As PNG : " + fileName);
    }

    private Texture2D ReadPNGAsTexture(string fileName)
    {
        if (!fileName.EndsWith(".png"))
        {
            fileName = fileName + ".png";
        }

        if (!File.Exists(SCPath + fileName))
        {
            return null;
        }

        byte[] byteTexture = File.ReadAllBytes(SCPath + fileName);
        Texture2D texture = new Texture2D(1, 1);
        texture.LoadImage(byteTexture);

        UnityEngine.Debug.Log("Read PNG As Texture : " + fileName);

        return texture;
    }

    private (Texture2D, List<List<int>>) BinarizeTexture(Texture2D tx, int width, int height)
    {
        List<List<int>> positiveLst = new List<List<int>>();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Color rgbValue = tx.GetPixel(x, y);
                if (rgbValue[0] > 0.8 && rgbValue[1] > 0.8 && rgbValue[2] > 0.8) //색깔에 따른 인식 개선 필요
                {
                    tx.SetPixel(x, y, Color.white);

                    positiveLst.Add(new List<int> { x, y });
                }
                else
                {
                    tx.SetPixel(x, y, Color.black);
                }
            }
        }

        return (tx, positiveLst);
    }

    //private Texture2D ExpandTexturePixel(Texture2D tx, List<List<int>> positiveLst, int width, int height, int expandScale)
    //{
    //    Texture2D tmpTexture = tx;

    //    for (int i = 0; i < positiveLst.Count;i++)
    //    {
    //        List<int> pivot = positiveLst[i];

    //        for (int x = pivot[0] - expandScale; x < pivot[0] + expandScale + 1; x++)
    //        {
    //            for (int y = pivot[1] - expandScale; y < pivot[1] + expandScale + 1; y++)
    //            {
    //                if ((x >= 0 && x <= width) && (y >= 0 && y <= height))
    //                {
    //                    tmpTexture.SetPixel(x, y, Color.white);
    //                }
    //            }
    //        }
    //    }

    //    return tmpTexture;
    //}

    //Scores drawing according to simularity

    private float ScoreDrawingByGrid(Texture2D answerTx, Texture2D userDrawingTx, int gridScale)
    {
        if (answerTx.width != userDrawingTx.width || answerTx.height != userDrawingTx.height)
        {
            UnityEngine.Debug.Log("Error : ScoringDrawingByGrid() / Texture Size Difference");
            return 0.0f;
        }

        int maxCnt = 0;
        int hitCnt = 0;

        for (int c = 0; c < answerTx.width - gridScale; c = c + gridScale)
        {
            for (int r = 0; r < answerTx.height - gridScale; r = r + gridScale)
            {
                bool isAFilled = false;
                bool isUFilled = false;

                for (int x = c; x < c + gridScale; x++)
                {
                    for (int y = r; y < r + gridScale; y++)
                    {
                        if (answerTx.GetPixel(x, y) == Color.white)
                        {
                            isAFilled = true;
                        }
                        if (userDrawingTx.GetPixel(x, y) == Color.white)
                        {
                            isUFilled = true;
                        }
                    }
                }

                if (isAFilled)
                {
                    maxCnt = maxCnt + 50;
                    if (isUFilled)
                    {
                        hitCnt = hitCnt + 50;
                    }
                    else
                    {
                        hitCnt = hitCnt + 0;
                    }
                }
                else
                {
                    maxCnt = maxCnt + 1;
                    if (isUFilled)
                    {
                        hitCnt = hitCnt - 25;
                    }
                    else
                    {
                        hitCnt = hitCnt + 1;
                    }
                }
            }
        }

        return (float)hitCnt / (float)maxCnt;
    }

    //================================PUBLIC=================================================

    //Judges if the score satisfies the threshold of each stage and Returns (star count, simularity)
    public (int, float) JudgeClear(int stageNum)
    {
        Texture2D answerTx = GetAnswerTexture(stageNum);
        Texture2D userDrawingTx = GetUserDrawingTexture();

        if (answerTx == null)
        {
            UnityEngine.Debug.Log("File Doesn't Exist! : " + SCPath + "Answer_Stage" + stageNum + "  Generate Answer File First!");
            return (0, 0.0f);
        }

        var simularity = ScoreDrawingByGrid(answerTx, userDrawingTx, 10);

        float oneStarThreshold = 0.8f;
        float twoStarThreshold = 0.9f;
        float threeStarThreshold = 0.95f;

        if (simularity > threeStarThreshold)
        {
            return (3, simularity);
        }
        else if (simularity > twoStarThreshold)
        {
            return (2, simularity);
        }
        else if (simularity > oneStarThreshold)
        {
            return (1, simularity);
        }
        else
        {
            return (0, simularity);
        }
    }

    public void ExtractAnswerMap(int stageNum)
    {
        Texture2D screenCapture = CaptureScreenAsTexture();

        List<List<int>> positiveLst = new List<List<int>>();

        (screenCapture, positiveLst) = BinarizeTexture(screenCapture, resWidth, resHeight);

        SaveTextureAsPNG(screenCapture, "Answer_Stage" + stageNum);
    }

    public void ResetScreenCaptureDirectory(string path)
    {
        if (Directory.Exists(path))
        {
            Directory.Delete(path, true);
        }
        Directory.CreateDirectory(path);

        UnityEngine.Debug.Log("Directory Reset : " + path);
    }
}
