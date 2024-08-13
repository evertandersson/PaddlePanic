using System.Threading.Tasks;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{

    [SerializeField] private float smoothSpeed = 1f;
    [SerializeField] private Vector3 restOffset = Vector3.zero;
    [SerializeField] private float restingXRotationOffset = 10f;

    private Vector3 currentOffset = Vector3.zero;
    private float xRotationOffset = 0f;

    private Transform target;

    //private PlayerController player = null;
    private StaminaScript staminaScript;

    public void Initialize() {
        //player = target.GetComponent<PlayerController>();
        staminaScript = target.GetComponent<StaminaScript>();
        currentOffset = restOffset;
        xRotationOffset = restingXRotationOffset;
    }

    public async void SetNewOffset(Vector3 offset, float xRotationOffset, float duration) {
        var end = Time.fixedTime + duration;
        while(Time.fixedTime < end) {
            float t = (end - Time.fixedTime) / duration;
            float smoothT = 3 * t * t - 2 * t * t * t;
            currentOffset = Vector3.Lerp(offset, currentOffset, smoothT);
            xRotationOffset = Mathf.Lerp(xRotationOffset, this.xRotationOffset, smoothT);
            await Task.Yield();
        }
        currentOffset = offset;
        this.xRotationOffset = xRotationOffset;
    }
    public void ResetOffset(float duration) {
        SetNewOffset(restOffset, restingXRotationOffset, duration);
    }

    private void FixedUpdate() {
        Vector3 streamDirection = staminaScript.GetStreamDirection();
        Quaternion worldRot = Quaternion.LookRotation(streamDirection, Vector3.up);

        Vector3 desiredPosition = target.position + worldRot * currentOffset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, Mathf.Exp(Time.deltaTime * smoothSpeed));
        transform.position = smoothedPosition;

        transform.LookAt(target);
        transform.rotation = Quaternion.AngleAxis(xRotationOffset, -transform.right) * transform.rotation;
    }

    public void AssignTarget(Transform target) {
        this.target = target;
    }
}
