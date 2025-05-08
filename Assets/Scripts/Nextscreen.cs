using System.Collections;
using System.Collections.Generic;
using UnityEngine.Playables;
using UnityEngine;

public class Nextscreen : MonoBehaviour
{
    [SerializeField] private PlayableDirector timelineToPlay;
    [SerializeField] private PlayableDirector timelineToStop;

    void Update()
    {
        if (Input.GetMouseButtonDown(0) || Input.touchCount > 0)
        {

            if (timelineToStop != null && timelineToStop.state == PlayState.Playing)
            {
                timelineToStop.Stop();
            }

            if (timelineToPlay != null)
            {
                timelineToPlay.Stop();
                timelineToPlay.Play();
            }
        }
    }
}
