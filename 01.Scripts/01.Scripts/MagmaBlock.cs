using System.Collections;
using UnityEngine;

public class MagmaBlock : MonoBehaviour
{
    public float damageInterval = 0.5f; // 데미지를 받는 간격
    private bool isPlayerOn = false; // 플레이어가 위에 있는지 확인

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isPlayerOn = true;
            StartCoroutine(DamagePlayer());
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isPlayerOn = false;
        }
    }

    private IEnumerator DamagePlayer()
    {
        while (isPlayerOn)
        {
            Debug.Log("마그마 블록 위에 서서 데미지!");
            yield return new WaitForSeconds(damageInterval);
        }
    }
}
