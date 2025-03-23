using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private SaveData currentSave;
    private string saveFilePath;

    private Vector3 pendingWarp = Vector3.zero;
    public Vector3 respawnPosition => new Vector3(currentSave.respawnX, currentSave.respawnY, currentSave.respawnZ);

    public bool HasPendingWarp => pendingWarp != Vector3.zero;
    private bool autoSceneLoaded = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            saveFilePath = Application.persistentDataPath + "/saveData.json";
            LoadData();

            if (SceneManager.GetActiveScene().name != currentSave.lastSceneName)
            {
                autoSceneLoaded = true;
                SceneManager.LoadScene(currentSave.lastSceneName);
            }
        }
        else Destroy(gameObject);
    }

    private void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(HandleSceneLoaded());
    }

    private IEnumerator HandleSceneLoaded()
    {
        yield return null;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            player.transform.position = respawnPosition;
        }

        // 현재 씬에 있는 WarpPoint 모두 등록
        WarpPoint[] points = GameObject.FindObjectsOfType<WarpPoint>();
        foreach (var point in points)
        {
            WarpPointData data = new WarpPointData(point.warpID, point.warpName, point.sceneName, point.transform.position);
            RegisterWarpPointData(data); // 전체 목록에만 등록 (해금은 따로)
        }

        WarpManager.instance.OnSceneLoaded();
    }

    private void LoadData()
    {
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            currentSave = JsonUtility.FromJson<SaveData>(json);
        }
        else
        {
            currentSave = new SaveData();
            SaveDataToFile();
        }
    }

    private void SaveDataToFile()
    {
        string json = JsonUtility.ToJson(currentSave, true);
        File.WriteAllText(saveFilePath, json);
    }

    public void RegisterWarpPointData(WarpPointData data)
    {
        if (!currentSave.allWarpPoints.Exists(w => w.warpID == data.warpID))
        {
            currentSave.allWarpPoints.Add(data);
            SaveDataToFile();
        }
    }

    public void UnlockWarp(string id)
    {
        if (!currentSave.unlockedWarpIDs.Contains(id))
        {
            currentSave.unlockedWarpIDs.Add(id);
            SaveDataToFile();
        }
    }

    public bool IsWarpUnlocked(string id)
    {
        return currentSave.unlockedWarpIDs.Contains(id);
    }

    public List<WarpPointData> GetAllWarpPoints()
    {
        return currentSave.allWarpPoints;
    }

    public void SaveCheckpoint(Vector3 pos)
    {
        currentSave.respawnX = pos.x;
        currentSave.respawnY = pos.y;
        currentSave.respawnZ = pos.z;
        currentSave.lastSceneName = SceneManager.GetActiveScene().name;
        SaveDataToFile();
    }

    public void RespawnPlayer(GameObject player)
    {
        player.transform.position = respawnPosition;
    }

    public void SetPendingWarp(WarpPointData data)
    {
        pendingWarp = data.position;
    }

    public Vector3 ConsumePendingWarp()
    {
        Vector3 result = pendingWarp;
        pendingWarp = Vector3.zero;
        return result;
    }
}
