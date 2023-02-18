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
    [HideInInspector] public bool isScoreLoad;
    [HideInInspector] public string progressState;

    private float simularity;
    private int resWidth, resHeight;
    private Texture2D userDrawingTexture;

    string SCPath = "Assets/Resources/ScreenCapture/";

    void Start()
    {
        resWidth = Screen.width;
        resHeight = Screen.height;

        InitScoreVar();

        Instantiate(screenCaptureCamera);
    }

    void Update() //테스트용 지워도 무관
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(ExtractAnswerMap(StageManager.Instance.GetCurrentStage()));
        }

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            ResetScreenCaptureDirectory(SCPath);
        }
    }

    private void InitScoreVar()
    {
        isScoreLoad = false;
        simularity = 0f;
    }

    private Texture2D GetAnswerTexture(int stageNum)
    {
        progressState = "Getting Answer Map";

        return ReadPNGAsTexture("Answer_Stage" + stageNum);
    }

    private IEnumerator GetUserDrawingTexture()
    {
        progressState = "Getting User Drawing";

        int[] maskLst = { LayerMask.NameToLayer("Default"), LayerMask.NameToLayer("Light Raycast") };
        userDrawingTexture = CaptureScreenAsTexture(maskLst);

        yield return StartCoroutine(BinarizeTexture(userDrawingTexture, resWidth, resHeight));

        StartCoroutine(SaveTextureAsPNG(userDrawingTexture, "UserDrawing"));
    }

    private Texture2D CaptureScreenAsTexture(int[] maskLst)
    {
        int renderWidth = resHeight;
        int renderHeight = resHeight;

        screenCaptureCamera.cullingMask = 0;
        foreach (int mask in maskLst)
        {
            screenCaptureCamera.cullingMask |= 1 << LayerMask.NameToLayer("Group");
        }

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

    private IEnumerator SaveTextureAsPNG(Texture2D tx, string fileName)
    {
        progressState = progressState + " Saving Texture";
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

        yield return null;
    }

    private IEnumerator BinarizeTexture(Texture2D tx, int width, int height)
    {
        progressState = progressState + " Binarizing Texture";

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Color rgbValue = tx.GetPixel(x, y);
                if (rgbValue[0] > 0.8 && rgbValue[1] > 0.8 && rgbValue[2] > 0.8) //색깔에 따른 인식 개선 필요
                {
                    tx.SetPixel(x, y, Color.white);
                }
                else
                {
                    tx.SetPixel(x, y, Color.black);
                }
            }

            if (x % 10 == 0)
            {
                yield return null;
            }
        }
    }

    private IEnumerator ScoreDrawingByGrid(Texture2D answerTx, Texture2D userDrawingTx, int gridScale)
    {
        progressState = "Scoring Laser Drawing";

        if (answerTx.width != userDrawingTx.width || answerTx.height != userDrawingTx.height)
        {
            UnityEngine.Debug.Log("Error : ScoringDrawingByGrid() / Texture Size Difference");
            yield break;
        }

        int maxCnt = 0;
        int hitCnt = 0;

        for (int r = 0; r < answerTx.height - gridScale; r = r + gridScale)
        {
            for (int c = 0; c < answerTx.width - gridScale; c = c + gridScale)
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

            yield return null;
        }
        simularity = (float)hitCnt / (float)maxCnt;
    }

    //================================PUBLIC=================================================

    public IEnumerator StartJudge(int stageNum)
    {
        InitScoreVar();

        yield return StartCoroutine(ExtractAnswerMap(stageNum));

        Texture2D answerTx = GetAnswerTexture(stageNum);

        progressState = "Getting User Drawing";

        yield return StartCoroutine(GetUserDrawingTexture());

        if (answerTx == null)
        {
            UnityEngine.Debug.Log("File Doesn't Exist! : " + SCPath + "Answer_Stage" + stageNum + "  Generate Answer File First!");
        }
        yield return StartCoroutine(ScoreDrawingByGrid(answerTx, userDrawingTexture, 10));

        isScoreLoad = true;
        progressState = null;
    }

    public (int, float) GetScore()
    {
        if (!isScoreLoad)
        {
            UnityEngine.Debug.Log("ScoreJudgement : Score Not Loaded Yet");
            return (0, 0.0f);
        }

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

    public IEnumerator ExtractAnswerMap(int stageNum)
    {
        progressState = "Extracting Answer Map";

        int[] maskLst = { LayerMask.NameToLayer("Default") };
        Texture2D screenCapture = CaptureScreenAsTexture(maskLst);

        yield return StartCoroutine(BinarizeTexture(screenCapture, resWidth, resHeight));

        progressState = "Extracting Answer Map";

        yield return StartCoroutine(SaveTextureAsPNG(screenCapture, "Answer_Stage" + stageNum));

        progressState = null;
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
