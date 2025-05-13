using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CrosshairController : MonoBehaviour
{
    [SerializeField] private Image defaultCrosshair;  
    [SerializeField] private Image interactCrosshair;
    [SerializeField] private Text interactionText;
    [SerializeField] private Color interactableColor = Color.green;

    private void Start()
    {
        defaultCrosshair.enabled = true;
        interactCrosshair.enabled = false;
        interactionText.text = "";
    }

    public void SetNormal()
    {
        defaultCrosshair.enabled = true;
        interactCrosshair.enabled = false;
        interactionText.text = "";
    }

    public void SetInteractable(string prompt)
    {
        defaultCrosshair.enabled = false;
        interactCrosshair.enabled = true;
        interactionText.text = prompt;
    }
}
