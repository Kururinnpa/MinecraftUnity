using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public bool isInGame = false;
    public bool isCreative;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void StartSurvival()
    {
        isCreative = false;
        StartGame();
    }

    public void StartCreative()
    {
        isCreative = true;
        StartGame();
    }

    private void StartGame()
    {
        isInGame = true;
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadScene("GameScene");
    }

    public void ReturnToTitle()
    {
        GameManager.instance.isInGame = false;
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        Destroy(UIManager.instance);
        Destroy(instance);
        // 如果是在编辑器中运行：
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        // 如果是已构建的应用程序：
        #else
            Application.Quit();
        #endif
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "GameScene") // 确保是加载的目标场景
        {
            UIManager.instance.GetUIInGame(); // 执行UI相关的操作
            SceneManager.sceneLoaded -= OnSceneLoaded; // 移除事件监听，防止重复调用
        }
        else if (scene.name == "MainMenu")
        {
            Button survivalButton = GameObject.Find("Survival").GetComponent<Button>();
            Button creativeButton = GameObject.Find("Creative").GetComponent<Button>();
            Button quitButton = GameObject.Find("Quit").GetComponent<Button>();

            // 确保按钮事件绑定
            if (survivalButton != null)
            {
                survivalButton.onClick.RemoveAllListeners();
                survivalButton.onClick.AddListener(StartSurvival);
            }
            if (creativeButton != null)
            {
                creativeButton.onClick.RemoveAllListeners();
                creativeButton.onClick.AddListener(StartCreative);
            }
            if (quitButton != null)
            {
                quitButton.onClick.RemoveAllListeners();
                quitButton.onClick.AddListener(QuitGame);
            }

            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }
}
