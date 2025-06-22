using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class CutsceneTrigger : MonoBehaviour
{
    [SerializeField] private PlayableDirector timelineToPlay;

    private void OnTriggerEnter(Collider other)
    {
        if (timelineToPlay != null)
        {
            timelineToPlay.Play();
            Destroy(gameObject);
        }
    }
}
