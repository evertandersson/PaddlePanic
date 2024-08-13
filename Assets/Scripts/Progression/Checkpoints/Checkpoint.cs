using System;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private int scoreAwarded = 20;
    [SerializeField] private float threshold = 0.2f;

    [SerializeField] private float moveFrequency;
    [SerializeField] private float moveAmplitude;

    [SerializeField] private bool disableQuads;

    [SerializeField] private AudioSource CheckpointSource;
    
    
    private void Awake()
    {
        DisableQuads();
    }

    private void FixedUpdate()
    {
        Vector3 offset = transform.right * (Mathf.Cos(Time.time * 2 * Mathf.PI * moveFrequency) * moveAmplitude);
        transform.position += offset * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        var xf = transform;
        var xfOther = other.transform;
        
        var forward = new Vector2(xf.forward.x, xf.forward.z);
        var direction = new Vector2(xf.position.x, xf.position.z) - new Vector2(xfOther.position.x, xfOther.position.z);
        
        if (other.CompareTag("Player"))
        {
            EventManager.Raise(EventKey.SCORE_GIVE, scoreAwarded);
            if (CheckpointSource && !CheckpointSource.isPlaying)
            {
                CheckpointSource.Play();
                Debug.Log("Played sound");
            }
            //Debug.Log($"Hit Checkpoint. Dot: {dot}");
        }
        
        Destroy(gameObject);
    }

    private void DisableQuads()
    {
        if (disableQuads)
        {
            var children = GetComponentsInChildren<Transform>();
            for (int i = 1; i < children.Length; i++)
            {
                children[i].gameObject.SetActive(false);
            }
        }
    }
}
