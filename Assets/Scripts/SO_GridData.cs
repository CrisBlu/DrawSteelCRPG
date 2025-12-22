using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "SO_GridData", menuName = "Scriptable Objects/GridData")]
//Meant for storing information about what is on the battle grid and validating against conflicts

public class SO_GridData : ScriptableObject
{
    public readonly int size = 100;
    public Tile[,] GridMatrix;

    //Suspect but arguably grid data should probably contain a reference to it's map
    [HideInInspector] public Mesh mesh;

    private void OnEnable()
    {
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


    public Tile GetTile(Vector2Int pos)
    {
        if(pos.x >= size || pos.y >= size) { return null; }

        return GridMatrix[pos.x, pos.y];
    }

    public bool AddToGrid(Tile tile, MB_Entity entity)
    {
        if (!tile.entity) 
        {
            tile.entity = entity;
            return true;
        }
        else { return false; }
    }

    public bool AddToGridByVector(Vector2Int tilePos, MB_Entity entity)
    {
        Tile tile = GetTile(tilePos);
        if (!tile.entity)
        {
            tile.entity = entity;
            return true;
        }
        else { return false; }
    }
}
