using UnityEngine;
using UnityEngine.Playables;

public class FinishPoint : MonoBehaviour
{
    public PlayableDirector playableDirector;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Something entered: " + other.name); 

        if (other.CompareTag("Key"))
        {
            Debug.Log("Key entered!"); 

            if (playableDirector != null)
            {
                playableDirector.Play();
            }
        }
    }

}