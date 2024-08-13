using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.UIElements;

public class Buoyancy : MonoBehaviour
{
    [SerializeField] private Transform boatObj = null;
    [SerializeField, Range(0f, 5f)] private float buoyancyMultiplier = 1f;
    [SerializeField, Range(0f, 2f)] private float forceFalloffFromVelocity = 0.5f;
    [SerializeField, Range(0f, 1f)] private float linearDrag = 0.15f;
    [SerializeField, Range(0f, 1f)] private float angularDrag = 0.1f;
    [SerializeField] private float streamPointInstensity = 5f;

    [Space]
    [SerializeField] Vector3 buoyancyOffset = Vector3.zero;
    [SerializeField, Range(0.5f, 10f)] private float samplePointDensity = 2f;

    [Space, Tooltip("Decides how the hydrodynamics should treat it. A smaller value means the orientation of the raft matters more. In other words, a ring shaped boat would have a value of 1 and a canoe a smaller value (like 0.5 or something)")]
    [SerializeField, Range(0f, 1f)] private float circularity = 1;

    [Space]
    [SerializeField] float emitMinParticlesAtForce = 3;
    [SerializeField] int minParticles = 10;
    [SerializeField] float emitMaxParticlesAtForce = 6;
    [SerializeField] int maxParticles = 100;


    Vector3[] centersL = null;
    Vector3[] centersW = null;

    private River activeRiver = null;
    private Collider activeRiverCol = null;
    private Rigidbody rigidbodyComp = null;
    private ParticleSystem particleComp = null;
    private Mesh colliderMesh = null;

    private Vector3 boundsMin;
    private Vector3 boundsMax;

    private float volumePP, totalVolume;

    private bool isSubmerged = false;

    private Vector3 streamDirection;

    private float particleTimer = 0;

    private void Start()
    {
        colliderMesh = boatObj.GetComponent<MeshCollider>().sharedMesh;

        centersL = Partition();
        volumePP = 1 / (samplePointDensity * samplePointDensity * samplePointDensity);
        totalVolume = centersL.Length * volumePP;

        centersW = new Vector3[centersL.Length];

        rigidbodyComp = GetComponent<Rigidbody>();

        particleComp = transform.GetComponentInChildren<ParticleSystem>();
        particleComp.Stop();
    }

    private void FixedUpdate()
    {
        Collider[] cols = Physics.OverlapSphere(transform.position, 2f, LayerMask.GetMask("Water"), QueryTriggerInteraction.Collide);
        if (cols.Length > 0 && (activeRiver == null || activeRiverCol != null && cols[0] != activeRiverCol))
        {
            River river = cols[0].transform.GetComponent<River>();
            activeRiverCol = cols[0];
            activeRiver = river;
        }
        else if(activeRiver == null || cols.Length == 0) 
            return;


        float intensity;
        streamDirection = activeRiver.SampleStream(transform.position, out intensity);
        streamDirection = streamDirection.normalized;
        //streamDirection *= intensity;

        //rigidbodyComp.velocity = new Vector3(streamDirection.x * streamVelocity, 0, streamDirection.z * streamVelocity);
        //isSubmerged = true;

        isSubmerged = false;

        for (int i = 0; i < centersL.Length; i++)
        {
            centersW[i] = transform.rotation * centersL[i] + transform.position;
            if (rigidbodyComp)
            {
                bool result = EvaluateForce(centersW[i]);
                if(result) isSubmerged = true;
            }
        }

        if (isSubmerged)
        {
            Vector3 stream = intensity * streamDirection * streamPointInstensity;
            Vector3 hydrodynamics = Vector3.ProjectOnPlane((1f - circularity * circularity) * Vector3.Dot(stream, rigidbodyComp.velocity.normalized) * transform.forward, stream);

            rigidbodyComp.AddForce(stream + hydrodynamics, ForceMode.Acceleration);

            rigidbodyComp.AddForce(-(1 - linearDrag) * rigidbodyComp.velocity, ForceMode.Acceleration);
            //rigidbodyComp.velocity *= 1 - linearDrag;
            rigidbodyComp.angularVelocity *= 1 - angularDrag;

            particleTimer += Time.fixedDeltaTime;

            float accForce = (rigidbodyComp.GetAccumulatedForce() / rigidbodyComp.mass + Physics.gravity).magnitude;

            if (accForce > emitMinParticlesAtForce && particleTimer > 0.4f && rigidbodyComp.velocity.y < 0)
            {
                particleTimer = 0;

                float t = (accForce - emitMinParticlesAtForce) / (emitMaxParticlesAtForce - emitMinParticlesAtForce);

                int count = (int)Mathf.Lerp(minParticles, maxParticles, t * t);
                //particleComp.emission.rate
                particleComp.Emit(count);
            }
        }
    }

