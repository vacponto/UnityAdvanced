using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MainMenuNavigation : MonoBehaviour
{
    [System.Serializable]
    public class SliderValueChangedEvent : UnityEvent<float> { }

    public SliderValueChangedEvent onSliderValueChanged1;
    public SliderValueChangedEvent onSliderValueChanged2;
    public SliderValueChangedEvent onSliderValueChanged3;
    public SliderValueChangedEvent onSliderValueChanged4;

    [Header("Slider References")]
    public Slider sliderSens; // Sensitivity
    public Slider sliderMV; // Master Volume
    public Slider sliderBMV; // Music Volume
    public Slider sliderSFXV; // SFX Volume

    [Header("Text References")]
    public TextMeshProUGUI sliderValueText1;
    public TextMeshProUGUI sliderValueText2;
    public TextMeshProUGUI sliderValueText3;
    public TextMeshProUGUI sliderValueText4;

    [Header("Slider Value Format")]
    public string format = "0";

    private bool isUpdatingSlider = false;

    void Start()
    {
        SaveGameSingleton.Instance.OnSettingsLoaded.AddListener(ApplyLoadedSettings);
        SaveGameSingleton.Instance.LoadSettings();
    }

    private void ApplyLoadedSettings(SaveData settings)
    {
        Debug.Log($"Loaded settings: Sensitivity={settings.mouseSensitivity}, MasterVol={settings.masterVolume}, MusicVol={settings.musicVolume}, SFXVol={settings.sfxVolume}");

        isUpdatingSlider = true;

        if (sliderSens != null)
        {
            sliderSens.value = settings.mouseSensitivity;
            sliderValueText1.text = settings.mouseSensitivity.ToString(format);
        }

        if (sliderMV != null)
        {
            sliderMV.value = settings.masterVolume;
            sliderValueText2.text = settings.masterVolume.ToString(format);
        }

        if (sliderBMV != null)
        {
            sliderBMV.value = settings.musicVolume;
            sliderValueText3.text = settings.musicVolume.ToString(format);
        }

        if (sliderSFXV != null)
        {
            sliderSFXV.value = settings.sfxVolume;
            sliderValueText4.text = settings.sfxVolume.ToString(format);
        }

        isUpdatingSlider = false;
    }

    public void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    void Awake()
    {
        if (sliderSens != null)
            sliderSens.onValueChanged.AddListener(HandleSlider1ValueChanged);
        if (sliderMV != null)
            sliderMV.onValueChanged.AddListener(HandleSlider2ValueChanged);
        if (sliderBMV != null)
            sliderBMV.onValueChanged.AddListener(HandleSlider3ValueChanged);
        if (sliderSFXV != null)
            sliderSFXV.onValueChanged.AddListener(HandleSlider4ValueChanged);
    }

    private void HandleSlider1ValueChanged(float value)
    {
        if (isUpdatingSlider) return;

        isUpdatingSlider = true;
        UpdateSliderText(sliderValueText1, value);
        SaveGameSingleton.Instance.SetMouseSensitivity(value);
        onSliderValueChanged1?.Invoke(value);
        isUpdatingSlider = false;
    }

    private void HandleSlider2ValueChanged(float value)
    {
        if (isUpdatingSlider) return;

        isUpdatingSlider = true;
        UpdateSliderText(sliderValueText2, value);
        SaveGameSingleton.Instance.SetMasterVolume(value);
        onSliderValueChanged2?.Invoke(value);
        isUpdatingSlider = false;
    }

    private void HandleSlider3ValueChanged(float value)
    {
        if (isUpdatingSlider) return;

        isUpdatingSlider = true;
        UpdateSliderText(sliderValueText3, value);
        SaveGameSingleton.Instance.SetMusicVolume(value);
        onSliderValueChanged3?.Invoke(value);
        isUpdatingSlider = false;
    }

    private void HandleSlider4ValueChanged(float value)
    {
        if (isUpdatingSlider) return;

        isUpdatingSlider = true;
        UpdateSliderText(sliderValueText4, value);
        SaveGameSingleton.Instance.SetSFXVolume(value);
        onSliderValueChanged4?.Invoke(value);
        isUpdatingSlider = false;
    }

    private void UpdateSliderText(TextMeshProUGUI textComponent, float value)
    {
        if (textComponent != null)
            textComponent.text = value.ToString(format);
    }
}