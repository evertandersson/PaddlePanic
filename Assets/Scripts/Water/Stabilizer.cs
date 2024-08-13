using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stabilizer : MonoBehaviour
{
    [SerializeField] float stabilizationStrength = 5;

    private Rigidbody rigidbodyComp = null;
    private Gyroscope gyro = null;

    void Start()
    {
        rigidbodyComp = GetComponent<Rigidbody>();
        gyro = Input.gyro;
        gyro.enabled = true;
    }
    void FixedUpdate()
    {
        if (gyro == null) 
            return;
        float up = transform.up.y;
        float t = up > 0 ? 1 - up : 0;
        t = Mathf.Sqrt(t);

        rigidbodyComp.AddTorque(t * stabilizationStrength * Vector3.Cross(transform.up, Vector3.up).normalized);
        //Transform cameraTransform = Camera.main.transform;
        //float angleAroundY = cameraTransform.rotation.eulerAngles.y;
        //rigidbodyComp.angularVelocity += (t * Time.fixedDeltaTime * stabilizationStrength * gyro.rotationRate);
    }
}
