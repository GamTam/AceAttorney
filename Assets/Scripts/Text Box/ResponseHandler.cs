using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ResponseHandler : MonoBehaviour
{
    [SerializeField] private RectTransform responseButton;
    private DialogueManager _dialogueManager;
    
    private void Start()
    {
        _dialogueManager = GameObject.FindWithTag("Dialogue Manager").GetComponent<DialogueManager>();
        _dialogueManager._responseHandler = this;
    }

    public void ShowResponses(Response[] responses)
    {
        foreach (Response response in responses)
        {
            RectTransform responseButton = Instantiate(this.responseButton);
            responseButton.gameObject.SetActive(true);
            responseButton.parent = gameObject.GetComponent<RectTransform>();
            responseButton.localScale = new Vector3(1, 1, 1);
            responseButton.gameObject.GetComponentInChildren<TMP_Text>().text = response.ResponseText;
            responseButton.gameObject.GetComponent<Button>().onClick.AddListener(() => OnPickedResponse(response));
        }

        gameObject.GetComponentInParent<Image>().enabled = true;
    }

    private void OnPickedResponse(Response response)
    {
        _dialogueManager.StartText(response.DialogueObject);
    }
}
