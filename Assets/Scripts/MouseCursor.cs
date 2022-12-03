using UnityEngine;
using UnityEngine.InputSystem;

public class MouseCursor : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    [SerializeField] private Sprite _baseSprite;
    [SerializeField] private Sprite _selectedSprite;
    [SerializeField] private Sprite _selectedAgainSprite;
    [SerializeField] private float _moveSpeed = 3;

    [SerializeField] private DialogueTrigger _noClues;

    private Camera _cam;
    private DialogueTrigger _selectedObj;

    private SwapCharacters _swap;

    private PlayerInput _playerInput;
    private InputAction _mousePos;
    private InputAction _vCursor;
    private InputAction _select;
    private InputAction _back;

    private string _char;
    
    void Start()
    {
        _playerInput = GameObject.FindWithTag("Controller Manager").GetComponent<PlayerInput>();
        _cam = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        _swap = GameObject.FindWithTag("CharacterPlane").GetComponent<SwapCharacters>();
        _char = _swap._charName;
        
        transform.position = new Vector3(_cam.transform.position.x, _cam.transform.position.y, transform.position.z);
        _playerInput.SwitchCurrentActionMap("Investigation");

        _mousePos = _playerInput.actions["Investigation/MousePos"];
        _vCursor = _playerInput.actions["Investigation/MoveVector"];
        _select = _playerInput.actions["Investigation/Select"];
        _back = _playerInput.actions["Investigation/Back"];

        _selectedObj = _noClues;
        
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        Cursor.visible = false;
        Vector2 cursorPos;

        if (_playerInput.currentControlScheme == "Mouse")
        {
            cursorPos = _cam.ScreenToWorldPoint(_mousePos.ReadValue<Vector2>());
            transform.position = cursorPos;
        }
        else
        {
            cursorPos = _vCursor.ReadValue<Vector2>();
            transform.position = transform.position + (Vector3) (cursorPos * Time.deltaTime * _moveSpeed);
        }

        if (_select.triggered)
        {
            _selectedObj.TriggerDialogue();
            _spriteRenderer.enabled = false;
            _spriteRenderer.sprite = _selectedAgainSprite;
        }

        if (_back.triggered)
        {
            _swap.StartSwap(_char);
            _playerInput.SwitchCurrentActionMap("Menu");
            Destroy(gameObject);
            Cursor.visible = true;
        }

        if (_playerInput.currentActionMap.name == "Investigation" && !_spriteRenderer.enabled)
        {
            if (_selectedObj == _noClues)
            {
                _spriteRenderer.sprite = _baseSprite;
            }
            _swap.StartSwap(fadeIn:false);
            _spriteRenderer.enabled = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Examinable")
        {
            DialogueTrigger trigger = other.GetComponent<DialogueTrigger>();
            
            if (trigger._inspected)
            {
                _spriteRenderer.sprite = _selectedAgainSprite;
            }
            else
            {
                _spriteRenderer.sprite = _selectedSprite;
            }
            
            _selectedObj = trigger;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Examinable")
        {
            if (_selectedObj == other.GetComponent<DialogueTrigger>())
            {
                _spriteRenderer.sprite = _baseSprite;
                _selectedObj = _noClues;
            }
        }
    }
}
