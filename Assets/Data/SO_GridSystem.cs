using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "SO_GridSystem", menuName = "Scriptable Objects/SO_GridSystem")]
public class SO_GridSystem : ScriptableObject
{
    [SerializeField] SO_BattleManager BattleManager;
    List<Tile> possibleSteps = new List<Tile>();
    public readonly int size = 10;
    public Tile[,] GridMatrix;


    private void OnEnable()
    {

        GridMatrix = new Tile[size, size];

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                GridMatrix[i, j] = new Tile();
                GridMatrix[i, j].position = new Vector2Int(i, j);
            }
        }
    }

    //Add new entity to grid
    public Tile GridAdd(MB_Entity newEntity)
    {

        GridMatrix[newEntity.X, newEntity.Y].entity = newEntity;
        return GridMatrix[newEntity.X, newEntity.Y];
    }

    //Move entity position on grid
    public Tile GridUpdatePos(MB_Entity movingEntity, Vector2Int lastPos)
    {
        //Need to check if this actually works
        GridMatrix[lastPos.x, lastPos.y].entity = null;
        GridMatrix[movingEntity.X, movingEntity.Y].entity = movingEntity;

        return GridMatrix[movingEntity.X, movingEntity.Y];
    }

    public void GridRemove(MB_Entity destroyedEntity)
    {
        destroyedEntity.currentTile.entity = null;
    }

    public void GridCellSelect(Vector2Int selectedPos)
    {
        if(selectedPos.x >= size || selectedPos.y >= size || selectedPos.x < 0 || selectedPos.y < 0)
            { return; }

        Tile selectedCell = GridMatrix[selectedPos.x, selectedPos.y];

        GridOnSelection(selectedCell);
        
    }

    public void GridOnSelection(Tile selectedCell)
    {
        if (BattleManager.selectState == E_SelectState.LookingForTarget)
        {
            GridTarget(selectedCell);
        }
        else if (BattleManager.selectState == E_SelectState.LookingForActor)
        {
            GridActivate(selectedCell);
        }
        else if (BattleManager.selectState == E_SelectState.LookingForMove)
        {
            GridMoveTo(selectedCell);
        }
        else if (BattleManager.selectState == E_SelectState.LookingForCell)
        {
            GridForceMoveTo(selectedCell);
        }
    }

    //--------------------------------------------------------------------------------------------------------------- Feeling like these should be in a different script
    //Activate an Actor
    public void GridActivate(Tile cell)
    {

        MB_Entity entityInSpace = cell.entity;

        if (entityInSpace == null)
        {
            return;
        }

        if (!entityInSpace.CompareTag(BattleManager.activePlayer.role))
        {
            Debug.Log("Not your unit");
            return;
        }


        if (entityInSpace is MB_Actor)
        {
            //Activate an Actor
            if (BattleManager.SetActiveActor((MB_Actor)entityInSpace))
            {
                GridDisplayPossibleSteps(BattleManager.activeActor.Speed);
            }
            return;
        }
        
    }

    //
    public void GridMoveTo(Tile cell)
    {
        GridUpdateBFS(BattleManager.activeActor.movement);
        MB_Entity entityInSpace = cell.entity;
        if(entityInSpace == null)
        {
            if(!possibleSteps.Contains(cell))
            {
                return;
            }
            BattleManager.MoveAction(cell, this);
        }
    }

    public void GridTarget(Tile cell)
    {
        List<Tile> possibleTargets = GridBFSFromCell(BattleManager.activeActor.currentTile, BattleManager.activeAbility.Range, false);
        
        if(!possibleTargets.Contains(cell))
        { return; }
        
        MB_Entity entityInSpace = cell.entity;
        if (entityInSpace != null)
        {
            BattleManager.SetActiveTarget(cell);
        }
    }

    public void GridForceMoveTo(Tile cell)
    {
        BattleManager.activeTarget[1] = cell;
        BattleManager.UseAbility();
    }

    //-----------------------------------------------------------------------------------------------------------------------------------

    public bool GridCheckIfFull(Tile cell)
    {
        if (cell.entity != null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public List<Tile> GridBFSFromCell(Tile cell, int range, bool forMove)
    {
        Queue<Tile> openSet = new Queue<Tile>();
        List<Tile> possibilities = new List<Tile>();
        cell.cost = 0;
        openSet.Enqueue(cell);

        while (openSet.Count > 0)
        {
            Tile currentCell = openSet.Dequeue();

            foreach (Tile neighbor in currentCell.FindNeighbors(this))
            {
                if (openSet.Contains(neighbor))
                    continue;

                neighbor.cost = currentCell.cost + 1;

                if (!CheckIfValidCell(neighbor, range, possibilities, forMove))
                    continue; 


                GridMatrix[neighbor.position.x, neighbor.position.y].parent = currentCell;


                //The idea here is to make it so that when targeting, cells with targets don't become parents of any other cell
                if(neighbor.entity == null)
                {
                    openSet.Enqueue(neighbor);
                }
                
                possibilities.Add(neighbor);
            }
        }

        return possibilities;
    }

    bool CheckIfValidCell(Tile cell, int maxcost, List<Tile> possibilities, bool forMove)
    {
        bool valid = false;

        if (forMove)
        {
            //If moving return false for cells that have entities in them
            if (GridCheckIfFull(cell))
                return valid;
        }


        
        

        if (!possibilities.Contains(cell) && cell.cost <= maxcost)
            valid = true;  

        return valid;
    }

    public void GridDisplayPossibleSteps(int distance)
    {
        possibleSteps = GridBFSFromCell(BattleManager.activeActor.currentTile, distance, true);

        foreach (Tile step in possibleSteps)
        {
            //Debug.Log(step);
        }
    }

    public void GridUpdateBFS(int distance)
    {
        possibleSteps = GridBFSFromCell(BattleManager.activeActor.currentTile, distance, true);

    }




    //Upon clicking a Cell
    //If user is not attacking
    //  If entity is in Cell 
    //      If entity is Actor
    //          Activate Actor
    //
    //If user is attacking
    //  If entity is in Cell and Actor is activated
    //      entity is now target
    //
    //If user is not attacking
    //  If no entity is in Cell and Actor is activated
    //      Actor moves to cell
    //      



}
