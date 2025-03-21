using UnityEngine;
using UnityEngine.SceneManagement;

public class WarpPoint : MonoBehaviour
{
    public string warpID = "default";
    public string warpName = "Unnamed";
    public string sceneName = "";

    private bool playerInRange = false;

    private void Awake()
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            sceneName = SceneManager.GetActiveScene().name;
        }
    }

    private void Start()
    {
        if (GameManager.instance.IsWarpUnlocked(warpID))
        {
            WarpManager.instance.RegisterWarpPoint(new WarpPointData(warpID, warpName, sceneName, transform.position));
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;

            if (!GameManager.instance.IsWarpUnlocked(warpID))
            {
                GameManager.instance.UnlockWarp(warpID);
                WarpManager.instance.RegisterWarpPoint(new WarpPointData(warpID, warpName, sceneName, transform.position));
            }

            // ★ 무조건 세이브
            GameManager.instance.SaveCheckpoint(transform.position);
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) playerInRange = false;
    }

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.R))
        {
            if (WarpManager.instance != null && WarpManager.instance.gameObject != null)
            {
                WarpManager.instance.OpenWarpUI();
            }
        }
    }

}
