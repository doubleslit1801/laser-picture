using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class InGameSettings : MonoBehaviour
{
    public GameObject canvasObj, colorControl, pauseBlock, returnPanel;
    public Slider mainSlider, bgmSlider, sfxSlider, redSlider, greenSlider, blueSlider;
    public TMP_InputField mainInputField, bgmInputField, sfxInputField, redInputField, greenInputField, blueInputField, arrowRateField;
    public UnityEngine.UI.Image circlePalette, picker, selectedColor;
    public Material[] BGMaterials;
    public Renderer planeRenderer;
    [HideInInspector] public Color answerDrawingColor;

    private float mainVolume, bgmVolume, sfxVolume;
    private float arrowMoveRate;
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
            SettingData.Instance.bgmVolume = bgmVolume;
            soundManager.SetBGMVolume(mainVolume * bgmVolume);
            bgmInputField.text = ((int)(bgmVolume * 100f)).ToString();
        }
        if (sfxSlider.value != sfxVolume)
        {
            sfxVolume = sfxSlider.value;
            SettingData.Instance.sfxVolume = sfxVolume;
            soundManager.SetSFXVolume(mainVolume * sfxVolume);
            sfxInputField.text = ((int)(sfxVolume * 100f)).ToString();
        }
    }

    private void InitSettingData()
    {
        mainVolume = SettingData.Instance.mainVolume;
        bgmVolume = SettingData.Instance.bgmVolume;
        sfxVolume = SettingData.Instance.sfxVolume;

        arrowMoveRate = SettingData.Instance.arrowMoveRate;



        answerDrawingColor = SettingData.Instance.answerDrawingColor;

        SetAnswerColor(answerDrawingColor);

        selectedColor.color = answerDrawingColor;

        redSlider.value = (int)(answerDrawingColor.r * 255);
        greenSlider.value = (int)(answerDrawingColor.g * 255);
        blueSlider.value = (int)(answerDrawingColor.b * 255);

        redInputField.text = ((int)redSlider.value).ToString();
        greenInputField.text = ((int)greenSlider.value).ToString();
        blueInputField.text = ((int)blueSlider.value).ToString();

        arrowRateField.text = arrowMoveRate.ToString();

        if (SettingData.Instance.BGMaterial != null)
        {
            planeRenderer.material = SettingData.Instance.BGMaterial;
        }

        SetBGScale(SettingData.Instance.BGScale);
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

        redSlider.value = (int)(answerDrawingColor.r * 255);
        greenSlider.value = (int)(answerDrawingColor.g * 255);
        blueSlider.value = (int)(answerDrawingColor.b * 255);

        redInputField.text = ((int)redSlider.value).ToString();
        greenInputField.text = ((int)greenSlider.value).ToString();
        blueInputField.text = ((int)blueSlider.value).ToString();
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
        if (CheckInGameScene())
        {
            GameObject answerDrawingObj = GameObject.Find("Drawing");
            if (answerDrawingObj != null)
            {
                answerDrawingObj.GetComponent<MeshRenderer>().material.color = color;
            }
        }
    }

    private bool CheckInGameScene()
    {
        return SceneManager.GetActiveScene().name == "InGameUITestScene";
    }

    private void SetBGMaterial(Material mat)
    {
        if (CheckInGameScene())
        {
            planeRenderer.material = mat;
        }
    }

    private void SetBGScale(Vector3 scale)
    {
        if (CheckInGameScene())
        {
            planeRenderer.gameObject.transform.localScale = scale;
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
        if (CheckInGameScene())
        {
            canvasObj.GetComponent<InGameUI>().Resume();
        }
        else
        {
            pauseBlock.SetActive(false);
        }
        gameObject.SetActive(false);
    }

    public void SetBGMaterial1()
    {
        SettingData.Instance.BGMaterial = BGMaterials[0];
        SetBGMaterial(BGMaterials[0]);

        Vector3 bgScale = new Vector3(3, 1, 3);
        SettingData.Instance.BGScale = bgScale;
        SetBGScale(bgScale);
    }

    public void SetBGMaterial2()
    {
        SettingData.Instance.BGMaterial = BGMaterials[1];
        SetBGMaterial(BGMaterials[1]);

        Vector3 bgScale = new Vector3(6.4f, 1, 2);
        SettingData.Instance.BGScale = bgScale;
        SetBGScale(bgScale);
    }

    public void SetBGMaterial3()
    {
        SettingData.Instance.BGMaterial = BGMaterials[2];
        SetBGMaterial(BGMaterials[2]);

        Vector3 bgScale = new Vector3(2.9f, 1, 2);
        SettingData.Instance.BGScale = bgScale;
        SetBGScale(bgScale);
    }

    public void InputFieldOnEndEdit()
    {
        mainVolume = (float)int.Parse(mainInputField.text) / 100f;
        bgmVolume = (float)int.Parse(bgmInputField.text) / 100f;
        sfxVolume = (float)int.Parse(sfxInputField.text) / 100f;

        arrowMoveRate = float.Parse(arrowRateField.text);

        mainSlider.value = mainVolume;
        bgmSlider.value = bgmVolume;
        sfxSlider.value = sfxVolume;

        SettingData.Instance.mainVolume = mainVolume;
        SettingData.Instance.bgmVolume = bgmVolume;
        SettingData.Instance.sfxVolume = sfxVolume;

        SettingData.Instance.arrowMoveRate = arrowMoveRate;

        soundManager.SetBGMVolume(mainVolume * bgmVolume);
        soundManager.SetSFXVolume(mainVolume * sfxVolume);
    }

    public void ActiveReturnPanel()
    {
        returnPanel.SetActive(true);
    }

    public void DeactiveReturnPanel()
    {
        returnPanel.SetActive(false);
    }

    public void ReturnStageSelectScene()
    {
        transform.root.GetComponent<InGameUI>().MoveCameraAfterSceneChange();
    }

    public void ColorSliderChanged()
    {
        Color color = new Color(redSlider.value / 255f, greenSlider.value / 255f, blueSlider.value / 255f);

        SetAnswerColor(color);

        selectedColor.color = color;

        SettingData.Instance.answerDrawingColor = color;

        redInputField.text = ((int)redSlider.value).ToString();
        greenInputField.text = ((int)greenSlider.value).ToString();
        blueInputField.text = ((int)blueSlider.value).ToString();
    }

    public void ColorInputFieldChanged()
    {
        float red = (float)int.Parse(redInputField.text);
        float green = (float)int.Parse(greenInputField.text);
        float blue = (float)int.Parse(blueInputField.text);

        if (red > 255)
        {
            red = 255;
        }
        if (green > 255)
        {
            green = 255;
        }
        if (blue > 255)
        {
            blue = 255;
        }

        Color color = new Color(red / 255f, green / 255f, blue / 255f);

        SetAnswerColor(color);

        selectedColor.color = color;

        SettingData.Instance.answerDrawingColor = color;

        redSlider.value = red;
        greenSlider.value = green;
        blueSlider.value = blue;
    }
}
