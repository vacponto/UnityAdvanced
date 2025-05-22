using System.Collections;
using UnityEngine;
using TMPro; 

public class TaskTrigger : MonoBehaviour
{
    [Header("Task Settings")]
    public string displayText = "Enter your text here...";
    public float textSpeed = 0.05f;
    public float disappearSpeed = 0.03f; 
    public TMP_Text textComponent;
    public float displayDurationAfterExit = 3f;
    public bool disableAfterTrigger = true;

    private bool isDisplaying = false;
    private bool isDisappearing = false;
    private bool hasBeenTriggered = false;
    private Coroutine textCoroutine;
    private Collider triggerCollider;
    private string fullText = "";

    private void Awake()
    {
        triggerCollider = GetComponent<Collider>();
        if (triggerCollider == null)
        {
            Debug.LogWarning("No collider found on this GameObject. Adding BoxCollider.");
            triggerCollider = gameObject.AddComponent<BoxCollider>();
        }
        triggerCollider.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isDisplaying && !hasBeenTriggered)
        {
            if (textCoroutine != null)
            {
                StopCoroutine(textCoroutine);
            }
            textCoroutine = StartCoroutine(DisplayText());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && isDisplaying && !isDisappearing)
        {
            StartCoroutine(StartDisappearingAfterDelay());
        }
    }

    private IEnumerator DisplayText()
    {
        isDisplaying = true;
        textComponent.text = "";
        fullText = displayText;

        for (int i = 0; i < fullText.Length; i++)
        {
            textComponent.text = fullText.Substring(0, i + 1);
            yield return new WaitForSeconds(textSpeed);
        }

        isDisplaying = false;
        hasBeenTriggered = true;

        if (disableAfterTrigger)
        {
            StartCoroutine(StartDisappearingAfterDelay());
        }
    }

    private IEnumerator StartDisappearingAfterDelay()
    {
        yield return new WaitForSeconds(displayDurationAfterExit);

        if (textCoroutine != null)
        {
            StopCoroutine(textCoroutine);
        }
        textCoroutine = StartCoroutine(DisappearText());
    }

    private IEnumerator DisappearText()
    {
        isDisappearing = true;
        int currentLength = textComponent.text.Length;

        while (currentLength > 0)
        {
            currentLength--;
            textComponent.text = fullText.Substring(0, currentLength);
            yield return new WaitForSeconds(disappearSpeed);
        }

        textComponent.text = "";
        isDisappearing = false;

        if (disableAfterTrigger)
        {
            triggerCollider.enabled = false;
        }
    }


}