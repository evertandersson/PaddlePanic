using UnityEngine;

public class BasicMovement : MonoBehaviour
{
    [SerializeField] private float speed;

    private Vector3 GetDirection()
    {
        Vector3 direction = new Vector3();
        
        if (Input.GetKey(KeyCode.W))
        {
            direction.z += 1;
        }
        if (Input.GetKey(KeyCode.S))
        {
            direction.z += -1;
        }
        if (Input.GetKey(KeyCode.D))
        {
            direction.x += 1;
        }
        if (Input.GetKey(KeyCode.A))
        {
            direction.x += -1;
        }

        return direction.normalized;
    }

    private void FixedUpdate()
    {
        Vector3 direction = GetDirection();
        transform.position += direction * (speed * Time.deltaTime);
    }
}