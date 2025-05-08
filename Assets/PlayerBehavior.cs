using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehavior : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 5f;
    public float stepInterval = 0.5f;
    public float groundCheckDistance = 1.1f;
    public LayerMask groundMask;

    [Header("Footsteps")]
    public FootstepSoundPool footstepSoundPool;

    private Rigidbody rb;
    private float stepTimer = 0f;
    private Vector3 lastPosition;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        lastPosition = transform.position;
    }

    void Update()
    {
        GroundCheck();
        HandleFootsteps();
    }

    void FixedUpdate()
    {
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        Move(input);
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
        Ray ray = new Ray(transform.position, Vector3.down);
        isGrounded = Physics.Raycast(ray, groundCheckDistance, groundMask);
    }

    private void HandleFootsteps()
    {
        float movementSpeed = (transform.position - lastPosition).magnitude / Time.deltaTime;
        lastPosition = transform.position;
        //Debug.Log(movementSpeed);
        Debug.Log(isGrounded);
        if (movementSpeed > 0.1f && isGrounded)
        {
            stepTimer += Time.deltaTime;
            if (stepTimer >= stepInterval)
            {
                footstepSoundPool.PlayFootstep(transform.position);
                stepTimer = 0f;
            }
        }
        else
        {
            stepTimer = 0f;
            Debug.Log("Reseting step timer");
        }
    }
}
