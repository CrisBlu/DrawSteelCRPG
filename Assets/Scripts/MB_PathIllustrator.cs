
using System.Collections.Generic;

using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class MB_PathIllustrator : MonoBehaviour
{
    private const float LineHeightOffset = 0.33f;
    LineRenderer line;

    private void Start()
    {
        line = GetComponent<LineRenderer>();
    }

    public void IllustratePath(List<Tile> tiles)
    {
        line.positionCount = tiles.Count;

        for (int i = 0; i < tiles.Count; i++)
        {
            Vector3 linePos = new Vector3(tiles[i].position.x, LineHeightOffset, tiles[i].position.y);
            line.SetPosition(i, linePos);
        }
    }

}