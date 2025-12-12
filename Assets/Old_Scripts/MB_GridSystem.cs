
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

//Deal with user interaction that effects the Grid Data
public class MB_GridSystem : MonoBehaviour
{
    [SerializeField] SO_GridSystem data;
    [SerializeField] SO_BattleManager BattleManager;

    private Vector3[] vertices;
    Color[] colors;

    InputAction selectAction;
    private void Awake()
    {/*
        selectAction = InputSystem.actions.FindAction("Select");
        selectAction.performed += CellSelect;

        data.EventGridUpdate.AddListener(ColorCells);
        BattleManager.EventTurnEnd.AddListener(ClearGridColors);
        */

        GenerateGrid();
    }


    //Mouse over and highlight Grid Cells
    [SerializeField] private Camera sceneCamera;
    [SerializeField] private Grid Map;
    [SerializeField] private GameObject cellIndicator;

    CS_GridSelect GridSelect = new CS_GridSelect();

    private Vector3Int currentCell;
    void Update()
    {
        //Get Cell user is mousing over
        Vector3 mousePosition = GridSelect.GetSelectedMapPosition(sceneCamera);
        currentCell = Map.WorldToCell(mousePosition);

        //Highlight Cell
        cellIndicator.transform.position = Map.CellToWorld(currentCell);
    }



    //Select a Cell on the Grid
    void CellSelect(InputAction.CallbackContext context)
    {
        //Watch for entries outside of the bounds of the array
        data.GridCellSelect(CoordTranslate(currentCell));
    }

    private Vector2Int CoordTranslate(Vector3 unityGridCoords)
    {
        Vector2Int correctedCords = new Vector2Int((int)unityGridCoords.x + 10/ 2, (int)unityGridCoords.z + 10 / 2);
        return correctedCords;
    }

    private Mesh mesh;
    private void GenerateGrid()
    {
        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        mesh.name = "Procedural Grid";
       
        vertices = new Vector3[(data.size + 1) * (data.size + 1)];
        for(int i = 0, y = 0; y <= data.size; y++)
        {
            for(int x = 0; x <= data.size; x++, i++)
            {
                vertices[i] = new Vector3(x, 0, y);
            }
        }

        mesh.vertices = vertices;


        int[] triangles = new int[data.size * data.size * 6];
       
        for (int ti = 0, vi = 0, y = 0; y < data.size; y++, vi++)
        {
            for (int x = 0; x < data.size; x++, ti += 6, vi++)
            {
                triangles[ti] = vi;
                triangles[ti + 3] = triangles[ti + 2] = vi + 1;
                triangles[ti + 4] = triangles[ti + 1] = vi + data.size + 1;
                triangles[ti + 5] = vi + data.size + 2;
            }

        }

        

        colors = new Color[vertices.Length];
        for (int i = 0; i < vertices.Length; i++)
            colors[i] = Color.gray;


        
        mesh.colors = colors;

        //ColorCell(new Vector2Int(1, 1));


        /*triangles[0] = 0;
        triangles[1] = data.size + 1;
        triangles[2] = 1;
        triangles[3] = 1;
        triangles[4] = data.size + 1;
        triangles[5] = data.size + 2;*/
        mesh.triangles = triangles;
        mesh.RecalculateNormals();


        
        gameObject.AddComponent<MeshCollider>();
        

    }

    public void ColorCells(List<Tile> cellsToColor, Color color)
    {
        ClearGridColors();

        foreach (Tile cell in cellsToColor)
        {
            Vector2Int cellPosition = cell.position;
        
            int x = cellPosition.x;
            int y = cellPosition.y;
            int size = data.size + 1;

            int bottomLeftVert = (y * size) + x;
            int bottomRightVert = (y * size) + (x + 1);
            int topLeftVert = ((y+1) * size) + x;
            int topRightVert = ((y + 1) * size) + (x + 1);

            colors[bottomLeftVert] = colors[topLeftVert] = colors[bottomRightVert] = colors[topRightVert] = color;
            

        }

        mesh.colors = colors;
    }

    public void ClearGridColors()
    {
        colors = new Color[vertices.Length];
        for (int i = 0; i < vertices.Length; i++)
            colors[i] = Color.gray;

        mesh.colors = colors;
    }



    /*private void OnDrawGizmos()
    {
        if(vertices == null)
        {
            return;
        }
        Gizmos.color = Color.black;
        for (int i = 0; i < vertices.Length; i++)
        {
            Gizmos.DrawSphere(transform.TransformPoint(vertices[i]), 0.1f);
        }
    }*/

}
