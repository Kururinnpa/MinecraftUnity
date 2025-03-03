using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class World : MonoBehaviour
{
    private const int renderChunks = 5;
    private const int totalChunks = (renderChunks * 2 + 1) * (renderChunks * 2 + 1);
    private int progressChunks = 0;

    private int seed;
    private List<Tuple<DropBlock, Vector3>> dropBlocks = new List<Tuple<DropBlock, Vector3>>();
    private Dictionary<Vector2Int, Chunk> chunkDict = new Dictionary<Vector2Int, Chunk>();
    private ConcurrentDictionary<Vector2Int, Chunk> visibleChunks = new ConcurrentDictionary<Vector2Int, Chunk>();
    private Vector2 lastChunkPos;
    public GameObject playerFab;
    private GameObject playerObject;

    public Material cubeMat;

    // Start is called before the first frame update
    public void SetSeed()
    {
        seed = UnityEngine.Random.Range(100, 300);
    }

    void Start()
    {
        gameObject.name = "World";
        SetSeed();
        GeneratePlayer();
        StartCoroutine(GenerateWorld());
    }

    //public void InitWorld()
    //{
    //    gameObject.name = "World";
    //    SetSeed();
    //    StartCoroutine(GenerateWorld());
    //    GeneratePlayer();
    //}

    // Update is called once per frame
    void Update()
    {
        StartCoroutine(UpdateWorld());
    }

    public void GeneratePlayer()
    {
        playerObject = Instantiate(playerFab, new Vector3(0, 20, 0), Quaternion.identity);
        playerObject.AddComponent<Player>();
        playerObject.name = "Player";
        playerObject.tag = "Player";
        playerObject.layer = LayerMask.NameToLayer("Player");
        playerObject.SetActive(false);
    }

    IEnumerator GenerateWorld()
    {
        int playerX = 0;
        int playerZ = 0;
        for (int x = -renderChunks; x <= renderChunks; x++)
        {
            for (int z = -renderChunks; z <= renderChunks; z++)
            {
                //StartCoroutine(LoadChunk(x + playerX, z + playerZ));
                LoadChunk(x + playerX, z + playerZ);
                progressChunks++;
                LoadManager.instance.SetProgress((float)progressChunks / totalChunks);
                if(progressChunks == totalChunks)
                {
                    playerObject.SetActive(true);
                    LoadManager.instance.loadScreen.SetActive(false);
                }

                yield return null;
            }
        }

        lastChunkPos = new Vector2Int(playerX, playerZ);
    }

    IEnumerator UpdateWorld()
    {
        int playerX = Mathf.FloorToInt(playerObject.transform.position.x / Chunk.chunkSize);
        int playerZ = Mathf.FloorToInt(playerObject.transform.position.z / Chunk.chunkSize);
        Vector2 currentChunkPos = new Vector2(playerX, playerZ);

        if (currentChunkPos != lastChunkPos)
        {
            lastChunkPos = currentChunkPos;

            // remove chunks
            foreach(KeyValuePair<Vector2Int, Chunk> chunk in visibleChunks)
            {
                if (Mathf.Abs(chunk.Key.x - playerX) > renderChunks || Mathf.Abs(chunk.Key.y - playerZ) > renderChunks)
                {
                    //StartCoroutine(UnloadChunk(chunk.Key.x, chunk.Key.y));
                    UnloadChunk(chunk.Key.x, chunk.Key.y);
                    yield return null;
                }
            }

            // add chunks
            for (int x = -renderChunks; x <= renderChunks; x++)
            {
                for (int z = -renderChunks; z <= renderChunks; z++)
                {
                    Vector2Int chunkPos = new Vector2Int(x + playerX, z + playerZ);
                    if (!chunkDict.ContainsKey(chunkPos))
                    {
                        //Debug.Log("start load chunk");
                        //StartCoroutine(LoadChunk(chunkPos.x, chunkPos.y));
                        LoadChunk(chunkPos.x, chunkPos.y);
                        yield return null;
                    }
                    //else
                    //{
                    //    chunkDict[chunkPos].GetComponent<MeshRenderer>().enabled = true;
                    //}
                }
            }
        }
    }

    private void LoadChunk(int x, int z)
    {
        GameObject chunkObject = new GameObject();
        chunkObject.transform.parent = this.transform;
        chunkObject.transform.position = new Vector3(x * Chunk.chunkSize, 0, z * Chunk.chunkSize);
        chunkObject.name = "Chunk " + x + ", " + z;
        chunkObject.tag = "Chunk";
        chunkObject.layer = LayerMask.NameToLayer("Chunk");

        Chunk chunk = chunkObject.AddComponent<Chunk>();
        Vector2Int chunkPos = new Vector2Int(x, z);
        chunkDict.Add(chunkPos, chunk);
        visibleChunks.TryAdd(chunkPos, chunk);
        chunk.InitChunk(cubeMat, seed);
        StartCoroutine(chunk.GenerateChunk());

        //yield return null;
    }

    private void UnloadChunk(int x, int z)
    {
        Vector2Int chunkPos = new Vector2Int(x, z);
        if (chunkDict.ContainsKey(chunkPos))
        {
            Destroy(chunkDict[chunkPos].gameObject);
            chunkDict.Remove(chunkPos);

            visibleChunks.TryRemove(chunkPos, out Chunk c);
        }
        //yield return null;
    }

    public Chunk GetChunk(int x, int z)
    {
        return chunkDict[new Vector2Int(x, z)];
    }

    public void AddDropBlock(DropBlock dropBlock, Vector3 pos)
    {
        dropBlocks.Add(new Tuple<DropBlock, Vector3>(dropBlock, pos));
    }

    public void RemoveDropBlock(DropBlock dropBlock)
    {
        dropBlocks.RemoveAll(tuple => tuple.Item1 == dropBlock);
    }

    public void UpdateDropBlock(DropBlock dropBlock, Vector3 newPos)
    {
        for (int i = 0; i < dropBlocks.Count; i++)
        {
            if (dropBlocks[i].Item1 == dropBlock)
            {
                dropBlocks[i] = new Tuple<DropBlock, Vector3>(dropBlock, newPos);
                return;
            }
        }
    }

    // ����ĳ��λ���Ƿ��з���
    public bool IsDropBlockAtPosition(Vector3 position)
    {
        foreach (var item in dropBlocks)
        {
            if (item.Item2 == position)
            {
                return true; // �ҵ�ƥ��λ�ã�����true
            }
        }
        return false; // û���ҵ�ƥ���λ�ã�����false
    }

    public DropBlock GetDropBlockAtPosition(Vector3 position)
    {
        foreach (var item in dropBlocks)
        {
            if (item.Item2 == position)
            {
                return item.Item1;
            }
        }
        return null;
    }
}
