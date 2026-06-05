using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

[CreateAssetMenu(fileName = "AP_Tactician", menuName = "Scriptable Objects/AbilityPacks/Classes/Tactician")]
public class AP_Tactician : SO_AbilityPack
{
    public override List<CS_Ability> Abilities => new List<CS_Ability> { new A_Parry() };

}


public class A_Parry : CS_Ability, ITrigger
{
    public override string Name => "Parry";
    public override string Description => "You lost the moment you entered these woods";
    public override E_ActionType Type => E_ActionType.trigger;
    public override List<string> Tags => new List<string> { "weapon", "melee"};
    public override int Range => 3;

    MB_Actor user;


    public override async Task<CS_AbilityReturnData> Use(TurnData data)
    {

        //List<Tile> PathToFriend = CS_GridUtility.FindShortestPath(target.currentTile, user.currentTile);

     


        return new CS_AbilityReturnData(true);
    }

    private async Task Trigger(int damage, MB_Actor target)
    {
        if (user.trigger == false)
            return;

        //Disregard is damaged target is not ally
        if (!target.CompareTag(user.tag))
            return;

        List<Tile> PathToFriend = CS_GridUtility.FindShortestPath(target.currentTile, user.currentTile);

        //If target out of range disregard
        if (PathToFriend.Count > Range) { return; }




        UserService userService = new UserService();
        // Begin waiting for the user's confirmation.

        SO_BattleEvents.triggers.Enqueue(userService);

        Task<bool> confirmationTask = userService.WaitForUserConfirmation();

        // This line will await the user's confirmation.
        bool confirmed = await confirmationTask;

        // Now you can use the user's confirmation.
        if (confirmed)
        {
            if (PathToFriend.Count > 1)
            {
                await Movement.ActorMovement(user, PathToFriend[PathToFriend.Count - 2]);
                target.Heal(damage / 2);
                user.trigger = false;
            }

        }


    }

    public void SetTrigger(SO_BattleEvents events, MB_Actor user)
    {
        this.user = user;
        SO_BattleEvents.EventActorTookDamage += Trigger;

    }
}