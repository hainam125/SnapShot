using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSound : MonoBehaviour {
    [SerializeField]
    private AudioSource audioSource;

    public void PlayDrivingAudio()
    {
        audioSource.mute = false;
        audioSource.clip = SoundData.Instance.engineDriving;
        audioSource.Play();
    }

    public void PlayIdlingAudio()
    {
        audioSource.mute = false;
        audioSource.clip = SoundData.Instance.engineIdling;
        audioSource.Play();
    }

    public void Mute()
    {
        audioSource.clip = null;
        audioSource.mute = true;
    }
}
