
using System.Collections.Generic;
using UnityEngine;

public class Tile
{
    public Tile parent;
    public MB_Entity entity = null;
    public Vector2Int position;
    public float costFromOrigin = 0;
    public float costToDestination = 0;
    public int terrainCost = 0;
    public float TotalCost { get { return costFromOrigin + costToDestination + terrainCost; } }

    public SO_GridData parentGrid;


    public List<Tile> FindNeighbors()
    {
        List<Tile> neighbors = new List<Tile>();
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0)
                    continue;

                //If outside the grid
                Vector2Int posToCheck = new Vector2Int(position.x - i, position.y - j);
                if (posToCheck.x < 0 || posToCheck.y < 0)
                    continue;

                //If outside the grid, need to change with grid size
                if (posToCheck.x >= parentGrid.size || posToCheck.y >= parentGrid.size)
                    continue;


                Tile neighbor = parentGrid.GridMatrix[position.x - i, position.y - j];

                



                //Check put diagonals first in the list (we reverse the list later)
                //If there is a cardinal straight line path, prioitize that
                if (Mathf.Abs(i) == Mathf.Abs(j))
                {
                    neighbors.Insert(0, neighbor);
                    continue;
                }

                neighbors.Add(neighbor);
            }

        }
        neighbors.Reverse();
        return neighbors;
    }
}
