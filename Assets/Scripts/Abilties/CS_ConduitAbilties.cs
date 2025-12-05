using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class CS_ConduitAbilties
{


}


public class A_Lightfall : CS_Ability
{
    public override string Name => "Lightfall";
    public override string Description => "A rain of holy light scours your enemies and repositions your allies.";
    public override E_ActionType Type => E_ActionType.main;
    public override List<string> Effects => new List<string> { "signature" };
    public override int Range => 0;


    public override CS_AbilityReturnData Use(CS_AbilityInputData data)
    {
        Queue<CS_CallbackData> callbackQueue = new();
        List<Tile> targetedTiles;
        List<MB_Actor> targetedActors;
        CS_Characteristics stats = data.actor.sheet.stats;
        int damage = 0;
        int distance = 2;


        int tier = CS_DiceRoller.PowerRoll(stats.Intuition);

        switch (tier)
        {
            case 1:
                damage = 2;
                break;

            case 2:
                damage = 3;
                break;

            case 3 or 4:
                damage = 5;
                break;
        }

        CS_AoeReturnData TilesAndActors = CS_GridUtility.GetTilesAndActorsWithin(data.actor.currentTile, distance);
        targetedActors = TilesAndActors.affectedActors;
        targetedTiles = TilesAndActors.affectedArea;


        foreach(MB_Actor target in targetedActors)
        {
            if(data.actor.CompareTag(target.tag))
            {
                callbackQueue.Enqueue(new CS_CallbackData(TeleportActor, target, targetedTiles));
            }
            else
            {
                target.TakeDamage(damage);
            }
        }

        return new CS_AbilityReturnData(true, callbackQueue);


    }

    private void TeleportActor(MB_Actor target, Tile destination)
    {
        target.Teleport(destination.position);
    }
}
