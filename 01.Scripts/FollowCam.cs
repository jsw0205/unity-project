using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player; // 따라갈 플레이어의 Transform
    public Vector3 offset = new Vector3(0, 2, -10); // 카메라 위치 오프셋
    public float smoothSpeed = 5f; // 부드러운 이동 속도

    void LateUpdate()
    {
        if (player != null)
        {
            Vector3 targetPosition = player.position + offset;
            transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);
        }
    }
}
