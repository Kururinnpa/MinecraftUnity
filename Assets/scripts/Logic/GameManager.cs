using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] private World WorldPrefab;
    private World WorldInstance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        StartGame();
    }

    public void StartGame()
    {
        WorldInstance = Instantiate(WorldPrefab);
        WorldInstance.gameObject.name = "World";
        WorldInstance.SetSeed();
        WorldInstance.GenerateWorld();
        WorldInstance.GeneratePlayer();
    }
}
