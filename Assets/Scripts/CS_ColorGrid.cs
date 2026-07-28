using System.Collections.Generic;
using UnityEngine;

public static class CS_ColorGrid
{
    public static void ColorCells(List<Tile> cellsToColor, Color color, bool resetColors = true)
    {
        if (cellsToColor.Count == 0)
        {
            return;
        }

        //Get grid tile is apart of and clear any color
        SO_GridData grid = cellsToColor[0].parentGrid;
        int size = grid.size;


        Color[] colors;
        if (resetColors) 
            colors = ClearGridColors(grid);
        else
            colors = grid.mesh.colors;


        foreach (Tile cell in cellsToColor)
        {
            Vector2Int cellPosition = cell.position;

            int x = cellPosition.x;
            int y = cellPosition.y;
            int cellIndex = ((y * size) + x) * 4;

            int bottomLeftVert = cellIndex;
            int bottomRightVert = cellIndex + 1;
            int topLeftVert = cellIndex + 2;
            int topRightVert = cellIndex + 3;

            colors[bottomLeftVert] = colors[topLeftVert] = colors[bottomRightVert] = colors[topRightVert] = color;
            cell.color = color;
        }

        grid.mesh.colors = colors;
    }

    public static Color[] ClearGridColors(SO_GridData grid)
    {
        int size = grid.size;

        Color[] colors = new Color[(size) * (size) * 4];
        for (int i = 0; i < colors.Length; i++)
            colors[i] = Color.clear;

       for(int i = 0; i < grid.size; i++)
       {
           for (int j = 0; j < grid.size; j++)
           {
               grid.GetTile(new Vector2Int(i, j)).color = Color.clear;
           }
       }

        grid.mesh.colors = colors;
        return colors;
    }



    public static void HighlightCells(List<Tile> cellsToColor, Color color, List<Tile> cellsToUncolor)
    {
        if (cellsToColor.Count == 0)
        {
            return;
        }

        SO_GridData grid = cellsToColor[0].parentGrid;
        int size = grid.size;


        Color[] colors = grid.mesh.colors;


        //Return unhighlighted cells to their real color
        if (cellsToUncolor != null)
        {
            foreach (Tile cell in cellsToUncolor)
            {
                Vector2Int cellPosition = cell.position;

                int x = cellPosition.x;
                int y = cellPosition.y;
                int cellIndex = ((y * size) + x) * 4;

                int bottomLeftVert = cellIndex;
                int bottomRightVert = cellIndex + 1;
                int topLeftVert = cellIndex + 2;
                int topRightVert = cellIndex + 3;

                colors[bottomLeftVert] = colors[topLeftVert] = colors[bottomRightVert] = colors[topRightVert] = cell.color;
            }
        }


        foreach (Tile cell in cellsToColor)
        {
            Vector2Int cellPosition = cell.position;

            int x = cellPosition.x;
            int y = cellPosition.y;
            int cellIndex = ((y * size) + x) * 4;

            int bottomLeftVert = cellIndex;
            int bottomRightVert = cellIndex + 1;
            int topLeftVert = cellIndex + 2;
            int topRightVert = cellIndex + 3;

            colors[bottomLeftVert] = colors[topLeftVert] = colors[bottomRightVert] = colors[topRightVert] = color;
        }

        


        grid.mesh.colors = colors;
    }

}
