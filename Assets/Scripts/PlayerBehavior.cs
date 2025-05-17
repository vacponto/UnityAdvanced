using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBehavior : MonoBehaviour
{
    [Header("Ground Check")]
    public Transform groundCheckPoint;
    public float groundCheckDistance = 0.2f;
    public LayerMask groundMask;

    [Header("Movement Settings")]
    public float speed = 5f;
    public float stepInterval = 0.5f;

    [Header("Camera Settings")]
    public float mouseSensitivity = 2f;
    public Transform cameraTransform;
    public bool invertYLook = false;

    [Header("Head Bob Settings")]
    public float bobSpeed = 10f;
    public float bobAmount = 0.05f;
    private float bobTimer = 0f;
    private Vector3 cameraInitialPos;

    [Header("Footsteps")]
    public FootstepSoundPool footstepSoundPool;

    private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction lookAction;

    private Rigidbody rb;
    private float stepTimer = 0f;
    private Vector3 lastPosition;
    private bool isGrounded;
    private Vector2 currentMovementInput;
    private float verticalRotation = 0f;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["Move"];
        lookAction = playerInput.actions["Look"];

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Start()
    {
        lastPosition = transform.position;
        cameraInitialPos = cameraTransform.localPosition;

        moveAction.performed += OnMovementInput;
        moveAction.canceled += OnMovementInput;
        lookAction.performed += OnLookInput;
        lookAction.canceled += OnLookInput;

        mouseSensitivity = SaveGameSingleton.Instance.GameSettings.mouseSensitivity;
    }

    void OnDisable()
    {
        moveAction.performed -= OnMovementInput;
        moveAction.canceled -= OnMovementInput;
        lookAction.performed -= OnLookInput;
        lookAction.canceled -= OnLookInput;
    }

    void Update()
    {
        GroundCheck();
        HandleFootsteps();
        HandleHeadBob();
    }

    void FixedUpdate()
    {
        Move(currentMovementInput);
    }

    public void SetSensitivity(float sensitivity)
    {
        mouseSensitivity = sensitivity;
    }

    private void OnMovementInput(InputAction.CallbackContext context)
    {
        currentMovementInput = context.ReadValue<Vector2>();
    }

    private void OnLookInput(InputAction.CallbackContext context)
    {
        Vector2 lookInput = context.ReadValue<Vector2>();


        transform.Rotate(Vector3.up * lookInput.x * mouseSensitivity);

        verticalRotation += lookInput.y * mouseSensitivity * (invertYLook ? 1 : -1);
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);
        cameraTransform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
    }

    private void Move(Vector2 direction)
    {
        Vector3 moveDir = (transform.right * direction.x + transform.forward * direction.y).normalized;
        Vector3 velocity = moveDir * speed;
        velocity.y = rb.velocity.y;
        rb.velocity = velocity;
    }

    private void GroundCheck()
    {
        isGrounded = Physics.Raycast(groundCheckPoint.position, Vector3.down, groundCheckDistance, groundMask);
        Debug.Log("Grounded");
    }

    private void HandleFootsteps()
    {

        bool isMoving = (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0);
        float movementSpeed = (transform.position - lastPosition).magnitude / Time.deltaTime;
        lastPosition = transform.position;

        if (isMoving && isGrounded)
        {
            stepTimer += Time.deltaTime;
            if (stepTimer >= stepInterval)
            {
                footstepSoundPool.PlayFootstep(groundCheckPoint.position);
                stepTimer = 0f;
            }
        }
        else
        {
            stepTimer = 0f;
        }
    }

    private void HandleHeadBob()
    {
        Vector3 velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);

        if (velocity.magnitude > 0.1f && isGrounded)
        {
            bobTimer += Time.deltaTime * bobSpeed;
            float bobOffset = Mathf.Sin(bobTimer) * bobAmount;
            cameraTransform.localPosition = cameraInitialPos + new Vector3(0, bobOffset, 0);
        }
        else
        {
            bobTimer = 0f;
            cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, cameraInitialPos, Time.deltaTime * bobSpeed);
        }
    }
}