    float EvaluateHeight(Vector3 at)
    {
        return activeRiver.GetCurrentHeightAtPlayer();
    }

    bool EvaluateForce(Vector3 at)
    {
        if (activeRiver)
        {
            River.Vertex evaluatedVert = activeRiver.SampleWaves(new Vector3(at.x, 0, at.z));
            Vector3 closestNormal = activeRiver.GetClosestNormal();


            Vector3 offset = Vector3.ProjectOnPlane(at - transform.position, closestNormal);
            offset.y -= (1 - Vector3.Dot(closestNormal, transform.up)) * boundsMax.x;
            evaluatedVert.position.y += EvaluateHeight(at) + offset.y;

            if (at.y < evaluatedVert.position.y)
            {
                float kx = forceFalloffFromVelocity * rigidbodyComp.velocity.y;
                float velocityModifier = Mathf.Exp(-(kx + Mathf.Abs(kx)));
                Vector3 force = buoyancyMultiplier * velocityModifier * volumePP * -Physics.gravity;

                rigidbodyComp.AddForceAtPosition(force, at, ForceMode.Force);

                return true;
            }
        }
        return false;
    }

    public Vector3 SampleStream(Vector3 at)
    {
        float intensity = 0;
        if (activeRiver)
        {
            Vector3 dir = activeRiver.SampleStream(at, out intensity);
            return dir * intensity;
        }
        return Vector3.zero;
    }

    Vector3[] Partition()
    {
        List<Vector3> result = new List<Vector3>();

        if (colliderMesh == null) 
            return result.ToArray();

        float invDensity = 1 / samplePointDensity;

        Vector3 boundsTemp1 = boatObj.localToWorldMatrix * colliderMesh.bounds.min;
        Vector3 boundsTemp2 = boatObj.localToWorldMatrix * colliderMesh.bounds.max;
        boundsMin = Vector3.Min(boundsTemp1, boundsTemp2);
        boundsMax = Vector3.Max(boundsTemp1, boundsTemp2);
        float max = Mathf.Max(Mathf.Abs(boundsMin.x), Mathf.Abs(boundsMin.z), Mathf.Abs(boundsMax.x), Mathf.Abs(boundsMax.z));
        boundsMin = new Vector3(-max, boundsMin.y, -max);
        boundsMax = new Vector3(max, boundsMax.y, max);

        for (float cursorX = boundsMin.x - 0.5f * invDensity; cursorX <= boundsMax.x + 0.5 * invDensity; cursorX += invDensity) {
            for (float cursorY = boundsMin.y - 0.5f * invDensity; cursorY <= boundsMax.y + 0.5 * invDensity; cursorY += invDensity) {
                for (float cursorZ = boundsMin.z - 0.5f * invDensity; cursorZ <= boundsMax.z + 0.5 * invDensity; cursorZ += invDensity) {
                    Vector3 pointL = new Vector3(cursorX, cursorY, cursorZ);
                    Vector3 pointW = Vector3.Scale(transform.rotation * pointL + transform.position, transform.lossyScale);


                    if (Physics.OverlapSphereNonAlloc(pointW, 0.25f * invDensity, new Collider[1], LayerMask.GetMask("Player")) > 0)
                        result.Add(pointL + buoyancyOffset);
                }
            }
        }

        return result.ToArray();
    }

    public bool GetIsSubmerged()
    {
        return isSubmerged;
    }

    private void OnDrawGizmos()
    {
        if (centersL == null) 
            return;

        float invDensity = 1 / samplePointDensity;

        Gizmos.color = new Color(1f, 0.92f, 0.016f, 0.5f);
        foreach(var center in centersW)
        {
            Gizmos.DrawCube(center, Vector3.one * invDensity);
        }
    }
}
