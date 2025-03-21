using System.Collections;
using UnityEngine;

public class FirePillarSpawner : MonoBehaviour
{
    public GameObject firePillarPrefab; // 불기둥 프리팹
    public float spawnInterval = 5f; // 불기둥 생성 간격
    public Transform spawnPoint; // 불기둥이 생성될 위치

    void Start()
    {
        StartCoroutine(SpawnFirePillar());
    }

    private IEnumerator SpawnFirePillar()
    {
        while (true)
        {
            GameObject firePillar = Instantiate(firePillarPrefab, spawnPoint.position, Quaternion.identity);
            firePillar.GetComponent<FirePillar>().Activate();

            yield return new WaitForSeconds(spawnInterval);
        }
    }
}
