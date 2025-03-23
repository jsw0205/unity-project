using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class WarpManager : MonoBehaviour
{
    public static WarpManager instance;

    public GameObject warpUI;
    public Transform warpListParent;
    public GameObject warpItemPrefab;
    public TMP_Text sceneNameText; // 인스펙터에서 연결

    private List<WarpPointData> allWarpPoints = new List<WarpPointData>();
    private List<string> scenePages = new List<string>();
    private int currentScenePageIndex = 0;
    private string currentSceneFilter = "";

    
    public bool isOpen = false;

    private PlayerControls controls;

    private List<WarpPointData> currentFilteredList = new List<WarpPointData>();


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        controls = new PlayerControls();

        controls.Gameplay.Move.performed += ctx => OnMove(ctx.ReadValue<Vector2>());
        controls.UI.Confirm.performed += ctx => OnConfirm();
        controls.UI.Cancel.performed += ctx => OnCancel();
    }

    private void Start()
    {
        if (GameManager.instance != null)
        {
            foreach (var data in GameManager.instance.GetAllWarpPoints())
            {
                RegisterWarpPoint(data);
            }
        }
    }

    private void OnEnable()
    {
        controls.Gameplay.Enable();
        controls.UI.Enable();
    }

    private void OnDisable()
    {
        if (controls != null)
        {
            controls.Gameplay.Disable();
            controls.UI.Disable();
        }
    }

    void OnMove(Vector2 input)
    {
        if (!isOpen) return;

        // ← →
        if (input.x > 0.5f)
        {
            currentScenePageIndex = (currentScenePageIndex + 1) % scenePages.Count;
            currentSceneFilter = scenePages[currentScenePageIndex];
            sceneNameText.text = currentSceneFilter;
            RefreshWarpList();
            currentIndex = 0;
            UpdateHighlight();
            return;
        }
        else if (input.x < -0.5f)
        {
            currentScenePageIndex = (currentScenePageIndex - 1 + scenePages.Count) % scenePages.Count;
            currentSceneFilter = scenePages[currentScenePageIndex];
            sceneNameText.text = currentSceneFilter;
            RefreshWarpList();
            currentIndex = 0;
            UpdateHighlight();
            return;
        }

        // ↑ ↓
        if (input.y > 0.5f)
        {
            currentIndex = (currentIndex - 1 + warpListParent.childCount) % warpListParent.childCount;
            UpdateHighlight();
        }
        else if (input.y < -0.5f)
        {
            currentIndex = (currentIndex + 1) % warpListParent.childCount;
            UpdateHighlight();
        }
    }

    void OnConfirm()
    {
        if (!isOpen) return;

        var filteredList = allWarpPoints
            .Where(wp => wp.sceneName == currentSceneFilter)
            .OrderBy(wp => wp.warpName)
            .ToList();

        if (filteredList.Count == 0) return;

        WarpPointData data = filteredList[currentIndex];
        if (GameManager.instance.IsWarpUnlocked(data.warpID))
        {
            WarpTo(data);
        }
        else
        {
            Debug.Log("해금되지 않은 워프입니다.");
        }
    }

    void OnCancel()
    {
        if (isOpen)
        {
            CloseWarpUI();
        }
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

        // 씬 이름 페이지 구성 (정렬 대신 현재 씬 우선)
        scenePages = allWarpPoints
            .Select(wp => wp.sceneName)
            .Distinct()
            .ToList();

        string currentSceneName = SceneManager.GetActiveScene().name;

        if (scenePages.Contains(currentSceneName))
        {
            scenePages.Remove(currentSceneName);
            scenePages.Insert(0, currentSceneName);
        }

        currentScenePageIndex = 0;
        currentSceneFilter = scenePages[currentScenePageIndex];
        sceneNameText.text = currentSceneFilter;

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

        currentFilteredList = allWarpPoints
            .Where(wp => wp.sceneName == currentSceneFilter)
            .OrderBy(wp => wp.warpName)
            .ToList();

        for (int i = 0; i < currentFilteredList.Count; i++)
        {
            int index = i;
            var data = currentFilteredList[i];
            GameObject item = Instantiate(warpItemPrefab, warpListParent);
            TMP_Text text = item.GetComponentInChildren<TMP_Text>();
            Button btn = item.GetComponent<Button>();

            bool isUnlocked = GameManager.instance.IsWarpUnlocked(data.warpID);

            text.text = isUnlocked ? data.warpName : "???";

            Image bg = item.GetComponent<Image>();
            if (bg != null)
                bg.color = isUnlocked ? Color.white : new Color(0.5f, 0.5f, 0.5f, 1f);

            if (btn != null)
            {
                if (isUnlocked)
                    btn.onClick.AddListener(() => WarpTo(data));
                else
                    btn.interactable = false;
            }
        }
    }
    private int currentIndex = 0;


    void UpdateHighlight()
    {
        int count = Mathf.Min(currentFilteredList.Count, warpListParent.childCount);

        for (int i = 0; i < count; i++)
        {
            Image bg = warpListParent.GetChild(i).GetComponent<Image>();
            if (bg != null)
            {
                bool isSelected = (i == currentIndex);
                bool isUnlocked = GameManager.instance.IsWarpUnlocked(currentFilteredList[i].warpID);

                bg.color = isSelected && isUnlocked ? Color.yellow
                          : isUnlocked ? Color.white
                          : new Color(0.5f, 0.5f, 0.5f, 1f);
            }
        }
    }



    void WarpTo(WarpPointData data)
    {
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

        foreach (var data in GameManager.instance.GetAllWarpPoints())
        {
            RegisterWarpPoint(data);
        }
    }
}
