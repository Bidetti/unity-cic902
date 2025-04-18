using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;
    public float smoothSpeed = 0.125f;
    public float fixedY = 0f;
    public float fixedZ = -10f;

    void LateUpdate()
    {
        if (player != null)
        {
            Vector3 desiredPosition = new Vector3(player.position.x, fixedY, fixedZ);
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;
        }
    }
}