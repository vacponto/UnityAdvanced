using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehavior : MonoBehaviour
{
    [Header("Ground Check")]
    public Transform groundCheckPoint;
    public float groundCheckDistance = 0.2f;
    public LayerMask groundMask;

    [Header("Movement Settings")]
    public float speed = 5f;
    public float stepInterval = 0.5f;


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
        isGrounded = Physics.Raycast(groundCheckPoint.position, Vector3.down, groundCheckDistance, groundMask);
    }

    private void HandleFootsteps()
    {
        bool hasInput = Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0;
        float movementSpeed = (transform.position - lastPosition).magnitude / Time.deltaTime;
        lastPosition = transform.position;

        if (hasInput && isGrounded)
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

    void OnDrawGizmos()
    {
        if (groundCheckPoint != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawLine(groundCheckPoint.position, groundCheckPoint.position + Vector3.down * groundCheckDistance);
        }
    }
}
