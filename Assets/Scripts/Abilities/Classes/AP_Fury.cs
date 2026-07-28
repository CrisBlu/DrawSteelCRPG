using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using static A_BrutalSlam;

[CreateAssetMenu(fileName = "AP_Fury", menuName = "Scriptable Objects/AbilityPacks/Classes/Fury")]
public class AP_Fury : SO_AbilityPack
{
    public override List<CS_Ability> Abilities => new List<CS_Ability> { new A_BrutalSlam(), new A_DevastatingRush() };
}





public class A_BrutalSlam : CS_Ability
{
    public override string Name => "Brutal Slam";
    public override string Description => "Drive them back!";
    public override E_ActionType Type => E_ActionType.main;
    public override List<string> Tags => new List<string> { "melee", "signature", "strike" };
    public override int Range => 1;

    
    public async override Task<CS_AbilityReturnData> Use(TurnData data)
    {

        CS_Characteristics stats = Owner.sheet.stats;
        MB_Actor targetActor = (MB_Actor)targets[0].entity;

        int favoredStat = stats.Might;
        
        int tier = CS_DiceRoller.PowerRoll(favoredStat, data.edges, data.banes);

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

        await targetActor.TakeDamage(damage);


        //This chunk of code can maybe be a "push request"

        List<Tile> validPushLocations = CS_GridUtility.GetValidPushArea(Owner.currentTile, targets[0], distance);

        CS_ColorGrid.ColorCells(validPushLocations, Color.blue);
        AwaitTile tileRequest = new AwaitTile(validPushLocations);
        MB_PlayerInput.inputRequest = tileRequest;
        Tile tileToPushTarget = await tileRequest.WaitForUserConfirmation();

       
        targetActor.ForcedMovement(tileToPushTarget, distance);
        //


        return new CS_AbilityReturnData(true);
    }


    public class A_DevastatingRush : CS_Ability
    {
        public override string Name => "Devastating Rush";
        public override string Description => "Ready or not!";
        public override E_ActionType Type => E_ActionType.main;
        public override List<string> Tags => new List<string> { "melee", "signature", "strike" };
        public override int Range => 4;


        public async override Task<CS_AbilityReturnData> Use(TurnData data)
        {
            CS_Characteristics stats = Owner.sheet.stats;
            Tile target = targets[0];

            int favoredStat = stats.Might;

            int damage = favoredStat;
            int tier = CS_DiceRoller.PowerRoll(favoredStat, data.edges, data.banes);


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
            await targetActor.TakeDamage(damage);
 

            return new CS_AbilityReturnData(true);

        }
    }

}