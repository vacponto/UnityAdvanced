using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GrabbableItem : MonoBehaviour, IInteractable
{
    [SerializeField] private string itemName = "Item";
    [SerializeField] private Transform grabPosition;

    public string InteractionPrompt => $"Press to grab the {itemName}";
    public bool IsInteractable => true;


    private bool isGrabbed = false;
    private Transform originalParent;
    private Vector3 originalPosition;
    private Outline outline;

    private void Start()
    {
        originalParent = transform.parent;
        originalPosition = transform.position;
        outline = GetComponent<Outline>();
        outline.enabled = false;
    }

    public void OnHover()
    {
        if (!outline.enabled)
        {
            
            outline.enabled = true; 
        }
    }

    public void OnUnhover()
    {
        if (outline.enabled)
        {
            outline.enabled = false;
        }
    }

    public void OnInteract()
    {
        if (isGrabbed)
        {
            ReleaseItem();
        }
        else
        {
            GrabItem();
        }
    }
    
    private void GrabItem()
    {
        isGrabbed = true;
        transform.SetParent(grabPosition);
        transform.localRotation = Quaternion.Euler(-90f, -90f, 0f);
        transform.localPosition = Vector3.zero;
        GetComponent<Rigidbody>().isKinematic = true;
        gameObject.layer = LayerMask.NameToLayer("ItemInHand");
    }

    private void ReleaseItem()
    {
        isGrabbed = false;
        transform.SetParent(originalParent);
        transform.position = originalPosition;
        GetComponent<Rigidbody>().isKinematic = false;
    }
}
