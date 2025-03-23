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
            Debug.LogError("Bullet�� Rigidbody�� �����ϴ�! Rigidbody�� �߰��ϼ���.");
        }

        // �Ѿ��� ������ �߻�
        rb.velocity = transform.forward * speed;

        // ���� �ð��� ������ �ڵ� ����
        Destroy(gameObject, destroyTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("BreakableBlock")) // collision.gameObject�� ����
        {
            Destroy(collision.gameObject); // ��� ����
            Destroy(gameObject); // �Ѿ� ����
        }
    }

}
