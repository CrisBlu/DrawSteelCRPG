using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

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


        Debug.Log("Parry!");

        return new CS_AbilityReturnData(true);
    }

    private async Task Trigger(int damage, MB_Actor target)
    {
        //Disregard is damaged target is not ally
        if (!target.CompareTag(user.tag))
            return;

        List<Tile> PathToFriend = CS_GridUtility.FindShortestPath(target.currentTile, user.currentTile);

        //If target out of range disregard
        if (PathToFriend.Count > Range) { return; }

        if(PathToFriend.Count > 1)
        {
            await Movement.ActorMovement(user, PathToFriend[PathToFriend.Count - 2]);
        }

        UserService userService = new UserService();
        // Begin waiting for the user's confirmation.
       /* Task<bool> confirmationTask = userService.WaitForUserConfirmation();

        // This line will await the user's confirmation.
        bool confirmed = await confirmationTask;

        // Now you can use the user's confirmation.
        if (confirmed)
        {
            Console.WriteLine("User confirmed!");
        }
        else
        {
            Console.WriteLine("User didn't confirm.");
        }*/

        Debug.Log("Parry!");

    }

    public void SetTrigger(SO_BattleEvents events, MB_Actor user)
    {
        this.user = user;
        SO_BattleEvents.EventActorTookDamage += Trigger;

    }
}