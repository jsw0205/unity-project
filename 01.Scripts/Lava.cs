using UnityEngine;
using System.Collections;

public class Lava : MonoBehaviour
{
    private void Start()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Lava에 Rigidbody가 없습니다! Rigidbody를 추가하세요.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("용암에 닿음!");
            Destroy(gameObject); // 플레이어에 닿아도 삭제
        }
        else if (other.CompareTag("Ground"))
        {
            Destroy(gameObject); // 바닥에 닿으면 삭제
        }
    }
}
