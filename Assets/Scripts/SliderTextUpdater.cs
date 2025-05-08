using UnityEngine;
using TMPro;

public class SliderTextUpdater : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI targetText;
    [SerializeField] private string format = "0";

    public void UpdateText(float value)
    {
        if (targetText != null)
            targetText.text = value.ToString(format);
    }
}
