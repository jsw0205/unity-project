using UnityEngine;

public class SlopePhysics : MonoBehaviour
{
    public float slideSpeed = 2.0f; // 미끄러지는 속도 조절 가능
    public float slopeAngleThreshold = 10f; // 미끄러짐 시작 최소 각도

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // **회전 방지**
    }

    void OnCollisionStay(Collision collision)
    {
        Vector3 normal = collision.contacts[0].normal;
        float slopeAngle = Vector3.Angle(normal, Vector3.up);

        if (slopeAngle > slopeAngleThreshold)
        {
            Vector3 slideDirection = new Vector3(normal.x, -normal.y, 0).normalized;
            rb.velocity = new Vector3(slideDirection.x * slideSpeed, rb.velocity.y, 0);
            // **Y축 속도 유지 → 중력이 정상 작동**
        }
    }
}
