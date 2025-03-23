using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public Transform pointA; // 이동할 시작 지점
    public Transform pointB; // 이동할 끝 지점
    public float speed = 3f; // 이동 속도

    private Vector3 targetPosition;

    private void Start()
    {
        targetPosition = pointB.position; // 처음에는 pointB로 이동
    }

    private void Update()
    {
        // 플랫폼을 목표 위치로 이동
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        // 목표 위치에 도달하면 반대 방향으로 이동
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            targetPosition = (targetPosition == pointA.position) ? pointB.position : pointA.position;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(transform); // 플레이어를 플랫폼의 자식으로 설정
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(null); // 플레이어가 떠나면 부모 설정 해제
        }
    }
}
