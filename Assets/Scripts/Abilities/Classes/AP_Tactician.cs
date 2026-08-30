
using System.Collections.Generic;

using System.Threading.Tasks;
using UnityEngine;


[CreateAssetMenu(fileName = "AP_Tactician", menuName = "Scriptable Objects/AbilityPacks/Classes/Tactician")]
public class AP_Tactician : SO_AbilityPack
{
    public override List<CS_Ability> Abilities => new List<CS_Ability> { new A_StrikeNow(), new A_Mark(), new A_Parry(), new A_BattleGrace(), new A_TwoShot(), new A_InspiringStrike() };

}

public class A_StrikeNow : CS_Ability
{
    public override string Name => "Strike Now";
    public override string Description => "Your foe left an opening";
    public override E_ActionType Type => E_ActionType.main;
    public override List<string> Tags => new List<string> { "ranged" };
    public override int Range => 10;

    public override int[] Cost => new int[2] { 0, 5 };


    private List<E_AbilityInstructions> InstructionsRef = new List<E_AbilityInstructions>() { E_AbilityInstructions.SpendResource };
    public override List<E_AbilityInstructions> Instructions { 
        get 
        {
            return InstructionsRef;
        } 
    }

    

    public override async Task<bool> Use(int tier = 0)
    {

        foreach(Tile target in targets)
        {
            MB_Actor targetActor = (MB_Actor)target.entity;
            SO_TurnManager.Instance.CreateAndStoreTurn(targetActor, 1, 0, 0, "signature");
        }
        

        return true;
    }

    public override void Spend(bool spent)
    {
        if(spent)
            InstructionsRef.AddRange(new List<E_AbilityInstructions>() { E_AbilityInstructions.SelectTarget, E_AbilityInstructions.SelectTarget, E_AbilityInstructions.Confirm });
        else
            InstructionsRef.AddRange(new List<E_AbilityInstructions>() { E_AbilityInstructions.SelectTarget, E_AbilityInstructions.Confirm });

    }

    public override CS_AbilityTargetingData Target(Tile origin)
    {
        return CS_GridUtility.GetFriendsWithin(origin, Range, origin.entity.tag, true);
    }
}

public class A_InspiringStrike : CS_Ability, ITieredAbility
{
    public override string Name => "Inspiring Strike";

    public override string Description => "If they bleed!";

    public override E_ActionType Type => E_ActionType.main;

    public override List<string> Tags => new List<string> { "Melee, Strike, Weapon"};

    public override int Range => 1;

    public List<E_Stats> BonusStat => new List<E_Stats> { E_Stats.M };

    public override async Task<bool> Use(int tier = 0)
    {
        CS_Characteristics stats = Owner.sheet.stats;
        int favoredStat = stats.Get(BonusStat[0]);

        int damage = 0;

        MB_Actor targetActor = (MB_Actor)targets[0].entity;
        List<Tile> validTiles = new();

        switch (tier)
        {
            case 1:
                validTiles = CS_GridUtility.GetFriendsWithin(Owner.currentTile, 10, Owner.tag).validTargets;
                validTiles.Add(Owner.currentTile);
                damage = 3 + favoredStat;
                break;

            case 2:
                validTiles = CS_GridUtility.GetFriendsWithin(Owner.currentTile, 10, Owner.tag).validTargets;
                validTiles.Add(Owner.currentTile);
                damage = 5 + favoredStat;
                break;

            case 3 or 4:
                validTiles = CS_GridUtility.GetFriendsWithin(Owner.currentTile, 10, Owner.tag).validTargets;
                damage = 8 + favoredStat;
                break;

        }

        AwaitTile userInput = new(validTiles);
        CS_ColorGrid.ColorCells(validTiles, Color.blue);
        MB_PlayerInput.inputRequest = userInput;
        Tile allyToSupport = await userInput.WaitForUserConfirmation();

        Debug.Log(allyToSupport.entity.name);
        SO_BattleEvents.AddRequest(new RequestDamage(targetActor.currentTile, damage));


        return true;
    }
}

public class A_BattleGrace: CS_Ability, ITieredAbility
{
    public override string Name => "Battle Grace";
    public override string Description => "Fient and Spin";
    public override E_ActionType Type => E_ActionType.main;
    public override List<string> Tags => new List<string> { "melee", "signature", "strike" };
    public override int Range => 1;
    public List<E_Stats> BonusStat => new() { E_Stats.M, E_Stats.A };


