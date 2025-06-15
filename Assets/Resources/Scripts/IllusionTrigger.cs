using UnityEngine;

public class IllusionTrigger : MonoBehaviour
{
    public Transform teleportDestination;

    void Start()
    {
        if (GetComponent<Collider>() == null)
        {
            gameObject.AddComponent<BoxCollider>().isTrigger = true;
        }
        else
        {
            GetComponent<Collider>().isTrigger = true;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("RobotToy"))
        {
            TeleportPlayer();
            Destroy(other.gameObject,3); 
        }
    }

    void TeleportPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null && teleportDestination != null)
        {
            player.transform.position = teleportDestination.position;
            player.transform.rotation = teleportDestination.rotation;
        }
    }
}