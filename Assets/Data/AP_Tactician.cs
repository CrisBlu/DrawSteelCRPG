using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AP_Tactician", menuName = "Scriptable Objects/Ability Packs/Classes/Tactician")]
public class AP_Tactician : SO_AbilityPack
{
    public override List<CS_Ability> Abilities => new List<CS_Ability> { new A_StrikeNow(), new A_BattleGrace(), new A_TwoShot() };
}



public class A_BattleGrace : CS_Ability
{
    public override string Name => "Battle Grace";
    public override string Description => "Fient and Spin";
    public override E_ActionType Type => E_ActionType.main;
    public override List<string> Effects => new List<string> { "melee", "signature", "strike" };
    public override int Range => 1;


    public override CS_AbilityReturnData Use(CS_AbilityInputData data)
    {
        CS_Characteristics stats = data.actor.sheet.stats;
        Queue<CS_CallbackData> callbackQueue = new Queue<CS_CallbackData>();
        int favoredStat = stats.Might >= stats.Agility ? stats.Might : stats.Agility;

        int tier = CS_DiceRoller.PowerRoll(favoredStat, data.edges, data.banes);

        switch (tier)
        {
            case 1:
                data.target.entity.TakeDamage(5 + favoredStat);
                break;

            case 2:
                data.target.entity.TakeDamage(8 + favoredStat);
                Dance(data.actor, data.target);
                break;

            case 3 or 4:
                data.target.entity.TakeDamage(11 + favoredStat);
                Dance(data.actor, data.target);
                break;

        }



        return new CS_AbilityReturnData(true);

    }

    void Dance(MB_Actor self, Tile unwillingPartner)
    {
        MB_Actor target = (MB_Actor)unwillingPartner.entity;
        Vector2Int lastPos = new Vector2Int(self.X, self.Y);
        self.X = unwillingPartner.position.x;
        self.Y = unwillingPartner.position.y;
        self.UpdatePosition();

        target.X = lastPos.x;
        target.Y = lastPos.y;
        target.UpdatePosition();

    }

}


public class A_TwoShot : CS_Ability
{
    public override string Name => "Rapid-Fire";
    public override string Description => "Spam A";
    public override E_ActionType Type => E_ActionType.main;
    public override List<string> Effects => new List<string> { "ranged", "signature" };
    public override int Range => 12;

    int damage;
    public override CS_AbilityReturnData Use(CS_AbilityInputData data)
    {
        CS_Characteristics stats = data.actor.sheet.stats;
        Queue<CS_CallbackData> callbackQueue = new Queue<CS_CallbackData>();
        int favoredStat = stats.Might >= stats.Agility ? stats.Might : stats.Agility;

        int tier = CS_DiceRoller.PowerRoll(favoredStat, data.edges, data.banes);

        switch (tier)
        {
            case 1:
                data.target.entity.TakeDamage(4);
                damage = 4;
                break;

            case 2:
                data.target.entity.TakeDamage(6);
                damage = 6;
                break;

            case 3 or 4:
                data.target.entity.TakeDamage(8);
                damage = 8;
                break;

        }
        List<Tile> validTiles = CS_GridUtility.GetTilesAndActorsWithin(data.actor.currentTile, Range, true).affectedArea;
        callbackQueue.Enqueue(new CS_CallbackData(SecondShot, data.actor, validTiles));

        return new CS_AbilityReturnData(true, callbackQueue);
    }

    void SecondShot(MB_Actor attacker, Tile target)
    {
        if(target.entity)
        {
            target.entity.TakeDamage(damage);
        }
        
    }
}