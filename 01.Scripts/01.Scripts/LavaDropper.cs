using System.Collections;
using UnityEngine;

public class LavaSpawner : MonoBehaviour
{
    public GameObject lavaPrefab; // 용암 프리팹
    public float spawnInterval = 5f; // 용암 생성 간격
    public Transform spawnPoint; // 용암이 생성될 위치

    void Start()
    {
        StartCoroutine(SpawnLava());
    }

    private IEnumerator SpawnLava()
    {
        while (true)
        {
            GameObject lava = Instantiate(lavaPrefab, spawnPoint.position, Quaternion.identity);
            Destroy(lava, 10f); // 용암이 일정 시간 후 자동 삭제 (안 남도록)
            yield return new WaitForSeconds(spawnInterval);
        }
    }
}
