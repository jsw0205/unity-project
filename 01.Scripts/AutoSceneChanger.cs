using UnityEngine;
using UnityEngine.SceneManagement;

public class AutoSceneChange : MonoBehaviour
{
    public string targetSceneName = "Stage2";
    public Vector3 targetPositionInNewScene = new Vector3(0, 1, 0); // Stage2에서 플레이어 도착 위치

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // 위치 기억
            GameManager.instance.SetPendingWarp(
                new WarpPointData("testWarp", "Stage2Test", targetSceneName, targetPositionInNewScene)
            );
            if (WarpManager.instance != null && WarpManager.instance.isOpen)
            {
                WarpManager.instance.CloseWarpUI();
            }

            // 씬 전환
            SceneManager.LoadScene(targetSceneName);
        }
    }
}
