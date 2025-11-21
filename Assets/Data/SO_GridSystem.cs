using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

[CreateAssetMenu(fileName = "SO_GridSystem", menuName = "Scriptable Objects/SO_GridSystem")]
public class SO_GridSystem : ScriptableObject
{
    [SerializeField] SO_BattleManager BattleManager;
    [HideInInspector] public List<Tile> possibleSteps = new List<Tile>();
    public readonly int size = 10;
    public Tile[,] GridMatrix;

    [HideInInspector] public UnityEvent<List<Tile>, Color> EventGridUpdate;


    private void OnEnable()
    {
        EventGridUpdate = new UnityEvent<List<Tile>, Color>();

        BattleManager.EventSelectStateMove.AddListener(GridUpdateBFS);
        BattleManager.EventSelectStateTarget.AddListener(GridUpdateBFSForAttack);
        BattleManager.EventSelectStateCell.AddListener(GridUpdateForCellSelect);

        GridMatrix = new Tile[size, size];

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                GridMatrix[i, j] = new Tile();
                GridMatrix[i, j].parentGrid = this;
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
        else if(BattleManager.selectState == E_SelectState.LookingForAction)
        {
            GridInspect(selectedCell);
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
                GridUpdateBFS(BattleManager.activeActor.Speed);
            }
            return;
        }
        
    }

    public void GridInspect(Tile cell)
    {
        if(cell == BattleManager.activeActor.currentTile)
        {
            BattleManager.SelectActiveActor();
        }
    }

    //
    public void GridMoveTo(Tile cell)
    {
        if (cell == BattleManager.activeActor.currentTile)
        {
            BattleManager.ReturnToDefaultState();
            return;
        }
        //GridUpdateBFS(BattleManager.activeActor.movement);
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
        List<Tile> possibleTargets = GridBFSForAttack(BattleManager.activeActor.currentTile, BattleManager.activeAbility.Range);
        
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
        if(BattleManager.AbilityParser.validTiles.Contains(cell))
        {
            BattleManager.AbilityParser.selectedCell = cell;
        }
        
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

    public List<Tile> GridBFSForAttack(Tile cell, int range, bool forMove = false)
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
                if (neighbor.entity == null)
                {
                    openSet.Enqueue(neighbor);
                }

                //Iterate to origin cell until finds origin cell or other target; this does mean actors can block shots, which is unwanted
                //While currentx and currenty don't equal origin x and origin y
                //  if current x doesn't equal, move one closer
                //  if current y doesn't equal, move one closer
                //  check closer tile, if entity != null break and do not add to possibilties
                //possibilities.Add(current)

                if(CheckForLineOfSight(cell, neighbor))
                {
                    possibilities.Add(neighbor);
                }
                
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

    bool CheckForLineOfSight(Tile origin, Tile current)
    {
        Vector2Int closerTile = current.position;
        while(closerTile != origin.position)
        { 
            if(closerTile.x != origin.position.x)
            {
                closerTile.x = closerTile.x > origin.position.x ? closerTile.x - 1 : closerTile.x + 1;
            }

            if (closerTile.y != origin.position.y)
            {
                closerTile.y = closerTile.y > origin.position.y ? closerTile.y - 1 : closerTile.y + 1;
            }

            if (GridMatrix[closerTile.x, closerTile.y].entity != null && closerTile != origin.position)
            {
                return false;
            }

        }

        return true;
    }


    public void GridUpdateBFS(int distance)
    {
        //This doesn't work properly
        possibleSteps = CS_GridUtility.GetMovementArea(BattleManager.activeActor.currentTile, distance, false);/*GridBFSFromCell(BattleManager.activeActor.currentTile, distance, true);*/
        EventGridUpdate.Invoke(possibleSteps, Color.green);
    }

    public void GridUpdateBFSForAttack(CS_Ability ability)
    {
        List<Tile> possibleTargets = GridBFSForAttack(BattleManager.activeActor.currentTile, ability.Range);
        EventGridUpdate.Invoke(possibleTargets, Color.red);
    }

    public void GridUpdateForCellSelect()
    {
        EventGridUpdate.Invoke(BattleManager.AbilityParser.validTiles, Color.blue);
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
