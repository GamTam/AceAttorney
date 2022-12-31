using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TitleScreenMenu : MenuCursor
{
    [SerializeField] public GameObject _fade;
    [SerializeField] public string _song;
    
    private MusicManager _musicManager;
    void Start()
    {
        base.Start();
        _musicManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<MusicManager>();
        _musicManager.Play(_song);
    }

    public void Click()
    {
        base.Click();
        switch (EventSystem.current.currentSelectedGameObject.name)
        {
            case "Start":
                StartCoroutine(Begin());
                break;
            case "Quit":
                StartCoroutine(Kill());
                break;
        }
    }

    IEnumerator Begin()
    {
        yield return new WaitForSeconds(1);
        _musicManager.fadeOut(3);
        GameObject obj = Instantiate(_fade);
        SceneTransition trans = obj.GetComponent<SceneTransition>();
        trans._speed = 0.5f;
        trans._destination = "Wright & Co. Law Offices";
    }

    IEnumerator Kill()
    {
        yield return new WaitForSeconds(1);
        Application.Quit();
    }
}
