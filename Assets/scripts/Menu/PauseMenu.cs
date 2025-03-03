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

        // 卸载当前场景
        AsyncOperation unloadOperation = SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());

        // 等待当前场景卸载完成
        yield return unloadOperation;

        // 异步加载新场景
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync("MainMenu", LoadSceneMode.Single);

        // 等待新场景加载完成
        yield return loadOperation;
    }
}
