using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameSettings : MonoBehaviour
{
    public GameObject canvasObj, soundManagerObject, colorControl;
    public Slider mainSlider, bgmSlider, sfxSlider;
    public UnityEngine.UI.Image circlePalette, picker, selectedColor;
    public Material[] BGMaterials;
    public Renderer planeRenderer;

    private float mainVolume, bgmVolume, sfxVolume;
    private SoundManager soundManager;
    private Vector2 sizeOfPalette;
    private CircleCollider2D paletteCollider;
    private RectTransform colorControlRT, circlePaletteRT, pickerRT;

    void Start()
    {
        mainVolume = 0;
        bgmVolume = 0;
        sfxVolume = 0;

        soundManager = soundManagerObject.GetComponent<SoundManager>();

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

    private void selectColor()
    {
        Vector3 offset = Input.mousePosition - colorControlRT.localPosition - circlePaletteRT.localPosition - new Vector3(Screen.width, Screen.height, 0) / 2;
        Vector3 diff = Vector3.ClampMagnitude(offset, paletteCollider.radius);

        pickerRT.localPosition = diff;

        selectedColor.color = getColor();
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
    }

    public void SetBGMaterial2()
    {
        planeRenderer.material = BGMaterials[1];
    }
}
