using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffectManager : MonoBehaviour
{

    public static AudioClip _moving, _select;
    static AudioSource _audioSrc;

    // Start is called before the first frame update
    void Start()
    {
        _moving = Resources.Load<AudioClip>("move");
        _select = Resources.Load<AudioClip>("select");
        _audioSrc = GetComponent<AudioSource>();
    }

    public static void PlaySound(string clip)
    {
        switch(clip)
        {
            case "move":
                _audioSrc.PlayOneShot(_moving);
                break;
            case "select":
                _audioSrc.PlayOneShot(_select);
                break;
            
        }
    }
}
