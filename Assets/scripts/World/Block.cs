using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BlockType { AIR, GRASS, DIRT, STONE, SAND, COBBLE_STONE, COAL_ORE, IRON_ORE, GOLD_ORE, DIAMOND_ORE };

public enum CubeSide { BOTTOM, TOP, RIGHT, LEFT, BACK, FRONT };

public enum BlockUVIndex { GRASS_TOP, GRASS_SIDE, DIRT, STONE, SAND, COBBLE_STONE, COAL_ORE, IRON_ORE, GOLD_ORE, DIAMOND_ORE };

public class Block
{
    public Vector3 position;
    public Vector3 absPosition;
    private float health;
    private float maxHealth;
    private float[] MaxHealth = { -1f, 10f, 10f, 20f, 10f, 20f, 20f, 20f, 20f, 20f };

    private BlockType bType;

    private Chunk owner; // The chunk that this block belongs to

    private enum HealthUVIndex { DAMAGE_0, DAMAGE_1, DAMAGE_2, DAMAGE_3, DAMAGE_4, DAMAGE_5, DAMAGE_6, DAMAGE_7, DAMAGE_8, DAMAGE_9 };

    public static Vector2[,] blockUVs = { 
        /*GRASS TOP*/        {new Vector2( 0.125f, 0.375f ), new Vector2( 0.1875f, 0.375f),
                                new Vector2( 0.125f, 0.4375f ), new Vector2( 0.1875f, 0.4375f )},
        /*GRASS SIDE*/        {new Vector2( 0.1875f, 0.9375f ), new Vector2( 0.25f, 0.9375f),
                                new Vector2( 0.1875f, 1.0f ), new Vector2( 0.25f, 1.0f )},
        /*DIRT*/            {new Vector2( 0.125f, 0.9375f ), new Vector2( 0.1875f, 0.9375f),
                                new Vector2( 0.125f, 1.0f ), new Vector2( 0.1875f, 1.0f )},
        /*STONE*/            {new Vector2( 0.0625f, 0.9375f ), new Vector2( 0.125f, 0.9375f ),
                                new Vector2( 0.0625f, 1.0f ), new Vector2( 0.125f, 1.0f )},
        /*SAND*/            {new Vector2( 0.125f, 0.875f ), new Vector2( 0.1875f, 0.875f ),
                                new Vector2( 0.125f, 0.9375f ), new Vector2( 0.1875f, 0.9375f )},
        /*COBBLE STONE*/    {new Vector2( 0.0f, 0.875f ), new Vector2( 0.0625f, 0.875f ),
                                new Vector2( 0.0f, 0.9375f ), new Vector2( 0.0625f, 0.9375f )},
        /*COAL ORE*/        {new Vector2( 0.125f, 0.8125f ), new Vector2( 0.1875f, 0.8125f ),
                                new Vector2( 0.125f, 0.875f ), new Vector2( 0.1875f, 0.875f )},
        /*IRON ORE*/        {new Vector2( 0.0625f, 0.8125f ), new Vector2( 0.125f, 0.8125f ),
                                new Vector2( 0.0625f, 0.875f ), new Vector2( 0.125f, 0.875f )},
        /*GOLD ORE*/        {new Vector2( 0.0f, 0.8125f ), new Vector2( 0.0625f, 0.8125f ),
                                new Vector2( 0.0f, 0.875f ), new Vector2( 0.0625f, 0.875f )},
        /*DIAMOND ORE*/        {new Vector2( 0.125f, 0.75f ), new Vector2( 0.1875f, 0.75f ),
                                new Vector2( 0.125f, 0.8125f ), new Vector2( 0.1875f, 0.8125f )}
    };

    private static Vector2[,] healthUVs =
    {
        {new Vector2(0.5625f, 0.0f), new Vector2(0.625f, 0.0f), new Vector2(0.5625f, 0.0625f), new Vector2(0.625f, 0.0625f)},
        {new Vector2(0.5f, 0.0f), new Vector2(0.5625f, 0.0f), new Vector2(0.5f, 0.0625f), new Vector2(0.5625f, 0.0625f)},
        {new Vector2(0.4375f, 0.0f), new Vector2(0.5f, 0.0f), new Vector2(0.4375f, 0.0625f), new Vector2(0.5f, 0.0625f)},
        {new Vector2(0.375f, 0.0f), new Vector2(0.4375f, 0.0f), new Vector2(0.375f, 0.0625f), new Vector2(0.4375f, 0.0625f)},
        {new Vector2(0.3125f, 0.0f), new Vector2(0.375f, 0.0f), new Vector2(0.3125f, 0.0625f), new Vector2(0.375f, 0.0625f)},
        {new Vector2(0.25f, 0.0f), new Vector2(0.3125f, 0.0f), new Vector2(0.25f, 0.0625f), new Vector2(0.3125f, 0.0625f)},
        {new Vector2(0.1875f, 0.0f), new Vector2(0.25f, 0.0f), new Vector2(0.1875f, 0.0625f), new Vector2(0.25f, 0.0625f)},
        {new Vector2(0.125f, 0.0f), new Vector2(0.1875f, 0.0f), new Vector2(0.125f, 0.0625f), new Vector2(0.1875f, 0.0625f)},
        {new Vector2(0.0625f, 0.0f), new Vector2(0.125f, 0.0f), new Vector2(0.0625f, 0.0625f), new Vector2(0.125f, 0.0625f)},
        {new Vector2(0.0f, 0.0f), new Vector2(0.0625f, 0.0f), new Vector2(0, 0.0625f), new Vector2(0.0625f, 0.0625f)},
        {new Vector2(0.25f, 0.125f), new Vector2(0.3125f, 0.125f), new Vector2(0.25f, 0.1875f), new Vector2(0.3125f, 0.1875f)}
    };

