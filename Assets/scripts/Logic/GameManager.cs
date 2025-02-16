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
        // ������ڱ༭�������У�
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        // ������ѹ�����Ӧ�ó���
        #else
            Application.Quit();
        #endif
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "GameScene") // ȷ���Ǽ��ص�Ŀ�곡��
        {
            UIManager.instance.GetUIInGame(); // ִ��UI��صĲ���
            SceneManager.sceneLoaded -= OnSceneLoaded; // �Ƴ��¼���������ֹ�ظ�����
        }
        else if (scene.name == "MainMenu")
        {
            Button survivalButton = GameObject.Find("Survival").GetComponent<Button>();
            Button creativeButton = GameObject.Find("Creative").GetComponent<Button>();
            Button quitButton = GameObject.Find("Quit").GetComponent<Button>();

            // ȷ����ť�¼���
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
