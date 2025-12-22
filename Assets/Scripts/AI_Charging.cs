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
        if(tileCount == self.abilities[2].Range)
        {
            currentPlay.score += 1;

            //Should be spear charge
            currentPlay.inputs.AddRange(AttackInput(self, target.currentTile));
        }
        else if(tileCount <= self.Speed + self.abilities[2].Range)
        {

            currentPlay.score += 1;

            currentPlay.inputs.AddRange(ChargeInput(self, pathToTarget[^2]));
            currentPlay.inputs.AddRange(AttackInput(self, target.currentTile));
        }
        /*else if(tileCount <= 2 * self.Speed + self.abilities[3].Range)
        {
            //Walk into range, then charge


        }*/


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
