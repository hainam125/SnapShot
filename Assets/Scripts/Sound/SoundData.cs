using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundData : MonoBehaviour
{
    public AudioClip engineDriving;
    public AudioClip engineIdling;

    private static SoundData instance;
    public static SoundData Instance
    {
        get
        {
            if (instance == null) instance = FindObjectOfType<SoundData>();
            return instance;
        }
    }
}
