using NUnit.Framework;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

using UnityEngine.Events;

public class MBGridSystem : MonoBehaviour
{

    public Tile[,] GridMatrix = new Tile[10,10];
    [SerializeField] private MBWorldSelect WorldSelect;
    [SerializeField] private Grid MapGrid;
    [SerializeField] private GameObject cellIndicator;
    [SerializeField] private SO_BattleManager BattleManager;


    InputAction selectAction;
    



    void Awake()
    {
        selectAction = InputSystem.actions.FindAction("Select");
        selectAction.performed += GridSelect;

        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                GridMatrix[i, j] = new Tile();
                GridMatrix[i, j].position = new Vector2Int(i, j);
            }
        }

        
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
         
        GridMatrix[newEntity.X, newEntity.Y].entity = newEntity;
    }

    public void UpdateOnGrid(MBEntity movingEntity, Vector2Int lastPos)
    {
        //Need to check if this actually works
        GridMatrix[lastPos.x, lastPos.y].entity = null;
        GridMatrix[movingEntity.X, movingEntity.Y].entity = movingEntity;
    }

    //This is a move action and happens when you click on a cell
    private void GridSelect(InputAction.CallbackContext context)
    {
        Vector2Int cords = CoordTranslate(gridPosition);
        MBEntity entityInSpace = GridMatrix[cords.x, cords.y].entity;
        //Something is in that space

        //Activate Actor
        if (entityInSpace != null)
        {
            if (entityInSpace.GetType() == typeof(MBActor))
            {
                //Activate an Actor
                BattleManager.activePlayer = (MBActor)entityInSpace;
                return;
            }
           
            
        }

        //If Actor has not activated nothing else can happen
        if (BattleManager.activePlayer == null)
        {
            return;
        }



        if (BattleManager.attackMode)
        {
            //Set Target
            //Doesn't work to confirm target
            if(BattleManager.currentTarget == null)
            {
                BattleManager.currentTarget = entityInSpace;
                return;
            }else
            {
                BattleManager.OnMainUsed(GridMatrix[cords.x, cords.y]);
            }
            
        }
        else
        {
            //Move action
            if (BattleManager.moveAction == true)
            {
                BattleManager.OnMoveUsed(cords);
            }
        }

        
        

        
        
        
    }

    public bool GridCheckIfFull(Vector2Int cords)
    {
        if (GridMatrix[cords.x, cords.y].entity != null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private Vector2Int CoordTranslate(Vector3 unityGridCords)
    {
        Vector2Int correctedCords = new Vector2Int((int)unityGridCords.x + 5, (int)unityGridCords.z + 5);
        return correctedCords;
    }
}
