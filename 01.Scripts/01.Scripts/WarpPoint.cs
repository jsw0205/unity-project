using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class WarpPoint : MonoBehaviour
{
    public string warpID = "default";
    public string warpName = "Unnamed";
    public string sceneName = "";

    private bool playerInRange = false;
    private PlayerControls controls;

    private void Awake()
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            sceneName = SceneManager.GetActiveScene().name;
        }

        controls = new PlayerControls();
        controls.Gameplay.Interact.performed += ctx => TryOpenWarpUI();
    }

    private void OnEnable()
    {
        controls.Gameplay.Enable();
    }

    private void OnDisable()
    {
        controls.Gameplay.Disable();
    }

    private void Start()
    {
        WarpPointData data = new WarpPointData(warpID, warpName, sceneName, transform.position);
        GameManager.instance.RegisterWarpPointData(data);
        if (GameManager.instance.IsWarpUnlocked(warpID))
        {
            // 이미 등록된 워프라도 UI에 뜨도록 매번 등록
            WarpManager.instance.RegisterWarpPoint(data);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerInRange = true;
        WarpPointData data = new WarpPointData(warpID, warpName, sceneName, transform.position);

        if (!GameManager.instance.IsWarpUnlocked(warpID))
        {
            GameManager.instance.UnlockWarp(warpID); //  여기
        }

        // Warp UI 목록 갱신용
        WarpManager.instance.RegisterWarpPoint(data);
        GameManager.instance.SaveCheckpoint(transform.position);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    private void TryOpenWarpUI()
    {
        if (playerInRange && WarpManager.instance != null)
        {
            WarpManager.instance.OpenWarpUI();
        }
    }
}
