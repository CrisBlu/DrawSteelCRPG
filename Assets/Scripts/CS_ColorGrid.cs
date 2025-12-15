using System.Collections.Generic;
using UnityEngine;

public static class CS_ColorGrid
{
    public static void ColorCells(List<Tile> cellsToColor, Color color)
    {
        if (cellsToColor.Count == 0)
        {
            return;
        }

        //Get grid tile is apart of and clear any color
        SO_GridData grid = cellsToColor[0].parentGrid;
        int size = grid.size + 1;
        Color[] colors = ClearGridColors(grid);


        foreach (Tile cell in cellsToColor)
        {
            Vector2Int cellPosition = cell.position;

            int x = cellPosition.x;
            int y = cellPosition.y;

            int bottomLeftVert = (y * size) + x;
            int bottomRightVert = (y * size) + (x + 1);
            int topLeftVert = ((y + 1) * size) + x;
            int topRightVert = ((y + 1) * size) + (x + 1);

            colors[bottomLeftVert] = colors[topLeftVert] = colors[bottomRightVert] = colors[topRightVert] = color;
        }

        grid.mesh.colors = colors;
    }

    public static Color[] ClearGridColors(SO_GridData grid)
    {
        int size = grid.size + 1;

        Color[] colors = new Color[(size) * (size)];
        for (int i = 0; i < colors.Length; i++)
            colors[i] = Color.gray;

        grid.mesh.colors = colors;
        return colors;
    }
}
