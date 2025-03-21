using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20f;
    public float destroyTime = 3f;
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (rb == null)
        {
            Debug.LogError("Bullet에 Rigidbody가 없습니다! Rigidbody를 추가하세요.");
        }

        // 총알을 앞으로 발사
        rb.velocity = transform.forward * speed;

        // 일정 시간이 지나면 자동 삭제
        Destroy(gameObject, destroyTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("BreakableBlock")) // collision.gameObject로 접근
        {
            Destroy(collision.gameObject); // 블록 삭제
            Destroy(gameObject); // 총알 삭제
        }
    }

}
