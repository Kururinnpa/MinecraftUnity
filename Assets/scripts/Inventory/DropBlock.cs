using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class DropBlock : MonoBehaviour
{
    private Vector3 localPosition;
    private BlockType bType;
    public Material cubeMat;

    private Rigidbody rigid;
    private BoxCollider boxCollider;

    private Item item;

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up, 90 * Time.deltaTime);
    }

    public void Initialize(Block block)
    {
        gameObject.layer = LayerMask.NameToLayer("Drop");
        transform.position = block.absPosition;
        localPosition = block.position;

        cubeMat = Resources.Load<Material>("Materials/dropBlock");
        bType = block.GetBlockType();

        CreateCube();
        CombineBlockMeshes();
        transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);

        rigid = gameObject.AddComponent<Rigidbody>();
        rigid.freezeRotation = true;

        boxCollider = gameObject.AddComponent<BoxCollider>();
        boxCollider.size = new Vector3(1.5f, 1.5f, 1.5f);

        item = Resources.Load<Item>("Items/" + Item.ItemTypes[(int)bType]);
        //if (bType == BlockType.GRASS || bType == BlockType.DIRT)
        //{
        //    item = Resources.Load<Item>("Items/Dirt");
        //}
        //else if (bType == BlockType.STONE)
        //{
        //    item = Resources.Load<Item>("Items/Stone");
        //}
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Player player = other.gameObject.GetComponent<Player>();
            player.AddItemToInventory(item);
            Destroy(gameObject);
            GameObject.Find("World").GetComponent<World>().RemoveDropBlock(this);
        }
    }

    public void MoveBlock()
    {
        int chunkX = Mathf.FloorToInt(transform.position.x / Chunk.chunkSize);
        int chunkZ = Mathf.FloorToInt(transform.position.z / Chunk.chunkSize);
        Chunk chunk = GameObject.Find("World").GetComponent<World>().GetChunk(chunkX, chunkZ);
        Debug.Log(chunk.gameObject);
        Debug.Log("local pos: " + localPosition);
        Debug.Log("if has block: " + chunk.CheckForVoxel(localPosition));

        Destroy(gameObject);
        GameObject.Find("World").GetComponent<World>().RemoveDropBlock(this);

        //Vector3 newPos = localPosition + new Vector3(1, 0, 0);
        //if (newPos.x >= Chunk.chunkSize)
        //{
        //    newPos.x = 0;
        //    chunk = GameObject.Find("World").GetComponent<World>().GetChunk(chunkX + 1, chunkZ);
        //}
        //if (!chunk.CheckForVoxel(newPos))
        //{
        //    Debug.Log("test right");
        //    //Debug.Log(chunk.gameObject);
        //    //Debug.Log("new pos: " + newPos);
        //    //Debug.Log("local pos: " + localPosition);
        //    //Debug.Log("old transform pos: " + transform.position);
        //    //rigid.MovePosition(transform.position + new Vector3(1, 0, 0));
        //    transform.position += new Vector3(1, 0, 0);
        //    localPosition = newPos;
        //    GameObject.Find("World").GetComponent<World>().UpdateDropBlock(this, transform.position);
        //    //Debug.Log("new transform pos: " + transform.position);
        //    return;
        //}

        //newPos = localPosition + new Vector3(-1, 0, 0);
        //if (newPos.x < 0)
        //{
        //    newPos.x = Chunk.chunkSize - 1;
        //    chunk = GameObject.Find("World").GetComponent<World>().GetChunk(chunkX - 1, chunkZ);
        //}
        //if (!chunk.CheckForVoxel(newPos))
        //{
        //    Debug.Log("test left");
        //    //Debug.Log(chunk.gameObject);
        //    //Debug.Log("new pos: " + newPos);
        //    //Debug.Log("local pos: " + localPosition);
        //    transform.position += new Vector3(-1, 0, 0);
        //    localPosition = newPos;
        //    GameObject.Find("World").GetComponent<World>().UpdateDropBlock(this, transform.position);
        //    return;
        //}

        //newPos = localPosition + new Vector3(0, 0, 1);
        //if (newPos.z >= Chunk.chunkSize)
        //{
        //    newPos.z = 0;
        //    chunk = GameObject.Find("World").GetComponent<World>().GetChunk(chunkX, chunkZ + 1);
        //}
        //if (!chunk.CheckForVoxel(newPos))
        //{
        //    Debug.Log("test back");
        //    transform.position += new Vector3(0, 0, 1);
        //    localPosition = newPos;
        //    GameObject.Find("World").GetComponent<World>().UpdateDropBlock(this, transform.position);
        //    return;
        //}

        //newPos = localPosition + new Vector3(0, 0, -1);
        //if (newPos.z < 0)
        //{
        //    newPos.z = Chunk.chunkSize - 1;
        //    chunk = GameObject.Find("World").GetComponent<World>().GetChunk(chunkX, chunkZ - 1);
        //}
        //if (!chunk.CheckForVoxel(newPos))
        //{
        //    Debug.Log("test front");
        //    transform.position += new Vector3(0, 0, -1);
        //    localPosition = newPos;
        //    GameObject.Find("World").GetComponent<World>().UpdateDropBlock(this, transform.position);
        //    return;
        //}

        //newPos = localPosition + new Vector3(0, 1, 0);
        //if (!chunk.CheckForVoxel(newPos))
        //{
        //    Debug.Log("test up");
        //    transform.position += new Vector3(0, 1, 0);
        //    localPosition = newPos;
        //    GameObject.Find("World").GetComponent<World>().UpdateDropBlock(this, transform.position);
        //    return;
        //}
    }

    public void CreateCube()
    {
        CreateQuad(CubeSide.FRONT);
        CreateQuad(CubeSide.BACK);
        CreateQuad(CubeSide.TOP);
        CreateQuad(CubeSide.BOTTOM);
        CreateQuad(CubeSide.RIGHT);
        CreateQuad(CubeSide.LEFT);
    }

    private void CreateQuad(CubeSide side)
    {
        Mesh mesh = new Mesh();
        mesh.name = "S_Mesh" + side.ToString();

        Vector3[] vertices = new Vector3[4];
        Vector3[] normals = new Vector3[4];
        Vector2[] uvs = new Vector2[4];
        int[] triangles = new int[6];

        //all possible vertcies 
        Vector3 p0 = new Vector3(-0.5f, -0.5f, 0.5f);
        Vector3 p1 = new Vector3(0.5f, -0.5f, 0.5f);
        Vector3 p2 = new Vector3(0.5f, -0.5f, -0.5f);
        Vector3 p3 = new Vector3(-0.5f, -0.5f, -0.5f);
        Vector3 p4 = new Vector3(-0.5f, 0.5f, 0.5f);
        Vector3 p5 = new Vector3(0.5f, 0.5f, 0.5f);
        Vector3 p6 = new Vector3(0.5f, 0.5f, -0.5f);
        Vector3 p7 = new Vector3(-0.5f, 0.5f, -0.5f);

        //all possible UVs
        Vector2 uv00 = Block.blockUVs[0, 0];
        Vector2 uv10 = Block.blockUVs[0, 1];
        Vector2 uv01 = Block.blockUVs[0, 2];
        Vector2 uv11 = Block.blockUVs[0, 3];

        if (bType == BlockType.GRASS || bType == BlockType.DIRT)
        {
            uv00 = Block.blockUVs[(int)BlockUVIndex.DIRT, 0];
            uv10 = Block.blockUVs[(int)BlockUVIndex.DIRT, 1];
            uv01 = Block.blockUVs[(int)BlockUVIndex.DIRT, 2];
            uv11 = Block.blockUVs[(int)BlockUVIndex.DIRT, 3];
        }
        else if (bType == BlockType.STONE)
        {
            uv00 = Block.blockUVs[(int)BlockUVIndex.STONE, 0];
            uv10 = Block.blockUVs[(int)BlockUVIndex.STONE, 1];
            uv01 = Block.blockUVs[(int)BlockUVIndex.STONE, 2];
            uv11 = Block.blockUVs[(int)BlockUVIndex.STONE, 3];
        }
        else if (bType == BlockType.LAVA)
        {
            uv00 = Block.blockUVs[(int)BlockUVIndex.LAVA, 0];
            uv10 = Block.blockUVs[(int)BlockUVIndex.LAVA, 1];
            uv01 = Block.blockUVs[(int)BlockUVIndex.LAVA, 2];
            uv11 = Block.blockUVs[(int)BlockUVIndex.LAVA, 3];
        }

        switch (side)
        {
            case CubeSide.BOTTOM:
                vertices = new Vector3[] { p0, p1, p2, p3 };
                normals = new Vector3[] {Vector3.down,
                                 Vector3.down,
                                 Vector3.down,
                                 Vector3.down};
                uvs = new Vector2[] { uv11, uv01, uv00, uv10 };
                triangles = new int[] { 3, 1, 0, 3, 2, 1 };
                break;
            case CubeSide.TOP:
                vertices = new Vector3[] { p7, p6, p5, p4 };
                normals = new Vector3[] {Vector3.up,
                                 Vector3.up,
                                 Vector3.up,
                                 Vector3.up};
                uvs = new Vector2[] { uv11, uv01, uv00, uv10 };
                triangles = new int[] { 3, 1, 0, 3, 2, 1 };
                break;
            case CubeSide.RIGHT:
                vertices = new Vector3[] { p5, p6, p2, p1 };
                normals = new Vector3[] {Vector3.right,
                                 Vector3.right,
                                 Vector3.right,
                                 Vector3.right};
                uvs = new Vector2[] { uv11, uv01, uv00, uv10 };
                triangles = new int[] { 3, 1, 0, 3, 2, 1 };
                break;
            case CubeSide.LEFT:
                vertices = new Vector3[] { p7, p4, p0, p3 };
                normals = new Vector3[] {Vector3.left,
                                 Vector3.left,
                                 Vector3.left,
                                 Vector3.left};
                uvs = new Vector2[] { uv11, uv01, uv00, uv10 };
                triangles = new int[] { 3, 1, 0, 3, 2, 1 };
                break;
            case CubeSide.FRONT:
                vertices = new Vector3[] { p4, p5, p1, p0 };
                normals = new Vector3[] {Vector3.forward,
                                 Vector3.forward,
                                 Vector3.forward,
                                 Vector3.forward};
                uvs = new Vector2[] { uv11, uv01, uv00, uv10 };
                triangles = new int[] { 3, 1, 0, 3, 2, 1 };
                break;
            case CubeSide.BACK:
                vertices = new Vector3[] { p6, p7, p3, p2 };
                normals = new Vector3[] {Vector3.back,
                                 Vector3.back,
                                 Vector3.back,
                                 Vector3.back};
                uvs = new Vector2[] { uv11, uv01, uv00, uv10 };
                triangles = new int[] { 3, 1, 0, 3, 2, 1 };
                break;
        }
        mesh.vertices = vertices;
        mesh.normals = normals;
        mesh.uv = uvs;
        mesh.triangles = triangles;

        mesh.RecalculateBounds();

        GameObject quad = new GameObject("quad");
        quad.transform.parent = transform;
        quad.transform.localPosition = Vector3.zero;
        MeshFilter meshFilter = (MeshFilter)quad.AddComponent(typeof(MeshFilter));
        meshFilter.mesh = mesh;
    }

    public BlockType GetBlockType()
    {
        return bType;
    }
    
    public void SetBlockType(BlockType bType)
    {
        this.bType = bType;
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
}
