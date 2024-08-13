using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using Random = System.Random;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;
using Vector4 = UnityEngine.Vector4;

[RequireComponent(typeof(StreamInterpolator))]
public class River : MonoBehaviour
{
    public struct Vertex
    {
        public Vector3 position, normal;
    }
    private struct StreamDirection
    {
        public Vector2 hotspot;
        public Vector2 direction;
        public float intensity;
    }
    struct WaveParameters
    {
        public float amplitude, frequency, speed, angle, offset;
    };

    [SerializeField] private Mesh planeMesh;

    [SerializeField, Range(0, 2f)] private float amplitude = 0.05f;
    [SerializeField, Range(0, 1)] private float frequency = 0.25f;
    [SerializeField, Range(0, 20)] private float speed = 5f;
    [SerializeField, Range(0, 1)] private float amplitudeDropoff = 0.09f;
    [SerializeField, Range(0, 1)] private float frequencyGrowth = 0.15f;
    [SerializeField, Range(0f, 1f)] private float alignment = 0.75f;
    [SerializeField, Range(1, 50)] private int waves = 15;
    [Space]
    [SerializeField] private UnityEngine.Random.State randomState;
    
    
    private Vertex[] vertices;
    private Vector3[] initialVertices;
    private Vector3[] initialNormals;
    private Vertex[] displacement;

    private WaveParameters[] waveParameters;
    private StreamDirection[] streamDirections;

    private float[] vertDistances;


    private ComputeBuffer vertexBuffer;
    private ComputeBuffer displacementBuffer;
    private ComputeBuffer waveParametersBuffer;
    private ComputeBuffer streamDirectionsBuffer;

    private int vertexCount;
    private int kernel = 0;

    private int verticesID = Shader.PropertyToID("Vertices");

    private ComputeShader computeShaderResource = null;
    private ComputeShader computeShader = null;
    
    private MeshFilter meshFilterComp = null;
    private Mesh mesh = null;

    private BoxCollider triggerComp = null;
    private StreamInterpolator streamComp = null;
    private Transform playerTransform;

    private float currentHeightAtPlayer = float.NegativeInfinity;
    private Vector3 closestNormal;

    private void OnDisable()
    {
        vertexBuffer.Release();
        displacementBuffer.Release();
        waveParametersBuffer.Release();
        streamDirectionsBuffer.Release();
    }

    private void Start() {
        Initialize();
    }

