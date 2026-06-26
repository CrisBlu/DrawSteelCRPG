using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class MB_GenerateMesh : MonoBehaviour
{
    [SerializeField] SO_GridData GridData;
    private Vector3[] vertices;

    private void Awake()
    {

        GenerateGrid();
    }

    private void GenerateGrid()
    {
        Mesh mesh;
        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        mesh.name = "Procedural Grid";

        float offset = .05f;
        float farOffset = 1 - offset;

        vertices = new Vector3[GridData.size * GridData.size * 4];
        for (int i = 0, y = 0; y < GridData.size; y++)
        {
            for (int x = 0; x < GridData.size; x++, i += 4)
            {
                vertices[i] = new Vector3(x + offset, 0, y + offset);
                vertices[i+1] = new Vector3(x+ farOffset, 0, y + offset);
                vertices[i+2] = new Vector3(x + offset, 0, y + farOffset);
                vertices[i+3] = new Vector3(x + farOffset, 0, y + farOffset);

            }
        }

        mesh.vertices = vertices;

        int[] triangles = new int[GridData.size * GridData.size * 6];

        for (int ti = 0, vi = 0, y = 0; y < GridData.size; y++)
        {
            for (int x = 0; x < GridData.size; x++, ti += 6, vi +=4)
            {
                triangles[ti] = vi;
                triangles[ti + 3] = triangles[ti + 2] = vi + 1;
                triangles[ti + 4] = triangles[ti + 1] = vi + 2;
                triangles[ti + 5] = vi + 3;
            }
        }

  




        mesh.triangles = triangles;
        mesh.RecalculateNormals();



        gameObject.AddComponent<MeshCollider>();

        GridData.mesh = mesh;

        CS_ColorGrid.ClearGridColors(GridData);

        


    }
}
