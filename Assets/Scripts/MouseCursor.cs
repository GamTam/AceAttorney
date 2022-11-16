using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseCursor : MonoBehaviour
{
    [SerializeField] private Renderer _cursorRenderer;
    [SerializeField] private GameObject _cursoring;
    [SerializeField] private Color _newColor;

    bool _soTrue = false;

    void Start()
    {
        _cursorRenderer = _cursoring.GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if(_soTrue == true)
        {
            Cursor.visible = false;
            Vector2 cursorPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = cursorPos;
        }
    }

    public void TrueMachine()
    {
        _soTrue = true;
    }
}
