using System.Drawing;
using UnityEngine;
using UnityEngine.InputSystem;

//Deal with user interaction that effects the Grid Data
public class MB_GridSystem : MonoBehaviour
{
    [SerializeField] SO_GridSystem data;

    InputAction selectAction;
    private void Awake()
    {
        selectAction = InputSystem.actions.FindAction("Select");
        selectAction.performed += CellSelect;
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
        Vector2Int correctedCords = new Vector2Int((int)unityGridCoords.x + data.size / 2, (int)unityGridCoords.z + data.size / 2);
        return correctedCords;
    }

}
