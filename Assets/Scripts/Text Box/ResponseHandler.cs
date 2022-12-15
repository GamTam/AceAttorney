using System;
using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ResponseHandler : MonoBehaviour
{
    [SerializeField] private RectTransform responseButton;
    [SerializeField] private GameObject _selectionHead;

    private PlayerInput _playerInput;
    private DialogueManager _dialogueManager;
    private bool _shownResponses;

    private Navigation _nav = new Navigation();
    
    private void Start()
    {
        _dialogueManager = GameObject.FindWithTag("Dialogue Manager").GetComponent<DialogueManager>();
        _playerInput = GameObject.FindWithTag("Controller Manager").GetComponent<PlayerInput>();
        _dialogueManager._responseHandler = this;

        _nav.wrapAround = true;
        _nav.mode = Navigation.Mode.Vertical;
    }

    public void ShowResponses(Response[] responses)
    {
        
        _selectionHead.SetActive(true);
        int i = 0;
        foreach (Response response in responses)
        {
            RectTransform responseButton = Instantiate(this.responseButton);
            responseButton.gameObject.SetActive(true);
            responseButton.parent = gameObject.GetComponent<RectTransform>();
            responseButton.localScale = new Vector3(1, 1, 1);
            responseButton.gameObject.GetComponentInChildren<TMP_Text>().text = response.ResponseText;
            Button but = responseButton.gameObject.GetComponent<Button>();
            but.onClick.AddListener(() => StartCoroutine(OnPickedResponse(response)));
            but.navigation = _nav;
            
            if (i==0) EventSystem.current.SetSelectedGameObject(responseButton.gameObject);
            i++;
        }
        
        gameObject.GetComponentInParent<Image>().enabled = true;
        _playerInput.SwitchCurrentActionMap("Menu");
    }

    private IEnumerator OnPickedResponse(Response response)
    {
        foreach (GameObject but in GameObject.FindGameObjectsWithTag("Button"))
        {
             if (but != EventSystem.current.currentSelectedGameObject) but.GetComponent<Animator>().Play("Fade Out");
        }
        yield return new WaitForSeconds(0.5f);
        _playerInput.SwitchCurrentActionMap("TextBox");
        _dialogueManager.StartText(response.DialogueObject, prevActionMap:_dialogueManager._prevActionMap);
        _selectionHead.SetActive(false);
    }
}
