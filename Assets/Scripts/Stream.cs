using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stream : MonoBehaviour
{
    private Rigidbody rb;
    float moveSpeed = 4f;


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        Vector3 moveDirection = (Vector3.forward + transform.forward * 2f) / 2f;
        rb.AddForce(moveDirection * moveSpeed);
    }
}
