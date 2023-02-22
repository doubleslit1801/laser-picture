using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingData : MonoBehaviour
{
    public float mainVolume = 1f, bgmVolume = 1f, sfxVolume = 1f;
    public float arrowMoveRate = 0.1f;
    public Color answerDrawingColor = Color.white;
    public Material BGMaterial = null;
    public Vector3 BGScale = new Vector3(3, 1, 3);

    public static SettingData Instance;
    
    void Awake()
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
}
