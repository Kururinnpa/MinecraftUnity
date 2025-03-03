using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadManager : MonoBehaviour
{
    public static LoadManager instance;

    public GameObject loadScreen;
    public Slider slider;
    //public TextMeshProUGUI progressText;

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
        GameManager.instance.SetIsCreative(false);
        StartGame();
    }

    public void StartCreative()
    {
        GameManager.instance.SetIsCreative(true);
        StartGame();
    }

    private void StartGame()
    {
        GameManager.instance.SetIsInGame(true);
        SceneManager.sceneLoaded += OnSceneLoaded;
        StartCoroutine(LoadAsynchronously());
    }

    public void SetProgress(float progress)
    {
        slider.value = progress;
    }

    IEnumerator LoadAsynchronously()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync("GameScene");
        while (!operation.isDone)
        {
            //float progress = Mathf.Clamp01(operation.progress / 0.9f);
            //slider.value = progress;
            //progressText.text = progress * 100f + "%";
            yield return null;
        }
    }

    public void ReturnToTitle()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadScene("MainMenu");
        GameManager.instance.SetIsInGame(false);
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

            GameObject canvas = GameObject.Find("Canvas");
            loadScreen = canvas.GetComponentInChildren<Transform>(true).Find("LoadScreen")?.gameObject;
            //slider = GameObject.Find("Progress").GetComponent<Slider>();
            slider = canvas.GetComponentInChildren<Slider>(true);
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
