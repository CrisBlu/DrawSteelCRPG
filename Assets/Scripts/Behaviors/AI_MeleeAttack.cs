using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AI_MeleeAttack", menuName = "Scriptable Objects/Behaviors/MeleeAttack")]
public class AI_MeleeAttack : SO_AI
{

    public override List<GameInput> RunBehavior(TurnData turn, MB_Actor target)
    {

        if (turn.actions[E_ActionType.main] <= 0) { return null; }

        MB_Actor self = turn.actor;
        List<Tile> pathToTarget = CS_GridUtility.FindShortestPath(target.currentTile, self.currentTile);
        int tileCount = pathToTarget.Count;

        //If next to, attack
        if (tileCount <= self.abilities[self.sheet.preferredAttack].Range)
        {
            return AttackInput(self, target.currentTile);
        }

        return null;
    }

    private List<GameInput> AttackInput(MB_Actor self, Tile target)
    {
        List<GameInput> attackInputs = new List<GameInput>() { new AbilityInput(self.abilities[self.sheet.preferredAttack]),
            new TileInput(E_TurnState.UsingAbility, target)};


        return attackInputs;

    }
}
