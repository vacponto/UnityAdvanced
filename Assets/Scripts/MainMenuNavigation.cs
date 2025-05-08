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
    public Slider slider1;
    public Slider slider2;
    public Slider slider3;
    public Slider slider4;

    [Header("Text References")]
    public TextMeshProUGUI sliderValueText1;
    public TextMeshProUGUI sliderValueText2;
    public TextMeshProUGUI sliderValueText3;
    public TextMeshProUGUI sliderValueText4;

    [Header("Slider Value Format")]
    public string format = "0"; 

    private bool isUpdatingSlider = false;

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
        if (slider1 != null)
            slider1.onValueChanged.AddListener(HandleSlider1ValueChanged);
        if (slider2 != null)
            slider2.onValueChanged.AddListener(HandleSlider2ValueChanged);
        if (slider3 != null)
            slider3.onValueChanged.AddListener(HandleSlider3ValueChanged);
        if (slider4 != null)
            slider4.onValueChanged.AddListener(HandleSlider4ValueChanged);

        if (sliderValueText1 != null) sliderValueText1.text = slider1.value.ToString(format);
        if (sliderValueText2 != null) sliderValueText2.text = slider2.value.ToString(format);
        if (sliderValueText3 != null) sliderValueText3.text = slider3.value.ToString(format);
        if (sliderValueText4 != null) sliderValueText4.text = slider4.value.ToString(format);
    }

    private void HandleSlider1ValueChanged(float value)
    {
        if (isUpdatingSlider) return;

        isUpdatingSlider = true;
        UpdateSliderText(sliderValueText1, value);
        onSliderValueChanged1?.Invoke(value);
        isUpdatingSlider = false;
    }

    private void HandleSlider2ValueChanged(float value)
    {
        if (isUpdatingSlider) return;

        isUpdatingSlider = true;
        UpdateSliderText(sliderValueText2, value);
        onSliderValueChanged2?.Invoke(value);
        isUpdatingSlider = false;
    }

    private void HandleSlider3ValueChanged(float value)
    {
        if (isUpdatingSlider) return;

        isUpdatingSlider = true;
        UpdateSliderText(sliderValueText3, value);
        onSliderValueChanged3?.Invoke(value);
        isUpdatingSlider = false;
    }

    private void HandleSlider4ValueChanged(float value)
    {
        if (isUpdatingSlider) return;

        isUpdatingSlider = true;
        UpdateSliderText(sliderValueText4, value);
        onSliderValueChanged4?.Invoke(value);
        isUpdatingSlider = false;
    }

    private void UpdateSliderText(TextMeshProUGUI textComponent, float value)
    {
        if (textComponent != null)
            textComponent.text = value.ToString(format);
    }
}
