using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class Credits : MonoBehaviour
{
    [SerializeField] private VideoPlayer _video;
    [SerializeField] private GameObject _fadeOut;

    private bool PlayedVideo;
    private bool VideoDone;

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(1);
        _video.Play();
        yield return new WaitForSeconds(3);
        PlayedVideo = true;
    }

    void Update()
    {
        if (!_video.isPlaying && PlayedVideo & !VideoDone)
        {
            VideoDone = true;
            GameObject obj = Instantiate(_fadeOut);
            SceneTransition trans = obj.GetComponent<SceneTransition>();
            trans._destination = "Title Screen";
        }
    }
}
