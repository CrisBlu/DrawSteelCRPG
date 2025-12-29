using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AI_Charging", menuName = "Scriptable Objects/Behaviors/Charging")]
public class AI_Charging : SO_AI //Scriptable objects because I would like these AI behaviors to be serializable
{

    ///     The charger will either attack an enemy next to him, charge and attack an enemy, or move, then charge and attack an enemy
    ///     Charger: 
    ///     Consider spaces within melee range, if there is a target +1
    ///     Consider targets within charge range, if there is a target +1
    ///     Consider your walk range, if there is a tile you can walk to that will put you in range of a charge to a target +0
    public override List<GameInput> RunBehavior(TurnData turn, MB_Actor target)
    {


        if (turn.actions[E_ActionType.main] <= 0) { return null; }
        MB_Actor self = turn.actor;

        List<Tile> targeting = CS_GridUtility.GetTilesFromOrigin(target.currentTile, self.Speed + 1, true);

        Tile closestTile = null;
        
        //Find closest tile to actor within the charge range, for this all it's really doing is looking that our range contains the tile we are on
        foreach (Tile tile in targeting)
        {
            if (tile.entity && tile.entity != self) { continue; }

            if(tile.entity == self)
            {
                closestTile = tile;
                break;
            }

            if (closestTile == null || (self.currentTile.position - tile.position).magnitude < (self.currentTile.position - closestTile.position).magnitude) { closestTile = tile; }

        }

        List<Tile> pathToClosest = CS_GridUtility.FindPath(closestTile, self.currentTile);

        //if targeting reveals that our monster is in the valid area
        if (pathToClosest.Count <= 0)
        {

            List<Tile> pathToTarget = CS_GridUtility.FindPath(target.currentTile, closestTile);



            return ChargeInput(self, pathToTarget[^2]);
        }

        return null;
        
        
    }



    private List<GameInput> ChargeInput(MB_Actor self, Tile destination)
    {
 
        List<GameInput> chargeInputs = new List<GameInput>() { new AbilityInput(self.abilities[1]), 
            new TileInput(E_TurnState.UsingAbility, self.currentTile), 
            new TileInput(E_TurnState.ResolvingAbility, destination) };

        return chargeInputs;

    }

    

    //What charging wants to do in order
    //attack
    //charge
    //move into charge range
    //move as close as possible
    //move as far as possible

    //The idea is that when called upon to take a turn it'll decide which target it wants to fight
    // and go down the list of acts do whatever is possible based on position and actions left

}
