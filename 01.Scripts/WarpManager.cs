using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class WarpManager : MonoBehaviour
{
    public static WarpManager instance;

    public GameObject warpUI;
    public Transform warpListParent;
    public GameObject warpItemPrefab;

    private List<WarpPointData> allWarpPoints = new List<WarpPointData>();
    private int currentIndex = 0;
    public bool isOpen = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    private void Update()
    {
        if (!isOpen) return;

        if (Input.GetKeyDown(KeyCode.Escape)) CloseWarpUI();
        else if (Input.GetKeyDown(KeyCode.UpArrow)) { currentIndex = (currentIndex - 1 + allWarpPoints.Count) % allWarpPoints.Count; UpdateHighlight(); }
        else if (Input.GetKeyDown(KeyCode.DownArrow)) { currentIndex = (currentIndex + 1) % allWarpPoints.Count; UpdateHighlight(); }
        else if (Input.GetKeyDown(KeyCode.Return)) WarpTo(currentIndex);
    }

    public void RegisterWarpPoint(WarpPointData data)
    {
        if (!allWarpPoints.Exists(p => p.warpID == data.warpID))
            allWarpPoints.Add(data);
    }

    public void OpenWarpUI()
    {
        warpUI.SetActive(true);
        Time.timeScale = 0f;
        isOpen = true;

        RefreshWarpList();
        currentIndex = 0;
        UpdateHighlight();
    }

    public void CloseWarpUI()
    {
        warpUI.SetActive(false);
        Time.timeScale = 1f;
        isOpen = false;
    }

    void RefreshWarpList()
    {
        foreach (Transform child in warpListParent)
            Destroy(child.gameObject);

        for (int i = 0; i < allWarpPoints.Count; i++)
        {
            int index = i;
            GameObject item = Instantiate(warpItemPrefab, warpListParent);
            item.GetComponentInChildren<TMP_Text>().text = allWarpPoints[i].warpName;

            Button btn = item.GetComponent<Button>();
            if (btn != null)
                btn.onClick.AddListener(() => WarpTo(index));
        }
    }

    void UpdateHighlight()
    {
        for (int i = 0; i < warpListParent.childCount; i++)
        {
            Image bg = warpListParent.GetChild(i).GetComponent<Image>();
            if (bg != null)
                bg.color = (i == currentIndex) ? Color.yellow : Color.white;
        }
    }

    void WarpTo(int index)
    {
        WarpPointData data = allWarpPoints[index];
        Time.timeScale = 1f;

        if (SceneManager.GetActiveScene().name == data.sceneName)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            player.transform.position = data.position;
        }
        else
        {
            GameManager.instance.SetPendingWarp(data);
            SceneManager.LoadScene(data.sceneName);
        }

        CloseWarpUI();
    }

    public void OnSceneLoaded()
    {
        if (GameManager.instance.HasPendingWarp)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            player.transform.position = GameManager.instance.ConsumePendingWarp();
        }

        // 현재 씬에 존재하는 모든 WarpPoint들을 다시 등록
        WarpPoint[] points = GameObject.FindObjectsOfType<WarpPoint>();
        foreach (var point in points)
        {
            if (GameManager.instance.IsWarpUnlocked(point.warpID))
            {
                RegisterWarpPoint(new WarpPointData(
                    point.warpID,
                    point.warpName,
                    point.sceneName,
                    point.transform.position
                ));
            }
        }
    }

}
