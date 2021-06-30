using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    AudioSource audioSource;

    private void Awake()
    {
        if (instance == null) instance = this;
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlaySound(AudioClip clip)
    {
        // Debug.Log("사운드 효과");
        StopSound();
        audioSource.PlayOneShot(clip);
    }

    public void StopSound()
    {
         audioSource.Stop();
    }
}
