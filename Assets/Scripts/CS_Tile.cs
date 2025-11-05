
using System.Collections.Generic;
using UnityEngine;

public class Tile
{
    public Tile parent;
    public MB_Entity entity = null;
    public Vector2Int position;
    public int cost;


    public List<Tile> FindNeighbors(SO_GridSystem gridSystem)
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
                if (posToCheck.x >= gridSystem.size || posToCheck.y >= gridSystem.size)
                    continue;


                Tile neighbor = gridSystem.GridMatrix[position.x - i, position.y - j];

                



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
