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
    public override Play RunBehavior(MB_Actor self, MB_Actor target)
    {
        Play currentPlay = new Play(self, 0);

        //for right now charge is 2 on the ability list but this should be a dictionary

        List<Tile> pathToTarget = CS_GridUtility.FindPath(target.currentTile, self.currentTile);
        int tileCount = pathToTarget.Count;

        //If next to, want to attack
        if(tileCount <= self.abilities[2].Range)
        {
            currentPlay.score += 2;

            //Should be spear charge
            currentPlay.inputs.AddRange(AttackInput(self, target.currentTile));

            return currentPlay;
        }



        //Targeting from target means we have all (most) valid tiles a charge could be initated from
        List<Tile> targeting = CS_GridUtility.GetTilesFromOrigin(target.currentTile, self.Speed, true);

        Tile closestTile = null;
        foreach (Tile tile in targeting)
        {
            if(tile.entity && tile.entity != self) { continue; }

            if (closestTile == null || (self.currentTile.position - tile.position).magnitude < (self.currentTile.position - closestTile.position).magnitude) { closestTile = tile; }

        }

        List<Tile> pathToClosest = CS_GridUtility.FindPath(closestTile, self.currentTile);
        

        //if targeting reveals that our monster is at most one step away from being in the valid area, charge 
        if (pathToClosest.Count <= 1)
        {
            currentPlay.score += 2;

            pathToTarget = CS_GridUtility.FindPath(target.currentTile, closestTile);

            currentPlay.inputs.AddRange(ChargeInput(self, pathToTarget[^2]));
            currentPlay.inputs.AddRange(AttackInput(self, target.currentTile));
        }
        else
        {
            
            //If closest is outside of range, get as close as you can to the closest chargable tile
            if(pathToClosest.Count > self.Speed)
            {
                currentPlay.inputs.Add(new TileInput(E_TurnState.SelectingMove, pathToClosest[self.Speed-1]));
            }
            else //If closest is within range, move to it and then initate charge from that tile
            {
                currentPlay.score += 1;
                
                pathToTarget = CS_GridUtility.FindPath(target.currentTile, closestTile);

                currentPlay.inputs.Add(new TileInput(E_TurnState.SelectingMove, closestTile));
                currentPlay.inputs.AddRange(ChargeInput(self, pathToTarget[^2]));
                currentPlay.inputs.AddRange(AttackInput(self, target.currentTile));


            }
            

            

            
            
        }

            
        



        return currentPlay;

        
        
    }

    private List<GameInput> ChargeInput(MB_Actor self, Tile destination)
    {
 
        List<GameInput> chargeInputs = new List<GameInput>() { new AbilityInput(self.abilities[1]), 
            new TileInput(E_TurnState.UsingAbility, self.currentTile), 
            new TileInput(E_TurnState.ResolvingAbility, destination) };

        return chargeInputs;

    }

    private List<GameInput> AttackInput(MB_Actor self, Tile target)
    {
        List<GameInput> attackInputs = new List<GameInput>() { new AbilityInput(self.abilities[2]),
            new TileInput(E_TurnState.UsingAbility, target)};


        return attackInputs;

    }

}
