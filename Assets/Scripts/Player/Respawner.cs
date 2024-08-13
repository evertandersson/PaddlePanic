using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Respawner : MonoBehaviour
{
    [SerializeField, Range(0f, 5f)] private float timeBeforeRespawn = 2f;
    [SerializeField, Range(0f, 5f)] private float respawnTime = 2f;

    private float counter;

    private Vector3 safePosition;
    private Quaternion savedRotation;

    private Rigidbody rigidbodyComp = null;
    private Collider colliderComp = null;

    private bool isRespawning = false;

    private void Start() {
        rigidbodyComp = GetComponent<Rigidbody>();
        colliderComp = transform.GetComponentInChildren<Collider>();
    }

    private void OnEnable() {
        EventManager.AddListener<int>(EventKey.SCORE_GIVE, SaveSafeSpot);
        safePosition = transform.position;
        savedRotation = transform.rotation;
    }
    private void OnDisable() {
        EventManager.RemoveListener<int>(EventKey.SCORE_GIVE, SaveSafeSpot);
    }
    private void SaveSafeSpot(int empty) {
        safePosition = transform.position;
        savedRotation = transform.rotation;
    }

    private void FixedUpdate() {
        if (isRespawning) 
            return;
        if(transform.up.y < 0) {
            counter += Time.fixedDeltaTime;
        } else {
            counter = 0;
        }
        if(counter > timeBeforeRespawn) {
            Respawn();
        }
    }

    public async void Respawn() {
        if (isRespawning) 
            return;
        isRespawning = true;

        colliderComp.enabled = false;

        Vector3 initialPosition = transform.position;
        Quaternion initialRotation = transform.rotation;

        var end = Time.fixedTime + respawnTime;

        Vector3 targetPosition = safePosition + Vector3.up * 2;

        while (Time.fixedTime < end) {
            float t = (end - Time.fixedTime) / respawnTime;
            float smoothT = 3 * t * t - 2 * t * t * t;

            transform.position = Vector3.Lerp(targetPosition, initialPosition, smoothT);
            transform.rotation = Quaternion.Lerp(savedRotation, initialRotation, smoothT);

            await Task.Yield();
        }
        transform.position = targetPosition;
        transform.rotation = savedRotation;

        rigidbodyComp.velocity = Vector3.zero;
        rigidbodyComp.angularVelocity = Vector3.zero;

        colliderComp.enabled = true;

        isRespawning = false;
    }
}