    private void Update() {
        if (computeShaderResource) {
            computeShader.SetFloat("Time", Time.fixedTime);
            //computeShader.SetVector("PlayerPosition", new Vector4(playerTransform.position.x, playerTransform.position.z, 0, 0));
            
            computeShader.GetKernelThreadGroupSizes(kernel, out uint x, out uint y, out uint z);
            computeShader.Dispatch(kernel, Mathf.CeilToInt(vertexCount / (float)x), 1, 1);
        }
        displacementBuffer.GetData(displacement);


        Vector3[] positions = new Vector3[vertexCount];
        Vector3[] norms = new Vector3[vertexCount];

        int smallestDistanceIndex = 0;
        float minDistance = float.PositiveInfinity;

        Vector3 pos = transform.position;

        for (int i = 0; i < vertexCount; i++) {
            positions[i] = displacement[i].position;
            norms[i] = displacement[i].normal;

            Vector3 fromTo = initialVertices[i] + pos - playerTransform.position;
            fromTo.y = 0;
            float currentSqrDist = Vector3.SqrMagnitude(fromTo);

            if (currentSqrDist < minDistance)
            {
                minDistance = currentSqrDist;
                smallestDistanceIndex = i;
            }
        }

        currentHeightAtPlayer = initialVertices[smallestDistanceIndex].y + pos.y;
        closestNormal = initialNormals[smallestDistanceIndex];

        mesh.vertices = positions;
        mesh.normals = norms;
    }
    private void Initialize() {
        computeShaderResource = (ComputeShader)Resources.Load("RiverDisplacement");
        computeShader = Instantiate(computeShaderResource);

        UnityEngine.Random.state = randomState;

        meshFilterComp = GetComponent<MeshFilter>();
        streamComp = GetComponent<StreamInterpolator>();
        
        vertexCount = meshFilterComp.mesh.vertices.Length;

        streamComp.GetStreamPoints();
        streamDirections = new StreamDirection[streamComp.directions.Length];
        vertDistances = new float[vertexCount];

        playerTransform = GameObject.FindWithTag("Player").transform;

        for (int i = 0; i < streamComp.directions.Length; i++) {
            streamDirections[i].hotspot = streamComp.directions[i].hotspot;
            streamDirections[i].direction = streamComp.directions[i].heading;
            streamDirections[i].intensity = streamComp.directions[i].intensity;
        }
        mesh = meshFilterComp.sharedMesh;
        initialVertices = mesh.vertices;
        initialNormals = mesh.normals;

        triggerComp = this.AddComponent<BoxCollider>();
        triggerComp.center = mesh.bounds.center;
        triggerComp.size = new Vector3(mesh.bounds.size.x, 1000000f, mesh.bounds.size.z);
        triggerComp.isTrigger = true;
        

        vertices = new Vertex[vertexCount];
        displacement = new Vertex[vertexCount];
        
        vertexBuffer = new ComputeBuffer(vertexCount, 6 * sizeof(float));
        displacementBuffer = new ComputeBuffer(vertexCount, 6 * sizeof(float));
        waveParametersBuffer = new ComputeBuffer(waves, 5 * sizeof(float));
        streamDirectionsBuffer = new ComputeBuffer(streamDirections.Length, 5 * sizeof(float));
        
        

        for (int i = 0; i < vertexCount; i++) {
            vertices[i].position = mesh.vertices[i];
            vertices[i].normal = transform.rotation * mesh.normals[i];
            
            displacement[i].position = Vector3.zero;
            displacement[i].normal = vertices[i].normal;
        }
        
        vertexBuffer.SetData(vertices);
        displacementBuffer.SetData(displacement);
        GenerateWaveParameters();
        waveParametersBuffer.SetData(waveParameters);
        streamDirectionsBuffer.SetData(streamDirections);

        
        if (computeShader) {
            kernel = computeShader.FindKernel("SurfaceDeformation");
            
            computeShader.SetFloat("Amplitude", amplitude);
            computeShader.SetFloat("Frequency", frequency);
            computeShader.SetFloat("Speed", speed);
            computeShader.SetFloat("Alignment", alignment);
            
            computeShader.SetInt("WaveCount", waves);
            computeShader.SetInt("StreamPointCount", streamDirections.Length);
            computeShader.SetInt("Seed", 1);
            
            computeShader.SetVector("Position", transform.position);
            
            computeShader.SetMatrix("Transform", transform.localToWorldMatrix);
            computeShader.SetMatrix("InvTransform", transform.worldToLocalMatrix);
            
            computeShader.SetBuffer(kernel, verticesID, vertexBuffer);
            computeShader.SetBuffer(kernel, Shader.PropertyToID("Displacement"), displacementBuffer);
            computeShader.SetBuffer(kernel, Shader.PropertyToID("WaveParametersBuffer"), waveParametersBuffer);
            computeShader.SetBuffer(kernel, Shader.PropertyToID("StreamDirectionsBuffer"), streamDirectionsBuffer);
        }
    }
    
    void GenerateWaveParameters()
    {
        waveParameters = new WaveParameters[waves];
        
        waveParameters[0] = new WaveParameters
        {
            frequency = this.frequency,
            amplitude = amplitude,
            speed = speed,
            offset = 0
        };
        
        for (int i = 1; i < waves; i++)
        {
            waveParameters[i] = new WaveParameters
            {
                frequency = this.frequency * Mathf.Pow(1 + frequencyGrowth, i),
                amplitude = amplitude * Mathf.Pow(1 - amplitudeDropoff, i),
                speed = speed * Mathf.Pow(0.95f, i),
                angle = UnityEngine.Random.Range(-180f, 180f),
                offset = UnityEngine.Random.Range(0.0f, 2 * Mathf.PI) / frequency
            };
        }
    }
    
