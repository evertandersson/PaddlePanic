using System;
using UnityEngine;
using Unity.Mathematics;


[RequireComponent(typeof(Rigidbody))]
public class SpeedLines : MonoBehaviour
{
    [SerializeField] private Material speedLinesMaterial;
    
    [SerializeField] private float minSpeed;
    [SerializeField] private float maxSpeed;
    
    [SerializeField] private float minOpacity;
    [SerializeField] private float maxOpacity;
    
    private Rigidbody rb;
    
    private static readonly int LineOpacity = Shader.PropertyToID("_Line_Opacity");

    
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        var speed = rb.velocity.xz().magnitude;
        
        if (speed > 0)
        {
            var opacity = math.remap(minSpeed, maxSpeed, minOpacity, maxOpacity, speed);
            opacity = Mathf.Clamp01(opacity);
        
            speedLinesMaterial.SetFloat(LineOpacity, opacity);
        }
    }

    private void OnDestroy()
    {
        speedLinesMaterial.SetFloat(LineOpacity, 0);
    }
}