    public override async Task<bool> Use(int tier = 0)
    {

        CS_Characteristics stats = Owner.sheet.stats;
        int favoredStat = stats.Might >= stats.Agility ? stats.Might : stats.Agility;

        int damage = 0;

        MB_Actor targetActor = (MB_Actor)targets[0].entity;

        switch (tier)
        {
            case 1:
                damage = 5 + favoredStat;
                break;

            case 2:
                damage = 8 + favoredStat;
                Dance(Owner, targetActor);
                break;

            case 3 or 4:
                damage = 11 + favoredStat;
                Dance(Owner, targetActor);
                break;

        }

        SO_BattleEvents.AddRequest(new RequestDamage(targetActor.currentTile, damage));


        return true;
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




public class A_TwoShot : CS_Ability, ITieredAbility
{
    public override string Name => "Two Shot";
    public override string Description => "Fire two arrows back to back";
    public override E_ActionType Type => E_ActionType.main;
    public override List<string> Tags => new List<string> { "ranged", "signature" };
    public override int Range => 12;
    public override List<E_AbilityInstructions> Instructions { get { return new List<E_AbilityInstructions>() { E_AbilityInstructions.SelectTarget, E_AbilityInstructions.SelectTarget,
        E_AbilityInstructions.Confirm }; } }

    public List<E_Stats> BonusStat => new() { E_Stats.M, E_Stats.A };



    public async override Task<bool> Use(int tier = 0)
    {


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
            SO_BattleEvents.AddRequest(new RequestDamage(target, damage));
        }
        


        return true;
    }

}

public class A_Mark : CS_Ability, ITrigger
{
    public override string Name => "Mark";
    public override string Description => "Nothing gets by me";
    public override E_ActionType Type => E_ActionType.maneuver;
    public override List<string> Tags => new List<string> { "ranged" };
    public override int Range => 10;

    A_TriggerMark TriggerAbility;
    Status mark = new Status(E_StatusType.marked, E_StatusEnd.Never);



    public async override Task<bool> Use(int tier = 0)
    {

        MB_Actor targetActor = targets[0].entity as MB_Actor;

        if (TriggerAbility.currentlyMarked != null)
        {
            if (targetActor == TriggerAbility.currentlyMarked) { return false; }
            TriggerAbility.currentlyMarked.Condition.Remove(mark);
        }





        foreach (Status status in targetActor.Condition)
        {
            if (status.status == E_StatusType.marked) { return false; }

        }

        targetActor.Condition.Add(mark);
        TriggerAbility.currentlyMarked = targetActor;

        //Temp, this ability makes no requests so... yeah
        MB_PlayerInput.Instance.SetSelectState(E_SelectState.SelectingMove);

        return true;
    }

    public void Trigger(RequestPowerRoll request)
    {

        //Mark gives an edge to allies
        if (!Owner.CompareTag(request.ability.Owner.tag))
            return;

        request.edge++;
    }

    public void SetTrigger(SO_BattleEvents events, MB_Actor user)
    {
        TriggerAbility = new A_TriggerMark(Owner);
        SO_BattleEvents.EventBeforePowerRoll += Trigger;
        SO_BattleEvents.EventBeforeTakeDamage += TriggerAbility.Trigger;
    }



}

public class A_TriggerMark: CS_Ability, ITrigger
{
    public override string Name => name;
    private string name = "Trigger Mark";
    public override string Description => description;
    private string description = "There! Exploit their weakness...";
    public override E_ActionType Type => E_ActionType.trigger;
    public override List<string> Tags => new List<string> {};
    public override int Range => 1;

    public MB_Actor currentlyMarked = null;

    public A_TriggerMark(MB_Actor Owner, string name = "Trigger Mark", string description = "There! Exploit their weakness...")
    {
        this.name = name;
        this.description = description;
        this.Owner = Owner;
    }

    public async override Task<bool> Use(int tier = 0)
    {
        return true;
    }

    public async void Trigger(RequestDamage request)
    {
        MB_Actor targetActor = request.target.entity as MB_Actor;

        //if the target is marked by this tactician
        if (targetActor != currentlyMarked) { return; }

        List<AwaitTrigger> triggers = new List<AwaitTrigger>();


        //Need to ask to spend one focus, maybe this is another ability entirely?

        //Start adding triggers
        TriggerExtraDamage(request, triggers);
        TriggerRecovery(request, triggers);
        TriggerShift(request, triggers);
        TriggerTaunt(request, triggers);



    }

  

    //Biggest thing is that this is a free trigger but you may only pick one of these options,I think the play is one trigger function
    //that makes four triggers, and choosing one removes the rest

