using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floater : MonoBehaviour
{
    public Transform water;
    public Rigidbody rb;
    private float depthBeforeSubmerged = 1f;
    private float displacementAmount = 3f;

    [SerializeField] float displacementMultiplier;

    float height = 0f;

    private void FixedUpdate()
    {
        if (water != null)
        {
            if (transform.localPosition.y < height)
            {
                displacementMultiplier = Mathf.Clamp01((height - transform.position.y) / depthBeforeSubmerged) * displacementAmount;
                rb.AddForce(new Vector3(0, Mathf.Abs(Physics.gravity.y) * displacementMultiplier, 0), ForceMode.Acceleration);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Water"))
        {
            water = other.gameObject.transform;
            height = water.localPosition.y + water.GetComponent<MeshFilter>().mesh.bounds.extents.y;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Water"))
        {
            water = null;
        }
    }
}
