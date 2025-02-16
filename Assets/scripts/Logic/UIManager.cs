using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    private GameObject hotbar;
    private GameObject inventory;
    private GameObject menu;

    // Start is called before the first frame update
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

    private void Update()
    {
        if(GameManager.instance.isInGame && menu != null)
        {
            menu.GetComponent<PauseMenu>().ChangeState();
        }
    }

    public void GetUIInGame()
    {
        hotbar = GameObject.Find("Hotbar");
        inventory = GameObject.Find("Inventory");
        menu = GameObject.Find("Menu");
        menu.SetActive(false);
    }

    public void UpdateUIState()
    {
        if (GameManager.instance.isInGame)
        {
            if (inventory.activeSelf)
            {
                hotbar.SetActive(false);
            }
            else
            {
                hotbar.SetActive(true);
            }
        }
    }

    public GameObject GetInventory()
    {
        return GameManager.instance.isInGame ? inventory : null;
    }

    public void EnableCursor()
    {
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void DisableCursor()
    {
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
