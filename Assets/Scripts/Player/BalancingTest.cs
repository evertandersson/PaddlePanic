using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalancingTest : MonoBehaviour
{
    [SerializeField, Range(0f, 10f)] float torqueScale = 5;

    PlayerController player = null;
    Rigidbody rb = null;

    float steer = 0;

    void Start()
    {
        player = GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.X))
            steer = 1;
        else if (Input.GetKey(KeyCode.Z))
            steer = -1;
        else
            steer = 0;
    }

    private void FixedUpdate()
    {
        Vector3 torque = -torqueScale * steer * player.GetStreamDirection().normalized;

        Debug.Log(torque);

        rb.AddTorque(torque);
    }
}
