using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement Instance { get; private set; }

    public KeyCode forward = KeyCode.W;
    public KeyCode back = KeyCode.S;
    public KeyCode left = KeyCode.A;
    public KeyCode right = KeyCode.D;

    public float moveSpeed;

    Rigidbody rb;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        HandleInput();
    }

    void HandleInput()
    {
        Vector3 vel = Vector3.zero;
        if(Input.GetKey(forward))
        {
            vel += transform.forward;
        }   
        if (Input.GetKey(back))
        {
            vel -= transform.forward;
        }
        if (Input.GetKey(left))
        {
            vel -= transform.right;

        }
        if (Input.GetKey(right))
        {
            vel += transform.right;
        }
		vel = vel.normalized;

        rb.velocity = new Vector3(vel.x * moveSpeed, rb.velocity.y, vel.z * moveSpeed);
    }

    private void OnDisable()
    {
        if (!rb) { return; }
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }
}
