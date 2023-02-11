using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameSettings : MonoBehaviour
{
    public GameObject canvasObj, soundManagerObject;
    public Slider mainSlider, bgmSlider, sfxSlider;
    
    private float mainVolume, bgmVolume, sfxVolume;
    private SoundManager soundManager;

    void Start()
    {
        mainVolume = 0;
        bgmVolume = 0;
        sfxVolume = 0;

        soundManager = soundManagerObject.GetComponent<SoundManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (mainSlider.value != mainVolume)
        {
            mainVolume = mainSlider.value;
            soundManager.SetBGMVolume(mainVolume * bgmVolume);
            soundManager.SetSFXVolume(mainVolume * sfxVolume);
        }
        if (bgmSlider.value != bgmVolume)
        {
            bgmVolume = bgmSlider.value;
            soundManager.SetBGMVolume(mainVolume * bgmVolume);
        }
        if (sfxSlider.value != sfxVolume)
        {
            sfxVolume = sfxSlider.value;
            soundManager.SetSFXVolume(mainVolume * sfxVolume);
        }
    }

    public void ExitSettings()
    {
        canvasObj.GetComponent<InGameUI>().Resume();
        gameObject.SetActive(false);
    }
}
