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

        vertices = new Vector3[(GridData.size + 1) * (GridData.size + 1)];
        for (int i = 0, y = 0; y <= GridData.size; y++)
        {
            for (int x = 0; x <= GridData.size; x++, i++)
            {
                vertices[i] = new Vector3(x, 0, y);
            }
        }

        mesh.vertices = vertices;


        int[] triangles = new int[GridData.size * GridData.size * 6];

        for (int ti = 0, vi = 0, y = 0; y < GridData.size; y++, vi++)
        {
            for (int x = 0; x < GridData.size; x++, ti += 6, vi++)
            {
                triangles[ti] = vi;
                triangles[ti + 3] = triangles[ti + 2] = vi + 1;
                triangles[ti + 4] = triangles[ti + 1] = vi + GridData.size + 1;
                triangles[ti + 5] = vi + GridData.size + 2;
            }
        }


        mesh.triangles = triangles;
        mesh.RecalculateNormals();



        gameObject.AddComponent<MeshCollider>();

        GridData.mesh = mesh;


    }
}