    //And also need to check if person damaging is friend and if enemy is marked

    private async void TriggerExtraDamage(RequestDamage request, List<AwaitTrigger> triggerFamily)
    {

        A_TriggerMark thisAbility = new(Owner, "Strike Hard", "...and go for the eyes!");
        AwaitTrigger trigger = new(thisAbility, Owner);

        triggerFamily.Add(trigger);


        SO_BattleEvents.AddToTriggerList(trigger);


        Task<bool> confirmationTask = trigger.WaitForUserConfirmation();


        bool confirmed = await confirmationTask;

        if (confirmed)
        {
            //If one option in the family is selected, the rest are discarded
            foreach (AwaitTrigger option in triggerFamily)
            {
                if (trigger != option)
                {
                    option.OnUserActionCompleted(false);
                }
            }

            request.damage += Owner.sheet.stats.Reason * 2;


        }

        //Remove yourself from trigger list
        SO_BattleEvents.RemoveFromTriggerList(trigger);
    }

    private async void TriggerRecovery(RequestDamage request, List<AwaitTrigger> triggerFamily)
    {
        //Need to implement recovery

        A_TriggerMark thisAbility = new(Owner, "Strike Proud", "...and they cannot win!");

        AwaitTrigger trigger = new(thisAbility, Owner);

        triggerFamily.Add(trigger);


        SO_BattleEvents.AddToTriggerList(trigger);


        Task<bool> confirmationTask = trigger.WaitForUserConfirmation();


        bool confirmed = await confirmationTask;

        if (confirmed)
        {
            //If one option in the family is selected, the rest are discarded
            foreach (AwaitTrigger option in triggerFamily)
            {
                if (trigger != option)
                {
                    option.OnUserActionCompleted(false);
                }
            }

            //Do effect now

        }

        //Remove yourself from trigger list
        SO_BattleEvents.RemoveFromTriggerList(trigger);
    }

    private async void TriggerShift(RequestDamage request, List<AwaitTrigger> triggerFamily)
    {
        //Need to implement shift movement
        A_TriggerMark thisAbility = new(Owner, "Strike Fast", "...to get in and get out!");

        AwaitTrigger trigger = new(thisAbility, Owner);

        triggerFamily.Add(trigger);


        SO_BattleEvents.AddToTriggerList(trigger);


        Task<bool> confirmationTask = trigger.WaitForUserConfirmation();


        bool confirmed = await confirmationTask;

        if (confirmed)
        {
            //If one option in the family is selected, the rest are discarded
            foreach (AwaitTrigger option in triggerFamily)
            {
                if (trigger != option)
                {
                    option.OnUserActionCompleted(false);
                }
            }

            //Do effect now

        }

        //Remove yourself from trigger list
        SO_BattleEvents.RemoveFromTriggerList(trigger);
    }

    private async void TriggerTaunt(RequestDamage request, List<AwaitTrigger> triggerFamily)
    {
        //Need to implement data for attacker in Request
        //Need to implement shift taunt
        A_TriggerMark thisAbility = new (Owner, "Strike Foul", "...and keep their eyes on me.");

        AwaitTrigger trigger = new(thisAbility, Owner);

        triggerFamily.Add(trigger);


        SO_BattleEvents.AddToTriggerList(trigger);


        Task<bool> confirmationTask = trigger.WaitForUserConfirmation();


        bool confirmed = await confirmationTask;

        if (confirmed)
        {
            //If one option in the family is selected, the rest are discarded
            foreach (AwaitTrigger option in triggerFamily)
            {
                if (trigger != option)
                {
                    option.OnUserActionCompleted(false);
                }
            }

            //Do effect now

        }

        //Remove yourself from trigger list
        SO_BattleEvents.RemoveFromTriggerList(trigger);
    }

    public void SetTrigger(SO_BattleEvents events, MB_Actor user)
    {
       
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


    public override async Task<bool> Use(int tier = 0)
    {

        //List<Tile> PathToFriend = CS_GridUtility.FindShortestPath(target.currentTile, user.currentTile);

     


        return true;
    }

    private async void Trigger(RequestDamage request)
    {
        if (user.trigger == false)
            return;

        MB_Actor targetActor = request.target.entity as MB_Actor;

        //Disregard is damaged target is not ally
        if (!targetActor.CompareTag(user.tag))
            return;

        List<Tile> PathToFriend = CS_GridUtility.FindShortestPath(request.target, Owner.currentTile);

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

            request.damage /= 2;
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
        SO_BattleEvents.EventBeforeTakeDamage += Trigger;

    }
}