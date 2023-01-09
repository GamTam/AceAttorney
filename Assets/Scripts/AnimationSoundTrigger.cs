using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationSoundTrigger : MonoBehaviour
{
    private SoundManager _soundManager;

    private void Start()
    {
        _soundManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<SoundManager>();
    }

    public void PlaySound(string sound)
    {
        _soundManager.Play(sound);
    }
}
