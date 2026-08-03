using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "AP_Fury", menuName = "Scriptable Objects/AbilityPacks/Classes/Fury")]
public class AP_Fury : SO_AbilityPack
{
    public override List<CS_Ability> Abilities => new List<CS_Ability> { new A_BrutalSlam(), new A_DevastatingRush(), new A_LinesOfForce() };
}





public class A_BrutalSlam : CS_Ability, ITieredAbility
{
    public override string Name => "Brutal Slam";
    public override string Description => "Drive them back!";
    public override E_ActionType Type => E_ActionType.main;
    public override List<string> Tags => new List<string> { "melee", "signature", "strike" };
    public override int Range => 1;
    public List<E_Stats> BonusStat => new() { E_Stats.M };


    public async override Task<bool> Use(int tier = 0)
    {

        CS_Characteristics stats = Owner.sheet.stats;
        MB_Actor targetActor = (MB_Actor)targets[0].entity;

        int favoredStat = stats.Might;


        int damage = favoredStat;

        int distance = 0;

        switch (tier)
        {
            case 1:
                damage += 3;
                distance = 1;
                break;

            case 2:
                damage += 6;
                distance = 2;
                break;

            case 3 or 4:
                damage += 9;
                distance = 4;
                break;
        }

        SO_BattleEvents.AddRequest(new RequestDamage(targets[0], damage));


        SO_BattleEvents.AddRequest(new RequestForceMove(targets[0], distance, Owner.currentTile));

        return true;
    }

}
public class A_DevastatingRush : CS_Ability, ITieredAbility
{
    public override string Name => "Devastating Rush";
    public override string Description => "Ready or not!";
    public override E_ActionType Type => E_ActionType.main;
    public override List<string> Tags => new List<string> { "melee", "signature", "strike" };
    public override int Range => 4;
    public List<E_Stats> BonusStat => new() { E_Stats.M, E_Stats.A };


    public async override Task<bool> Use(int tier = 0)
    {
        CS_Characteristics stats = Owner.sheet.stats;
        Tile target = targets[0];

        //Might and agility, you find this out in RollAbility, how can we avoid this?
        int favoredStat = stats.Might;

        int damage = favoredStat;


        //Charge could work like this, but if your target is surrounded I need a fail condition

        List<Tile> tilesNextToTarget = target.FindNeighbors();
        Tile closestTile = null;
        foreach (Tile tile in tilesNextToTarget)
        {
            if (tile.entity) { continue; }

            if (closestTile == null || (Owner.currentTile.position - tile.position).magnitude < (Owner.currentTile.position - closestTile.position).magnitude) { closestTile = tile; }

        }

        List<Tile> chargePath = CS_GridUtility.FindShortestPath(closestTile, Owner.currentTile);
        //-----------------------------------------------


        damage += chargePath.Count;

       


        switch (tier)
        {
            case 1:
                damage += 3;
                break;

            case 2:
                damage += 6;
                break;

            case 3 or 4:
                damage += 13;
                break;

        }

        await Movement.ActorMovement(Owner, chargePath);
        MB_Actor targetActor = (MB_Actor)target.entity;
        SO_BattleEvents.AddRequest(new RequestDamage(targets[0], damage));


        return true;

    }
}

public class A_LinesOfForce : CS_Ability, ITrigger
{
    public override string Name => "Lines of Force";
    public override string Description => "No! You move!";
    public override E_ActionType Type => E_ActionType.trigger;
    public override List<string> Tags => new List<string> { "magic", "melee" };
    public override int Range => 1;


    RequestForceMove request;
    public override async Task<bool> Use(int tier = 0)
    {
        //The original action is nulled, every trigger that was associated with it is cancelled
        request.Cancel = true;
        int newDistance = request.distance + Owner.sheet.stats.Might;
        Debug.Log(request.distance + " + " + Owner.sheet.stats.Might);



        //If confirmed, allow the user to select new target within range
        GF_PlayerInput.inputEnabled = true;


        //Then the user will select where they would like to push that user, distance + might score
        /*MB_PlayerInput.Instance.SetSelectState(E_SelectState.UsingAbility);
        MB_PlayerInput.inputRequest = new AwaitTile(CS_GridUtility.GetTilesAndAllWithin(Owner.currentTile, Range).validTargets);*/

        //Build the new request
        RequestForceMove newRequest = new(targets[0], newDistance, Owner.currentTile);


        //The new push happens instead
        SO_BattleEvents.AddRequest(newRequest);


        return true;
    }

    private async void Trigger(RequestForceMove request)
    {
        if (Owner.trigger == false)
            return;


        List<Tile> PathToTarget = CS_GridUtility.FindShortestPath(request.target, Owner.currentTile);

        //If target out of range disregard
        if (PathToTarget.Count > Range) { return; }




        AwaitTrigger userService = new AwaitTrigger(this, Owner);
        // Begin waiting for the user's confirmation.


        SO_BattleEvents.AddToTriggerList(userService);

        Task<bool> confirmationTask = userService.WaitForUserConfirmation();



        // This line will await the user's confirmation.
        bool confirmed = await confirmationTask;

        // Now you can use the user's confirmation.
        if (confirmed)
        {
            await CS_AbilityParser.ReadAbility(this);
            Owner.trigger = false;
            this.request = request;
            Use();

           

        }

        SO_BattleEvents.RemoveFromTriggerList(userService);


    }

    /*public override CS_AbilityTargetingData Target(Tile origin)
    {

        return new CS_AbilityTargetingData(CS_GridUtility.GetTilesFromOrigin(origin, Range, true), new List<Tile>() { origin });
    }*/

    public void SetTrigger(SO_BattleEvents events, MB_Actor user)
    {
        SO_BattleEvents.EventBeforeForcedMoved += Trigger;

    }
}

