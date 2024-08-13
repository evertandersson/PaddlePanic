using System;
using System.Diagnostics;
using Unity.Mathematics;
using UnityEngine;

public class StreamInterpolator : MonoBehaviour
{
    [Serializable]
    public class StreamDirection
    {
        public Vector2 hotspot;
        public Vector2 heading;
        [Range(0f, 5f)] public float intensity = 1;
    }

    public StreamDirection[] directions;

    [SerializeField] private Vector2 samplePoint;

    public Vector2 Sample(Vector2 at, out float intensity) // Takes 0.6 us with 5 stream direction samples; takes 3 us with 25 stream samples; 
    {
        Vector2 sum = Vector2.zero;
        intensity = 0;
        float q = 0f;

        for(int i = 0; i < directions.Length; i++)
        {
            Vector2 fromTo = directions[i].hotspot - at;
            float sqrMag = fromTo.sqrMagnitude;
            q += 1 / sqrMag;
        }

        q = 1 / q;

        foreach (var direction in directions)
        {
            Vector2 fromTo = direction.hotspot - at;
            float sqrMag = fromTo.sqrMagnitude;
            float invSqrMag = 1 / sqrMag;

            sum += direction.heading * invSqrMag;
            intensity += q * invSqrMag * direction.intensity;
        }
        

        return sum;
    }
    public void GetStreamPoints()
    {
        directions = new StreamDirection[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            StreamDirection dir = new StreamDirection();
            Transform child = transform.GetChild(i);
            dir.hotspot = child.position.xz();
            dir.heading = child.forward.xz().normalized;
            dir.intensity = child.localScale.z;

            directions[i] = dir;
        }
    }
    public void SetStreamPoints(StreamDirection[] directions)
    {
        this.directions = directions;
    }

    public static Vector2 AngleToDirection(float angle)
    {
        float rad = angle * Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
    }

    private void OnDrawGizmosSelected()
    {
        if (directions.Length == 0)
            return;
        Gizmos.color = Color.red;
        foreach (var direction in directions)
        {
            Gizmos.DrawRay(direction.hotspot.x0z(), direction.heading.x0z());
        }
        Gizmos.color = Color.green;
        float intensity;
        Vector2 dir = Sample(samplePoint, out intensity);
        Gizmos.DrawRay(samplePoint.x0z(), intensity * new Vector3(dir.x, 0, dir.y));
    }

}