    void SampleSine(float amplitude, float frequency, float speed, float angle, float offset, Vector2 uv, out float height, out Vector2 derivative) {
        float rad = Mathf.Deg2Rad * angle;
        float cosAngle = Mathf.Cos(rad);
        float sinAngle = Mathf.Sin(rad);
        
        Vector2 direction = new Vector2(cosAngle, sinAngle);
        
        uv += direction * offset;
        
        float inputD = Mathf.PI * frequency * (uv.x * direction.x + uv.y * direction.y) - Time.fixedTime * speed;
    
        derivative = amplitude * frequency * Mathf.Exp(Mathf.Sin(inputD) - 1) * Mathf.Cos(inputD) * direction;
        uv -= derivative;
        float input = Mathf.PI * frequency * (uv.x * direction.x + uv.y * direction.y) - Time.fixedTime * speed;
        height = amplitude * Mathf.Exp(Mathf.Sin(input) - 1);
    }
    public Vertex SampleWaves(Vector3 at) {
        float height = 0f;
        Vector2 derivative = Vector2.zero;
        
        float newHeight = 0;
        Vector2 newDerivative = Vector2.zero;
    
        float amplitudeSum = waveParameters[0].amplitude;

        Vector2 uv = new Vector2(at.x, at.z);
        float intensity;
        Vector2 streamDirection = streamComp.Sample(uv, out intensity);
        //uv.x = streamDirection.x * uv.x - streamDirection.y * uv.x;
        //uv.y = streamDirection.y * uv.y - streamDirection.x * uv.y;
    
        for(uint i = 0; i < waves; i++) {
            WaveParameters parameters = waveParameters[i];
            amplitudeSum += parameters.amplitude;
            
            SampleSine(parameters.amplitude * intensity, parameters.frequency, parameters.speed, (1 - alignment) * parameters.angle, parameters.offset, uv, out newHeight, out newDerivative);
            
            derivative += newDerivative;
            height += (newHeight - parameters.amplitude / 2);
        }
        
        Vector3 normal = new Vector3(derivative.x, 2, derivative.y).normalized;
        //height -= 1 * (1 - normal.y * normal.y * normal.y);
        Vector3 position = at + normal * height;
    
        Vertex output;
        output.position = position;
        output.normal = normal;
        
        return output;
    }

    public Vector3 SampleStream(Vector3 at, out float intensity) {
        Vector2 stream = streamComp.Sample(new Vector2(at.x, at.z), out intensity);

        return new Vector3(stream.x, 0, stream.y);
    }

    public float GetCurrentHeightAtPlayer() { return currentHeightAtPlayer; }
    public Vector3 GetClosestNormal() { return closestNormal; }

    private void OnDrawGizmos() {
        if (computeShader) {
            //Color color = Color.white;
            ////color.a = 0.25f;
            //float dist = 1f;
            //Vertex vertex = SampleWaves(playerTransform.position);
            //Vertex a = SampleWaves(playerTransform.position - dist * transform.right);
            //Vertex b = SampleWaves(playerTransform.position + dist * transform.right);
            //Vertex c = SampleWaves(playerTransform.position - dist * transform.forward);
            //Vertex d = SampleWaves(playerTransform.position + dist * transform.forward);

            //Gizmos.DrawWireSphere(vertex.position, 0.1f);
            //Gizmos.DrawWireSphere(a.position, 0.1f);
            //Gizmos.DrawWireSphere(b.position, 0.1f);
            //Gizmos.DrawWireSphere(c.position, 0.1f);
            //Gizmos.DrawWireSphere(d.position, 0.1f);
        }

    }
}
