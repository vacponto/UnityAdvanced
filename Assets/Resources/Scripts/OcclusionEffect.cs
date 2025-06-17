using UnityEngine;

public class OcclusionEffect : MonoBehaviour
{
    public GameObject objectToHide;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && objectToHide != null)
        {
            objectToHide.SetActive(false);
        }
    }
}