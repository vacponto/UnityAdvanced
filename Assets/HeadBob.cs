using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadBob : MonoBehaviour
{
    public float bobSpeed = 10f;
    public float bobAmount = 0.05f;
    public Transform player;

    private float timer = 0f;
    private Vector3 initialPos;
    private Rigidbody playerRb;

    void Start()
    {
        initialPos = transform.localPosition;
        playerRb = player.GetComponent<Rigidbody>();
    }

    void Update()
    {
        Vector3 velocity = new Vector3(playerRb.velocity.x, 0, playerRb.velocity.z);

        if (velocity.magnitude > 0.1f && IsGrounded())
        {
            timer += Time.deltaTime * bobSpeed;
            float bobOffset = Mathf.Sin(timer) * bobAmount;
            transform.localPosition = initialPos + new Vector3(0, bobOffset, 0);
        }
        else
        {
            timer = 0f;
            transform.localPosition = Vector3.Lerp(transform.localPosition, initialPos, Time.deltaTime * bobSpeed);
        }
    }

    bool IsGrounded()
    {
        return Physics.Raycast(player.position, Vector3.down, 1.1f);
    }
}
