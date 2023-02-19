using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InGameSettings : MonoBehaviour
{
    public GameObject canvasObj, colorControl;
    public Slider mainSlider, bgmSlider, sfxSlider;
    public TMP_InputField mainInputField, bgmInputField, sfxInputField;
    public UnityEngine.UI.Image circlePalette, picker, selectedColor;
    public Material[] BGMaterials;
    public Renderer planeRenderer;
    [HideInInspector] public Color answerDrawingColor;

    private float mainVolume, bgmVolume, sfxVolume;
    private SoundManager soundManager;
    private Vector2 sizeOfPalette;
    private CircleCollider2D paletteCollider;
    private RectTransform colorControlRT, circlePaletteRT, pickerRT;

    void Start()
    {
        soundManager = SoundManager.Instance.GetComponent<SoundManager>();

        InitSettingData();

        mainSlider.value = mainVolume;
        bgmSlider.value = bgmVolume;
        sfxSlider.value = sfxVolume;

        mainInputField.text = ((int)(mainVolume * 100f)).ToString();
        bgmInputField.text = ((int)(bgmVolume * 100f)).ToString();
        sfxInputField.text = ((int)(sfxVolume * 100f)).ToString();

        paletteCollider = circlePalette.GetComponent<CircleCollider2D>();

        colorControlRT = colorControl.GetComponent<RectTransform>();
        circlePaletteRT = circlePalette.GetComponent<RectTransform>();
        pickerRT = picker.GetComponent<RectTransform>();

        sizeOfPalette = new Vector2(
            circlePalette.GetComponent<RectTransform>().rect.width,
            circlePalette.GetComponent<RectTransform>().rect.height);
    }

    // Update is called once per frame
    void Update()
    {
        if (mainSlider.value != mainVolume)
        {
            mainVolume = mainSlider.value;
            SettingData.Instance.mainVolume = mainVolume;
            soundManager.SetBGMVolume(mainVolume * bgmVolume);
            soundManager.SetSFXVolume(mainVolume * sfxVolume);
            mainInputField.text = ((int)(mainVolume * 100f)).ToString();
        }
        if (bgmSlider.value != bgmVolume)
        {
            bgmVolume = bgmSlider.value;
            soundManager.SetBGMVolume(mainVolume * bgmVolume);
            bgmInputField.text = ((int)(bgmVolume * 100f)).ToString();
        }
        if (sfxSlider.value != sfxVolume)
        {
            sfxVolume = sfxSlider.value;
            soundManager.SetSFXVolume(mainVolume * sfxVolume);
            sfxInputField.text = ((int)(sfxVolume * 100f)).ToString();
        }
    }

    private void InitSettingData()
    {
        mainVolume = SettingData.Instance.mainVolume;
        bgmVolume = soundManager.GetBGMVolume();
        sfxVolume = soundManager.GetSFXVolume();



        answerDrawingColor = SettingData.Instance.answerDrawingColor;

        SetAnswerColor(answerDrawingColor);

        if (SettingData.Instance.BGMaterial != null)
        {
            planeRenderer.material = SettingData.Instance.BGMaterial;
        }
    }

    private void selectColor()
    {
        Vector3 offset = Input.mousePosition - colorControlRT.localPosition - circlePaletteRT.localPosition - new Vector3(Screen.width, Screen.height, 0) / 2;
        Vector3 diff = Vector3.ClampMagnitude(offset, paletteCollider.radius);

        pickerRT.localPosition = diff;

        answerDrawingColor = getColor();

        selectedColor.color = answerDrawingColor;

        SetAnswerColor(answerDrawingColor);

        SettingData.Instance.answerDrawingColor = answerDrawingColor;
    }

    private Color getColor()
    {
        Vector2 pickerPosition = pickerRT.anchoredPosition;

        Vector2 position = pickerPosition + sizeOfPalette * 0.5f;
        Vector2 normalized = new Vector2(
            (position.x / (circlePaletteRT.rect.width)),
            (position.y / (circlePaletteRT.rect.height)));

        Texture2D texture = circlePalette.mainTexture as Texture2D;
        Color circularSelectedColor = texture.GetPixelBilinear(normalized.x, normalized.y);

        return circularSelectedColor;
    }

    private void SetAnswerColor(Color color)
    {
        GameObject answerDrawingObj = GameObject.Find("AnswerDrawing");
        if (answerDrawingObj != null)
        {
            answerDrawingObj.GetComponent<MeshRenderer>().material.color = color;
        }
    }

    public void mousePointerDown()
    {
        selectColor();
    }

    public void mouseDrag()
    {
        selectColor();
    }

    public void ExitSettings()
    {
        canvasObj.GetComponent<InGameUI>().Resume();
        gameObject.SetActive(false);
    }

    public void SetBGMaterial1()
    {
        planeRenderer.material = BGMaterials[0];
        SettingData.Instance.BGMaterial = BGMaterials[0];
    }

    public void SetBGMaterial2()
    {
        planeRenderer.material = BGMaterials[1];
        SettingData.Instance.BGMaterial = BGMaterials[1];
    }

    public void InputFieldOnEndEdit()
    {
        mainVolume = (float)int.Parse(mainInputField.text) / 100f;
        bgmVolume = (float)int.Parse(bgmInputField.text) / 100f;
        sfxVolume = (float)int.Parse(sfxInputField.text) / 100f;

        mainSlider.value = mainVolume;
        bgmSlider.value = bgmVolume;
        sfxSlider.value = sfxVolume;

        SettingData.Instance.mainVolume = mainVolume;
        soundManager.SetBGMVolume(mainVolume * bgmVolume);
        soundManager.SetSFXVolume(mainVolume * sfxVolume);
    }
}
