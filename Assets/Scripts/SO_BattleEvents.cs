using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_BattleEvents", menuName = "Scriptable Objects/SO_BattleEvents")]
public class SO_BattleEvents : ScriptableObject
{

    [HideInInspector] public event Action<Tile, Tile, MB_Actor> EventActorLeftTile;
    [HideInInspector] public static event Action<int, MB_Actor> EventActorTookDamage;

    [HideInInspector] public static event Action EventPotentialTriggersChanged;

    static private UserService HoldOnTriggerList;
    private void OnEnable()
    {
        CS_BattleLog.BattleEvents = this;
    }

    public void TriggerActorLeftTileEvents(Tile exit, Tile entered, MB_Actor actor)
    {
        //Will ask all enemies with triggers that trigger when an enemy enters or leaves a space to check their triggers
        EventActorLeftTile.Invoke(exit, entered, actor);
    }

    public static async Task TriggerActorTookDamageEvents(int damage, MB_Actor actor)
    {
        EventActorTookDamage.Invoke(damage, actor);

        HoldOnTriggerList = new UserService();

        Task<bool> confirmationTask = HoldOnTriggerList.WaitForUserConfirmation();

        await Task.Delay(100);

        // This line will await until HoldOnTriggerList is empty
        if (triggers.Count > 0)
        {
            bool confirmed = await confirmationTask;
        }
            

    }

    public static List<AwaitTrigger> triggers = new List<AwaitTrigger>();

    public static void AddToTriggerList(AwaitTrigger trigger)
    {
        triggers.Add(trigger);
        EventPotentialTriggersChanged.Invoke();
    }

    public static void RemoveFromTriggerList(AwaitTrigger trigger)
    {
        //Remove from trigger list should probably just trigger the menu again
        triggers.Remove(trigger);
        


        if (triggers.Count == 0)
            HoldOnTriggerList.OnUserActionCompleted(true);
        else
            EventPotentialTriggersChanged.Invoke();


    }



    private void OnDisable()
    {
        EventActorLeftTile = null;
        EventActorTookDamage = null;
        EventPotentialTriggersChanged = null;
    }
}
