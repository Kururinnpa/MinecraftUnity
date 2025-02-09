using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class World : MonoBehaviour
{
    private const int renderChunks = 2;
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

    // Update is called once per frame
    void Update()
    {
        UpdateWorld();
    }

    public void GeneratePlayer()
    {
        playerObject = Instantiate(playerFab, new Vector3(0, 20, 0), Quaternion.identity);
        playerObject.AddComponent<Player>();
        playerObject.name = "Player";
        playerObject.tag = "Player";
        playerObject.layer = LayerMask.NameToLayer("Player");
    }

    public void GenerateWorld()
    {
        int playerX = 0;
        int playerZ = 0;
        for (int x = -renderChunks; x <= renderChunks; x++)
        {
            for (int z = -renderChunks; z <= renderChunks; z++)
            {
                StartCoroutine(LoadChunk(x + playerX, z + playerZ));
            }
        }

        lastChunkPos = new Vector2Int(playerX, playerZ);
    }

    private void UpdateWorld()
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
                    StartCoroutine(UnloadChunk(chunk.Key.x, chunk.Key.y));
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
                        StartCoroutine(LoadChunk(chunkPos.x, chunkPos.y));
                    }
                    //else
                    //{
                    //    chunkDict[chunkPos].GetComponent<MeshRenderer>().enabled = true;
                    //}
                }
            }
        }
    }

    private IEnumerator LoadChunk(int x, int z)
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

        yield return null;
    }

    private IEnumerator UnloadChunk(int x, int z)
    {
        Vector2Int chunkPos = new Vector2Int(x, z);
        if (chunkDict.ContainsKey(chunkPos))
        {
            Destroy(chunkDict[chunkPos].gameObject);
            chunkDict.Remove(chunkPos);

            visibleChunks.TryRemove(chunkPos, out Chunk c);
        }
        yield return null;
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

    // 查找某个位置是否有方块
    public bool IsDropBlockAtPosition(Vector3 position)
    {
        foreach (var item in dropBlocks)
        {
            if (item.Item2 == position)
            {
                return true; // 找到匹配位置，返回true
            }
        }
        return false; // 没有找到匹配的位置，返回false
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
