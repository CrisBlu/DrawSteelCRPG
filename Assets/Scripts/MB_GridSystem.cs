using NUnit.Framework;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

using UnityEngine.Events;

public class MBGridSystem : MonoBehaviour
{

    private MBEntity[,] GridMatrix = new MBEntity[10,10];
    [SerializeField] private MBWorldSelect WorldSelect;
    [SerializeField] private Grid MapGrid;
    [SerializeField] private GameObject cellIndicator;
    [SerializeField] private SO_BattleManager BattleManager;


    InputAction selectAction;
    



    void Start()
    {
        selectAction = InputSystem.actions.FindAction("Select");
        selectAction.performed += GridSelect;

        
    }

    //Eh
    //This highlights every cell you mouse over
    Vector3Int gridPosition;
    void Update()
    {
        Vector3 mousePosition = WorldSelect.GetSelectedMapPosition();
        gridPosition = MapGrid.WorldToCell(mousePosition);

        cellIndicator.transform.position = MapGrid.CellToWorld(gridPosition);
        
    }

    //Add Entity into Grid
    public void AddToGrid(MBEntity newEntity)
    {
         
        GridMatrix[newEntity.X, newEntity.Y] = newEntity;
    }

    public void UpdateOnGrid(MBEntity movingEntity, Vector2Int lastPos)
    {
        //Need to check if this actually works
        GridMatrix[lastPos.x, lastPos.y] = null;
        GridMatrix[movingEntity.X, movingEntity.Y] = movingEntity;
    }

    //This is a move action and happens when you click on a cell
    private void GridSelect(InputAction.CallbackContext context)
    {
        Vector2Int cords = CoordTranslate(gridPosition);
        MBEntity entityInSpace = GridMatrix[cords.x, cords.y];
        //Something is in that space
        if (entityInSpace != null)
        {

            if (entityInSpace.GetType() == typeof(MBActor))
            {
                //Activate an Actor
                BattleManager.activePlayer = (MBActor)entityInSpace;
            }

            return;
        }

        
        if(BattleManager.activePlayer == null)
        {
            return;
        }

        if (BattleManager.moveAction == true)
        {
            BattleManager.OnMoveUsed(cords);
        }
        
        
    }

    private Vector2Int CoordTranslate(Vector3 unityGridCords)
    {
        Vector2Int correctedCords = new Vector2Int((int)unityGridCords.x + 5, (int)unityGridCords.z + 5);
        return correctedCords;
    }
}
