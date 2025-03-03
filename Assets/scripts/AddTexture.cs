using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AddTexture : MonoBehaviour
{
    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;
    Mesh CreateMesh()
    {
        Mesh mesh = new Mesh();

        Vector3[] vertices = new Vector3[]
        {
            // ǰ��
            new Vector3(-0.5f, -0.5f, -0.5f), // 12
            new Vector3( 0.5f, -0.5f, -0.5f), // 13
            new Vector3( 0.5f,  0.5f, -0.5f), // 14
            new Vector3(-0.5f,  0.5f, -0.5f), // 15

            // ����
            new Vector3( 0.5f, -0.5f,  0.5f), // 9
            new Vector3(-0.5f, -0.5f,  0.5f), // 8
            new Vector3(-0.5f,  0.5f,  0.5f), // 11
            new Vector3( 0.5f,  0.5f,  0.5f), // 10

            // ����
            new Vector3(-0.5f, -0.5f,  0.5f), // 17
            new Vector3(-0.5f, -0.5f, -0.5f), // 16
            new Vector3(-0.5f,  0.5f, -0.5f), // 19
            new Vector3(-0.5f,  0.5f,  0.5f), // 18

            // ����
            new Vector3( 0.5f, -0.5f, -0.5f), // 20
            new Vector3( 0.5f, -0.5f,  0.5f), // 21
            new Vector3( 0.5f,  0.5f,  0.5f), // 22
            new Vector3( 0.5f,  0.5f, -0.5f), // 23

            // ����
            new Vector3(-0.5f, 0.5f, -0.5f), // 0
            new Vector3( 0.5f, 0.5f, -0.5f), // 1
            new Vector3( 0.5f, 0.5f,  0.5f), // 2
            new Vector3(-0.5f, 0.5f,  0.5f), // 3

            // ����
            new Vector3( 0.5f, -0.5f, -0.5f), // 5
            new Vector3(-0.5f, -0.5f, -0.5f), // 4
            new Vector3(-0.5f, -0.5f,  0.5f), // 7
            new Vector3( 0.5f, -0.5f,  0.5f)  // 6
        };

        int[] triangles = new int[]
        {
            // ǰ��
            0, 2, 1, 0, 3, 2,

            // ����
            4, 6, 5, 4, 7, 6,

            // ����
            8, 10, 9, 8, 11, 10,

            // ����
            12, 14, 13, 12, 15, 14,

            // ����
            16, 18, 17, 16, 19, 18,

            // ����
            20, 22, 21, 20, 23, 22
        };

        Vector2[] uv = new Vector2[]
        {
            // ǰ��
            new Vector2(0.33f, 0.25f), new Vector2(0.67f, 0.25f), new Vector2(0.67f, 0.5f), new Vector2(0.33f, 0.5f),
            // ����
            new Vector2(0.67f, 1f), new Vector2(0.33f, 1f), new Vector2(0.33f, 0.75f), new Vector2(0.67f, 0.75f),
            // ����
            new Vector2(0, 0.75f), new Vector2(0, 0.5f), new Vector2(0.33f, 0.5f), new Vector2(0.33f, 0.75f),
            // ����
            new Vector2(1f, 0.5f), new Vector2(1f, 0.75f), new Vector2(0.67f, 0.75f), new Vector2(0.67f, 0.5f),
            // ����
            new Vector2(0.33f, 0.5f), new Vector2(0.67f, 0.5f), new Vector2(0.67f, 0.75f), new Vector2(0.33f, 0.75f),
            // ����
            new Vector2(0.33f, 0), new Vector2(0.67f, 0), new Vector2(0.67f, 0.25f), new Vector2(0.33f, 0.25f)
        };

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uv;
        //mesh.RecalculateNormals();

        return mesh;
    }

    // Start is called before the first frame update
    void Start()
    {
        //meshRenderer = GetComponent<MeshRenderer>();
        //meshFilter = GetComponent<MeshFilter>();

        //Mesh mesh = CreateMesh();
        //meshFilter.mesh = mesh;

        //AssetDatabase.CreateAsset(mesh, "Assets/cube.asset");
        //AssetDatabase.Refresh();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
