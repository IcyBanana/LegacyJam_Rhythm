using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicTest : MonoBehaviour
{
    public AudioClip musicClip;
    public AudioSource myAudioSource;
    public float offset = 2.8f;

    void Start() {
        Invoke("StartPlayback", offset);
    }

    void StartPlayback () {
        myAudioSource.clip = musicClip;
        myAudioSource.Play();
    }
}
