﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    public AudioClip placementSound;
    public AudioSource audioSource;

    public static AudioPlayer instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(this.gameObject);

    }

    public void PlayPlacementSound()
    {
        if (placementSound != null)
        {
            audioSource.PlayOneShot(placementSound);
        }
    }
}
