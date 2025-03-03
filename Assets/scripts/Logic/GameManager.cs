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

    public void SetIsCreative(bool isCreative)
    {
        this.isCreative = isCreative;
    }

    public void SetIsInGame(bool isInGame)
    {
        this.isInGame = isInGame;
    }
}
