using System;
using System.Collections.Generic;

using UnityEngine;




public static class CS_GridUtility
{
    public static CS_AoeReturnData GetTilesAndActorsWithin(Tile origin, int distance, bool straightLine = false)
    {
        List<Tile> targetedTiles = new List<Tile>();
        List<MB_Actor> targetedActors = new List<MB_Actor>();
        Queue<Tile> openSet = new Queue<Tile>();
        openSet.Enqueue(origin);
        origin.cost = 0;

        while (openSet.Count > 0)
        {
            Tile currentCell = openSet.Dequeue();

            foreach (Tile neighbor in currentCell.FindNeighbors())
            {
                if (openSet.Contains(neighbor))
                    continue;

                neighbor.cost = currentCell.cost + 1;
                // Validating Function -----------------------------------------------------------------------------------
                if (neighbor.cost <= distance && !targetedTiles.Contains(neighbor))
                {
                    if (straightLine)
                    {
                        if (!CheckForLineOfSight(origin, neighbor)) { continue; }
                    }


                    targetedTiles.Add(neighbor);
                    openSet.Enqueue(neighbor);

                    

                    if (neighbor.entity && neighbor.entity.isActor)
                    {
                        targetedActors.Add((MB_Actor)neighbor.entity);
                    }
                }
                // Validating Function -----------------------------------------------------------------------------------
            }
        }

        return new CS_AoeReturnData(targetedTiles, targetedActors);
    }


    public static List<Tile> GetValidPushArea(Tile pusher, Tile target, int distance)
    {
        List<Tile> validPushLocations = new();
        Queue<Tile> openSet = new Queue<Tile>();


        Vector2Int actorPosition = pusher.position;
        Vector2Int targetPosition = target.position;
        Vector2Int distanceFromActor = AbsVec.f(actorPosition, targetPosition);
        bool xAxis = false;
        bool yAxis = false;

        if(distanceFromActor.x >= 1)
        {
            xAxis = true;
        }
        
        if(distanceFromActor.y >= 1) { 
            yAxis = true;
        }

        openSet.Enqueue(target);
        target.cost = 0;

        while (openSet.Count > 0)
        {
            Tile currentCell = openSet.Dequeue();

            foreach (Tile neighbor in currentCell.FindNeighbors())
            {
                if (openSet.Contains(neighbor))
                    continue;

                neighbor.cost = currentCell.cost + 1;


                // Validating Function -----------------------------------------------------------------------------------
                Vector2Int neighborDistanceFromCurrentCell = AbsVec.f(currentCell.position, neighbor.position);
                Vector2Int currentCellDistanceFromActor = AbsVec.f(currentCell.position, actorPosition);
                Vector2Int neighborDistanceFromActor = AbsVec.f(actorPosition, neighbor.position);

                //The neighbor cell must never not move along the axis it was pushed on
                //unless it's a diagonal push
                if(!xAxis || !yAxis)
                {
                    if (yAxis && (neighborDistanceFromCurrentCell.y == 0))
                    {
                        continue;
                    }

                    if (xAxis && (neighborDistanceFromCurrentCell.x == 0))
                    {
                        continue;
                    }
                }
                

                //The neighbor cell must be further away from the actor than the current cell is
                if (neighborDistanceFromActor.x >= currentCellDistanceFromActor.x && neighborDistanceFromActor.y >= currentCellDistanceFromActor.y)
                {

                    
                    //Check if this is a new push location and check if it's within distance (techincally)
                    if (!validPushLocations.Contains(neighbor) && neighbor.cost <= distance)
                    {
                        validPushLocations.Add(neighbor);
                        openSet.Enqueue(neighbor);
                    }
                }

                // Validating Function -----------------------------------------------------------------------------------
            }
        }

        return validPushLocations;


    }

    public static List<Tile> GetMovementArea(Tile origin, int distance, bool straightLine)
    {
        Queue<Tile> openSet = new Queue<Tile>();
        List<Tile> possibilities = new List<Tile>();
        origin.cost = 0;
        openSet.Enqueue(origin);

        while (openSet.Count > 0)
        {
            Tile currentCell = openSet.Dequeue();

            foreach (Tile neighbor in currentCell.FindNeighbors())
            {
                if (openSet.Contains(neighbor))
                    continue;

                neighbor.cost = currentCell.cost + 1;
               

                // Validating Function -----------------------------------------------------------------------------------
                //If Cell is full or If We've already decided Cell is a walkable tile or if cell is too far
                if (neighbor.entity != null || possibilities.Contains(neighbor) || neighbor.cost > distance) { continue; }

                neighbor.parent = currentCell;

                if (straightLine)
                {
                    if(!CheckForLineOfSight(origin, neighbor)) { continue; }
                }

                // Validating Function --------------------------------------------------------------------------------------


                openSet.Enqueue(neighbor);
                possibilities.Add(neighbor);

            }
        }

        return possibilities;
    }

    public static List<Tile> GridMakePath(Tile cellToMoveTo, Tile origin)
    {


        List<Tile> stepsToTake = new List<Tile>();

        Tile current = cellToMoveTo;

        while (current != origin)
        {
            stepsToTake.Add(current);
            if (current.parent != null)
            {
                current = current.parent;
            }
            else
                break;
        }

        stepsToTake.Reverse();

        return stepsToTake;

    }


    static bool CheckForLineOfSight(Tile origin, Tile current)
    {
        Vector2Int closerTile = current.position;
        while (closerTile != origin.position)
        {
            if (closerTile.x != origin.position.x)
            {
                closerTile.x = closerTile.x > origin.position.x ? closerTile.x - 1 : closerTile.x + 1;
            }

            if (closerTile.y != origin.position.y)
            {
                closerTile.y = closerTile.y > origin.position.y ? closerTile.y - 1 : closerTile.y + 1;
            }

            if (origin.parentGrid.GridMatrix[closerTile.x, closerTile.y].entity && closerTile != origin.position)
            {
                return false;
            }

        }

        return true;
    }

    public static bool CheckForFlanking(MB_Actor attacker, Tile target)
    {
        //Take the distance between attacker and target, move one more in that direction, check if there's an actor Ally there
        Vector2Int directionFromAttackerToTarget = target.position - attacker.currentTile.position;
        Vector2Int adjacentPosition = target.position + directionFromAttackerToTarget;
        Tile adjacentTile = target.parentGrid.GridMatrix[adjacentPosition.x, adjacentPosition.y];

        if(adjacentTile.entity && attacker.CompareTag(adjacentTile.entity.tag))
        {
            return true;
        }
        else
        {
            return false;
        }


    }

}








public class CS_AoeReturnData
{
    public List<Tile> affectedArea;
    public List<MB_Actor> affectedActors;

    public CS_AoeReturnData(List<Tile> affectedArea, List<MB_Actor> affectedActors)
    {
        this.affectedArea = affectedArea;
        this.affectedActors = affectedActors;
    }
}

public static class AbsVec
{
    public static Vector2Int f(Vector2Int vecOne, Vector2Int vecTwo)
    {
        return new Vector2Int(Math.Abs(vecOne.x - vecTwo.x), Math.Abs(vecOne.y - vecTwo.y));
    }
}