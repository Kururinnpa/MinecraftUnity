using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public GameObject hotbar;
    public GameObject inventory;

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

    // Update is called once per frame
    public void UpdateUIState()
    {
        if(inventory.activeSelf)
        {
            hotbar.SetActive(false);
        }
        else
        {
            hotbar.SetActive(true);
        }
    }
}
