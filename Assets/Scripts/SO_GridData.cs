using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "SO_GridData", menuName = "Scriptable Objects/GridData")]
//Meant for keeping track and storing information about what is on the battle grid

public class SO_GridData : ScriptableObject
{
    public readonly int size = 100;
    public Tile[,] GridMatrix;

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

}
