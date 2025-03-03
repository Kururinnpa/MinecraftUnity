using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    // Update is called once per frame
    public void ChangeState()
    {
        if(!UIManager.instance.GetInventory().activeSelf && !gameObject.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            gameObject.SetActive(true);
            UIManager.instance.EnableCursor();
        }
        else if (gameObject.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            gameObject.SetActive(false);
            UIManager.instance.DisableCursor();
        }
    }

    public void ReturnToGame()
    {
        gameObject.SetActive(false);
        UIManager.instance.DisableCursor();
    }

    public void ReturnToTitle()
    {
        Time.timeScale = 1;
        LoadManager.instance.ReturnToTitle();
    }

    private IEnumerator LoadNewScene()
    {
        GameManager.instance.isInGame = false;

        // ж�ص�ǰ����
        AsyncOperation unloadOperation = SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());

        // �ȴ���ǰ����ж�����
        yield return unloadOperation;

        // �첽�����³���
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync("MainMenu", LoadSceneMode.Single);

        // �ȴ��³����������
        yield return loadOperation;
    }
}
