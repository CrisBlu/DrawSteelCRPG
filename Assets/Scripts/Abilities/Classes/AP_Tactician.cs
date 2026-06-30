using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

[CreateAssetMenu(fileName = "AP_Tactician", menuName = "Scriptable Objects/AbilityPacks/Classes/Tactician")]
public class AP_Tactician : SO_AbilityPack
{
    public override List<CS_Ability> Abilities => new List<CS_Ability> { new A_StrikeNow(), new A_Parry(), new A_BattleGrace(), new A_TwoShot() };

}

public class A_StrikeNow : CS_Ability
{
    public override string Name => "Strike Now";
    public override string Description => "Your foe left an opening";
    public override E_ActionType Type => E_ActionType.main;
    public override List<string> Tags => new List<string> { "ranged" };
    public override int Range => 10;

    public override async Task<CS_AbilityReturnData> Use(TurnData data)
    {

        MB_Actor TargetActor = (MB_Actor)data.target.entity;

        SO_TurnManager.Instance.CreateAndStoreTurn(TargetActor, 1, 0, 0, "signature", E_TurnState.SelectingAbility);

        return new CS_AbilityReturnData(true);
    }

    public override CS_AbilityTargetingData Target(Tile origin)
    {
        return CS_GridUtility.GetFriendsWithin(origin, Range, origin.entity.tag, true);
    }
}

public class A_BattleGrace: CS_Ability
{
    public override string Name => "Battle Grace";
    public override string Description => "Fient and Spin";
    public override E_ActionType Type => E_ActionType.main;
    public override List<string> Tags => new List<string> { "melee", "signature", "strike" };
    public override int Range => 1;

    public override async Task<CS_AbilityReturnData> Use(TurnData data)
    {

        CS_Characteristics stats = data.actor.sheet.stats;
        Queue<CS_CallbackData> callbackQueue = new Queue<CS_CallbackData>();
        int favoredStat = stats.Might >= stats.Agility ? stats.Might : stats.Agility;

        int tier = CS_DiceRoller.PowerRoll(favoredStat, data.edges, data.banes);

        int damage = 0;

        MB_Actor targetActor = (MB_Actor)data.target.entity;

        switch (tier)
        {
            case 1:
                damage = 5 + favoredStat;
                break;

            case 2:
                damage = 8 + favoredStat;
                Dance(data.actor, targetActor);
                break;

            case 3 or 4:
                damage = 11 + favoredStat;
                Dance(data.actor, targetActor);
                break;

        }

        await targetActor.TakeDamage(damage);


        return new CS_AbilityReturnData(true);
    }

    public override CS_AbilityTargetingData Target(Tile origin)
    {

        return CS_GridUtility.GetTilesAndAllWithin(origin, Range, true);
    }

    async void Dance(MB_Actor self, MB_Actor unwillingPartner)
    {

        await Movement.ActorSwapPlaces(self, unwillingPartner);

    }
}




public class A_TwoShot: CS_Ability
{
    public override string Name => "Two Shot";
    public override string Description => "Fire two arrows back to back";
    public override E_ActionType Type => E_ActionType.main;
    public override List<string> Tags => new List<string> { "ranged", "signature" };
    public override int Range => 12;
    public override int NumberOfTargets => 2;


    public async override Task<CS_AbilityReturnData> Use(TurnData data)
    {

        CS_Characteristics stats = data.actor.sheet.stats;
        int favoredStat = stats.Might >= stats.Agility ? stats.Might : stats.Agility;

        int tier = CS_DiceRoller.PowerRoll(favoredStat, data.edges, data.banes);

        int damage = 0;


        switch (tier)
        {
            case 1:
                damage = 4;
                break;

            case 2:
                damage = 6;
                break;

            case 3 or 4:
                damage = 8;
                break;

        }

        foreach(Tile target in targets)
        {
            await target.entity.TakeDamage(damage);
        }
        


        return new CS_AbilityReturnData(true);
    }

}


public class A_Parry : CS_Ability, ITrigger
{
    public override string Name => "Parry";
    public override string Description => "You lost the moment you entered these woods";
    public override E_ActionType Type => E_ActionType.trigger;
    public override List<string> Tags => new List<string> { "weapon", "melee"};
    public override int Range => 2;

    MB_Actor user;


    public override async Task<CS_AbilityReturnData> Use(TurnData data)
    {

        //List<Tile> PathToFriend = CS_GridUtility.FindShortestPath(target.currentTile, user.currentTile);

     


        return new CS_AbilityReturnData(true);
    }

    private async void Trigger(int damage, MB_Actor target)
    {
        if (user.trigger == false)
            return;

        //Disregard is damaged target is not ally
        if (!target.CompareTag(user.tag))
            return;

        List<Tile> PathToFriend = CS_GridUtility.FindShortestPath(target.currentTile, user.currentTile);

        //If target out of range disregard
        if (PathToFriend.Count > Range) { return; }




        AwaitTrigger userService = new AwaitTrigger(this, user);
        // Begin waiting for the user's confirmation.
        

        SO_BattleEvents.AddToTriggerList(userService);

        Task<bool> confirmationTask = userService.WaitForUserConfirmation();



        // This line will await the user's confirmation.
        bool confirmed = await confirmationTask;

        // Now you can use the user's confirmation.
        if (confirmed)
        {

            if (PathToFriend.Count > 1)
            {
                PathToFriend.RemoveAt(PathToFriend.Count - 1);
                await Movement.ActorMovement(user, PathToFriend);
            }

            target.Heal(damage / 2);
            user.trigger = false;

        }

        SO_BattleEvents.RemoveFromTriggerList(userService);


    }

    public override CS_AbilityTargetingData Target(Tile origin)
    {
 
        return new CS_AbilityTargetingData(CS_GridUtility.GetTilesFromOrigin(origin, Range, true), new List<Tile>() { origin });
    }

    public void SetTrigger(SO_BattleEvents events, MB_Actor user)
    {
        this.user = user;
        SO_BattleEvents.EventActorTookDamage += Trigger;

    }
}