    public Block(BlockType bType, Chunk owner, Vector3 pos)
    {
        this.bType = bType;
        this.owner = owner;
        health = maxHealth = MaxHealth[(int)bType];
        position = pos;
        absPosition = owner.transform.position + position;
    }

    public void CreateCube()
    {
        if (bType == BlockType.AIR)
        {
            return;
        }

        if (!owner.CheckForVoxel(position + new Vector3(0, 0, 1)))
        {
            CreateQuad(CubeSide.FRONT);
        }
        if (!owner.CheckForVoxel(position + new Vector3(0, 0, -1)))
        {
            CreateQuad(CubeSide.BACK);
        }
        if (!owner.CheckForVoxel(position + new Vector3(0, 1, 0)))
        {
            CreateQuad(CubeSide.TOP);
        }
        if (!owner.CheckForVoxel(position + new Vector3(0, -1, 0)))
        {
            CreateQuad(CubeSide.BOTTOM);
        }
        if (!owner.CheckForVoxel(position + new Vector3(1, 0, 0)))
        {
            CreateQuad(CubeSide.RIGHT);
        }
        if (!owner.CheckForVoxel(position + new Vector3(-1, 0, 0)))
        {
            CreateQuad(CubeSide.LEFT);
        }
    }

    private void CreateQuad(CubeSide side)
    {
        Mesh mesh = new Mesh();
        mesh.name = "S_Mesh" + side.ToString();

        Vector3[] vertices = new Vector3[4];
        Vector3[] normals = new Vector3[4];
        Vector2[] uvs = new Vector2[4];
        int[] triangles = new int[6];
        List<Vector2> suvs = new List<Vector2>();

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
        Vector2 uv00 = blockUVs[0, 0];
        Vector2 uv10 = blockUVs[0, 1];
        Vector2 uv01 = blockUVs[0, 2];
        Vector2 uv11 = blockUVs[0, 3];

        if (bType == BlockType.GRASS)
        {
            if (side == CubeSide.TOP)
            {
                uv00 = blockUVs[(int)BlockUVIndex.GRASS_TOP, 0];
                uv10 = blockUVs[(int)BlockUVIndex.GRASS_TOP, 1];
                uv01 = blockUVs[(int)BlockUVIndex.GRASS_TOP, 2];
                uv11 = blockUVs[(int)BlockUVIndex.GRASS_TOP, 3];
            }
            else if (side == CubeSide.BOTTOM)
            {
                uv00 = blockUVs[(int)BlockUVIndex.DIRT, 0];
                uv10 = blockUVs[(int)BlockUVIndex.DIRT, 1];
                uv01 = blockUVs[(int)BlockUVIndex.DIRT, 2];
                uv11 = blockUVs[(int)BlockUVIndex.DIRT, 3];
            }
            else
            {
                uv00 = blockUVs[(int)BlockUVIndex.GRASS_SIDE, 0];
                uv10 = blockUVs[(int)BlockUVIndex.GRASS_SIDE, 1];
                uv01 = blockUVs[(int)BlockUVIndex.GRASS_SIDE, 2];
                uv11 = blockUVs[(int)BlockUVIndex.GRASS_SIDE, 3];
            }
        }
        else if (bType != BlockType.AIR)
        {
            uv00 = blockUVs[(int)bType, 0];
            uv10 = blockUVs[(int)bType, 1];
            uv01 = blockUVs[(int)bType, 2];
            uv11 = blockUVs[(int)bType, 3];
        }

        //set cracks
        suvs.Add(healthUVs[(int)(10 * health / maxHealth), 3]);
        suvs.Add(healthUVs[(int)(10 * health / maxHealth), 2]);
        suvs.Add(healthUVs[(int)(10 * health / maxHealth), 0]);
        suvs.Add(healthUVs[(int)(10 * health / maxHealth), 1]);

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
        mesh.SetUVs(1, suvs);
        mesh.triangles = triangles;

        mesh.RecalculateBounds();

        GameObject quad = new GameObject("quad");
        quad.transform.parent = owner.transform;
        quad.transform.localPosition = position;
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

    public bool GetHit(float damage)
    {
        int prevHealth = (int)health;
        health -= damage;
        if (health < 0)
        {
            GameObject dropBlockObject = new GameObject("DropBlock");
            DropBlock dropBlock = dropBlockObject.AddComponent<DropBlock>();
            dropBlock.Initialize(this);
            //Debug.Log("add drop block" + absPosition);
            GameObject.Find("World").GetComponent<World>().AddDropBlock(dropBlock, absPosition);

            bType = BlockType.AIR;

            owner.RedrawChunk();
            return true;
        }

        if((int)health != prevHealth)
        {
            owner.RedrawChunk();
        }

        return false;
    }

    public void Recover()
    {
        health = maxHealth = MaxHealth[(int)bType];
        owner.RedrawChunk();
    }
}
