using System.Collections.Generic;
using UnityEngine;

public class FootstepSoundPool : MonoBehaviour
{
    public AudioSource footstepPrefab;
    public int poolSize = 10;

    private Queue<AudioSource> pool = new Queue<AudioSource>();

    void Awake()
    {
        for (int i = 0; i < poolSize; i++)
        {
            AudioSource source = Instantiate(footstepPrefab, transform);
            source.gameObject.SetActive(false);
            pool.Enqueue(source);
        }
    }

    public void PlayFootstep(Vector3 position)
    {
        AudioSource source = pool.Dequeue();
        source.transform.position = position;
        source.gameObject.SetActive(true);
        source.Play();

        StartCoroutine(ReturnToPool(source, source.clip.length));
    }

    private System.Collections.IEnumerator ReturnToPool(AudioSource source, float delay)
    {
        yield return new WaitForSeconds(delay);
        source.gameObject.SetActive(false);
        pool.Enqueue(source);
    }
}

