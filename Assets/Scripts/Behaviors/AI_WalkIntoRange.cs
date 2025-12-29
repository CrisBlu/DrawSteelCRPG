using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AI_WalkIntoRange", menuName = "Scriptable Objects/Behaviors/WalkIntoRange")]
public class AI_WalkIntoRange : SO_AI
{
    public override List<GameInput> RunBehavior(TurnData turn, MB_Actor target)
    {
        if (turn.actions[E_ActionType.move] <= 0) { return null; }
        MB_Actor self = turn.actor;

        //This works for charge and not a goddamn thing else
        List<Tile> targeting = CS_GridUtility.GetTilesFromOrigin(target.currentTile, self.Speed + 1, true);

        Tile closestTile = null;

        //Find closest tile to actor within the charge range, for this all it's really doing is looking that our range contains the tile we are on
        foreach (Tile tile in targeting)
        {
            if (tile.entity && tile.entity != self) { continue; }

            if (tile.entity == self)
            {
                //If this procs it means we're already in range and don't want to move anymore unless further behaviors tells us to
                closestTile = tile;
                return null;
            }

            if (closestTile == null || (self.currentTile.position - tile.position).magnitude < (self.currentTile.position - closestTile.position).magnitude) { closestTile = tile; }

        }

        List<Tile> pathToClosest = CS_GridUtility.FindPath(closestTile, self.currentTile);

        //If closest is outside of range, get as close as you can to the closest chargable tile
        return new List<GameInput>() { new TileInput(E_TurnState.SelectingMove, closestTile)};



        
    }
}
