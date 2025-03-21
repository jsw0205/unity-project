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
        yield return null; // 한 프레임 대기
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            player.transform.position = new Vector3(
                currentSave.respawnX,
                currentSave.respawnY,
                currentSave.respawnZ
            );
        }
        else
        {
            Debug.LogError("Player object not found after scene load!");
        }

        // WarpManager 실행
        WarpManager.instance.OnSceneLoaded();
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
        player.transform.position = new Vector3(
            currentSave.respawnX,
            currentSave.respawnY,
            currentSave.respawnZ
        );
    }

    public void UnlockWarp(string id)
    {
        if (!currentSave.unlockedWarpIDs.Contains(id))
            currentSave.unlockedWarpIDs.Add(id);
        SaveDataToFile();
    }

    public bool IsWarpUnlocked(string id) => currentSave.unlockedWarpIDs.Contains(id);

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
        }
    }

    private void SaveDataToFile()
    {
        string json = JsonUtility.ToJson(currentSave, true);
        File.WriteAllText(saveFilePath, json);
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
