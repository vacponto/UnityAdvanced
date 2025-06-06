using UnityEngine;

public class DoorInteraction : MonoBehaviour, IInteractable
{
    [Header("Animation Settings")]
    [SerializeField] private Animator doorAnimator;
    [SerializeField] private float openAngle = 90f;
    [SerializeField] private float closeAngle = 0f;
    [SerializeField] private float animationSpeed = 1f;
    [SerializeField] private bool clockwiseOpen = true;

    private bool isOpen = false;
    private float currentRotation;

    public string InteractionPrompt => isOpen ? "Close" : "Open";
    public bool IsInteractable => true;

    void Start()
    {
        if (doorAnimator == null)
        {
            doorAnimator = GetComponent<Animator>();
            if (doorAnimator == null)
            {
                Debug.LogError("No Animator component found on the door!");
            }
        }

        // Initialize rotation
        currentRotation = closeAngle;
        UpdateDoorRotation();
    }

    public void OnHover() { }
    public void OnUnhover() { }

    public void OnInteract()
    {
        ToggleDoor();
    }

    void ToggleDoor()
    {
        if (!isOpen)
        {
            OpenDoor();
        }
        else
        {
            CloseDoor();
        }
    }

    void OpenDoor()
    {
        float targetAngle = clockwiseOpen ? openAngle : -openAngle;
        doorAnimator.SetFloat("Speed", animationSpeed);
        doorAnimator.SetTrigger("Open");
        isOpen = true;
    }

    void CloseDoor()
    {
        doorAnimator.SetFloat("Speed", animationSpeed);
        doorAnimator.SetTrigger("Close");
        isOpen = false;
    }


    void UpdateDoorRotation()
    {
        transform.localRotation = Quaternion.Euler(0, currentRotation, 0);
    }


    public void SetDoorRotation(float rotationProgress)
    {
        float targetAngle = isOpen ?
            (clockwiseOpen ? openAngle : -openAngle) :
            closeAngle;

        currentRotation = Mathf.Lerp(0, targetAngle, rotationProgress);
        UpdateDoorRotation();
    }
}