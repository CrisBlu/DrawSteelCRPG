using System.Collections.Generic;
using UnityEngine;

public class MB_HighlightTile : MonoBehaviour
{
    private Vector2Int highlightXY;
    public Color HighlightColor;

    private List<Tile> previousTiles = null;

    void Start()
    {
        highlightXY = Vector2Int.one;
    }


    void Update()
    {
        if (GF_PlayerInput.currentTileMouseOver == null)
            return;


        List<Tile> tilesToColor;
        //if(highlightXY == Vector2Int.one)
        //{
            tilesToColor = new List<Tile>() { GF_PlayerInput.currentTileMouseOver };
        //}


        CS_ColorGrid.HighlightCells(tilesToColor, HighlightColor, previousTiles);

        previousTiles = tilesToColor;
    }
}
