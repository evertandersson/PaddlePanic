using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ViewChanger : MonoBehaviour
{
    [Serializable]
    public enum TriggerType
    {
        ENTER,
        EXIT,
    }
    
    [SerializeField] private TriggerType triggerType;
    
    [SerializeField] Vector3 newOffset = new Vector3(0, 15f, -19f);
    [SerializeField, Range(-90f, 90f)] float newXRotationOffset = 10f;
    [SerializeField, Range(0f, 5f)] private float transitionDuration = 2f;

    private CameraFollow cameraFollow = null;
    

    void Start() {
        cameraFollow = Camera.main.GetComponent<CameraFollow>();
    }

    private void OnTriggerEnter(Collider other) {

        switch (triggerType) {
            case TriggerType.ENTER:
                if (other.GetComponentInParent<PlayerController>())
                    cameraFollow.SetNewOffset(newOffset, newXRotationOffset, transitionDuration);
                break;
            case TriggerType.EXIT:
                if (other.GetComponentInParent<PlayerController>())
                    cameraFollow.ResetOffset(transitionDuration);
                break;
        }
    }
}
