using UnityEngine;

public class WaterSplashCollision : MonoBehaviour
{
    [SerializeField] private AudioEvent SplashEvent;
    [SerializeField] private AudioSource splashSource;

    [SerializeField] private AudioEvent CollisionEvent;
    [SerializeField] private AudioSource CollisionSource;

    private void OnTriggerEnter(Collider other)
    {
        if (splashSource && other.gameObject.CompareTag("Player"))
        {
            SplashEvent.Play(splashSource);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (CollisionSource && other.gameObject.transform.parent.gameObject.CompareTag("Terrain"))
        {
            CollisionEvent.Play(CollisionSource);
        }
    }
}
