using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientSound : MonoBehaviour {
    [SerializeField]
    private AudioClip shotFiring;

    private AudioSource source;

	private void Awake () {
        source = GetComponent<AudioSource>();
	}

    public void ShotFire()
    {
        source.PlayOneShot(shotFiring);
    }
}
