using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GyroController : MonoBehaviour
{
    [SerializeField] float sensitivity = 1.0f;
    Gyroscope gyro = null;

    Vector3 gyroVelocity;

    Quaternion gyroRotation;
    Quaternion calibratedAttitude = Quaternion.identity;

    Rigidbody rb = null;

    private void Start()
    {
        gyro = Input.gyro;
        gyro.enabled = true;
        rb = GetComponent<Rigidbody>();
        Calibrate();
    }

    void Calibrate()
    {
        calibratedAttitude = gyro.attitude;
        if (calibratedAttitude == new Quaternion(0, 0, 0, 0)) calibratedAttitude = Quaternion.identity;
    }

    private void FixedUpdate()
    {
        gyroVelocity = gyro.rotationRate;

        rb.angularVelocity += sensitivity * gyroVelocity * Time.fixedDeltaTime;

        rb.angularVelocity *= 0.99f;
    }
}
