using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _speedMove = 5f;
    [SerializeField] private float _jumpForce = 5f;
    [SerializeField] private float _maxFallSpeed = -30f;
    // Gravity Scale
    [SerializeField] private float _gravityScale = -9.81f; // Default gravity value
    [SerializeField] LayerMask _groundLayerMask;
    private float _yVelocity;
    private InputSystem_Actions _actions;
    private CharacterController _controller;
    private Vector2 _moveInput;
    private Vector2 _playerVelocity;

    private bool _isGrounded;
    private bool _isJump;
    
    

    private void Awake()
    {
        // Initialize input actions
        if (_actions == null)
        {
            _actions = new InputSystem_Actions();
            _actions.Enable();
        }

        // Get the character controller component
        _controller = GetComponent<CharacterController>();
    }

    private void OnEnable()
    {
        // Bind input actions to callbacks
        _actions.Player.Enable();
        _actions.Player.Move.performed += OnMove;
        _actions.Player.Move.canceled += OnMove;
        _actions.Player.Jump.performed += OnJump;
        _actions.Player.Jump.canceled += OnJump;
    }

    private void OnDisable()
    {
        _actions.Player.Disable();
        _actions.Player.Move.performed -= OnMove;
        _actions.Player.Move.canceled -= OnMove;
        _actions.Player.Jump.performed -= OnJump;
        _actions.Player.Jump.canceled -= OnJump;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        PlayerMove();
        PhysicGravity();
        
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        _moveInput = context.ReadValue<Vector2>();
        Debug.Log("input: " + _moveInput);
    }

    private void PlayerMove()
    {
        Vector3 pMove = new Vector3(_moveInput.x, 0, 0);
        pMove = transform.TransformDirection(pMove);
        _controller.Move(pMove * _speedMove * Time.deltaTime);
    }

    private void PhysicGravity()
    {
        // Nastavení raycastu
        Vector3 rayOrigin = transform.position; 
        Vector3 rayDirection = -Vector3.up; 
        float rayDistance = 0.1f; // Adjust this distance as needed

        // Provedení raycastu
        if (Physics.Raycast(rayOrigin, rayDirection, out RaycastHit hit, rayDistance, _groundLayerMask))
        {
            _isGrounded = true;
        }
        else
        {
            _isGrounded = false;
        }

        if (!_isGrounded)
        {
            _yVelocity += _gravityScale * Time.deltaTime;

            // Omezit maximální rychlost pádu pouze při volném pádu
            if (_yVelocity < _maxFallSpeed && !_isGrounded) 
            {
                _yVelocity = _maxFallSpeed; 
            }
        }
        else
        {
            _yVelocity = 0f; 
        }

        Debug.Log("yVelocity: " + _yVelocity);
        // Apply gravity
        Vector3 gravity = new Vector3(0, _yVelocity, 0);
        _controller.Move(gravity * Time.deltaTime);
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        _isJump = context.ReadValueAsButton();

        if (_isJump && _controller.isGrounded)
        {
            Debug.LogWarning("jsem na zemi ");
            _yVelocity = Mathf.Sqrt(_jumpForce * -2f * _gravityScale);
            
        }
        else
        {
            Debug.LogWarning("Nejsem na zemi");
        }
    }
}
