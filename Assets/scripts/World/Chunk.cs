using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Chunk : MonoBehaviour
{
    public static int chunkSize = 16;
    public static int chunkHeight = 16;
    public static int maxChunkHeight = 16;
    private int seed;

    private Block[,,] blocks;

    public Material cubeMat;

    public void InitChunk(Material cubeMat, int seed)
    {
        this.cubeMat = cubeMat;
        this.seed = seed;
    }

    public IEnumerator GenerateChunk()
    {
        CreateChunk();
        CreateBlocks();
        CombineBlockMeshes();
        CreateCollider();
        yield return null;
    }

    public void DrawChunk()
    {
        CreateBlocks();
        CombineBlockMeshes();
        CreateCollider();
    }

    public void RedrawChunk()
    {
        DestroyImmediate(gameObject.GetComponent<MeshFilter>());
        DestroyImmediate(gameObject.GetComponent<MeshRenderer>());
        DestroyImmediate(gameObject.GetComponent<MeshCollider>());
        DrawChunk();
    }

    public bool CheckForVoxel(Vector3 pos)
    {
        int xCheck = Mathf.FloorToInt(pos.x);
        int yCheck = Mathf.FloorToInt(pos.y);
        int zCheck = Mathf.FloorToInt(pos.z);
        if (xCheck < 0 || xCheck >= chunkSize || yCheck < 0 || yCheck >= maxChunkHeight || zCheck < 0 || zCheck >= chunkSize)
        {
            return false;
        }
        return blocks[xCheck, yCheck, zCheck].GetBlockType() != BlockType.AIR;
    }

    private void CreateChunk()
    {
        blocks = new Block[chunkSize, maxChunkHeight, chunkSize];
        int i = 0;
        for (int x = 0; x < chunkSize; x++)
        {
            for (int z = 0; z < chunkSize; z++)
            {
                //seed = 300;
                float worldX = transform.position.x + x;
                float worldZ = transform.position.z + z;
                float height = PerlinNoise.GenerateHeight(worldX + seed, worldZ + seed);
                for (int y = 0; y < maxChunkHeight; y++)
                {
                    Vector3 pos = new Vector3(x, y, z);
                    if(pos.y >= height)
                    {
                        blocks[x, y, z] = new Block(BlockType.AIR, this, pos);
                    }
                    else if (pos.y == height - 1)
                    {
                        blocks[x, y, z] = new Block(BlockType.GRASS, this, pos);
                    }
                    else if (pos.y > 3)
                    {
                        blocks[x, y, z] = new Block(BlockType.DIRT, this, pos);
                    }
                    else
                    {
                        blocks[x, y, z] = new Block(BlockType.STONE, this, pos);
                    }

                    i++;
                }
            }
        }
    }

    private void CreateBlocks()
    {
        foreach (Block block in blocks)
        {
            block.CreateCube();
        }
    }

    private void CombineBlockMeshes()
    {
        // 合并所有子对象的网格
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];

        int i = 0;
        while (i < meshFilters.Length)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            // 这里的filter是quad，parent是block，parent.parent是chunk，计算quad在chunk中的位置
            combine[i].transform = meshFilters[i].transform.parent.localToWorldMatrix.inverse * meshFilters[i].transform.localToWorldMatrix;
            i++;
        }

        // 为父对象创建一个新的MeshFilter并将合并的网格添加到其中
        MeshFilter mf = (MeshFilter)gameObject.AddComponent(typeof(MeshFilter));
        mf.mesh = new Mesh();

        mf.mesh.CombineMeshes(combine);


        // 为父对象创建一个新的MeshRenderer并将材质添加到其中
        MeshRenderer meshRenderer = gameObject.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
        meshRenderer.material = cubeMat;


        // 删除所有未合并的子对象
        foreach (Transform quad in transform)
        {
            Destroy(quad.gameObject);
        }
    }

    private void CreateCollider()
    {
        MeshCollider meshCollider = gameObject.AddComponent<MeshCollider>();
        Mesh chunkMesh = GetComponent<MeshFilter>().mesh; // 假设Chunk已经有MeshFilter组件
        meshCollider.sharedMesh = chunkMesh;
    }

    public Block GetBlock(int x, int y, int z)
    {
        return blocks[x, y, z];
    }
}
