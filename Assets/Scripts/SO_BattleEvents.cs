using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_BattleEvents", menuName = "Scriptable Objects/SO_BattleEvents")]
public class SO_BattleEvents : ScriptableObject
{

    [HideInInspector] public event Action<Tile, Tile, MB_Actor> EventActorLeftTile;
    [HideInInspector] public static event Action<RequestPowerRoll> EventBeforePowerRoll;

    [HideInInspector] public static event Action<RequestDamage> EventBeforeTakeDamage;
    [HideInInspector] public static event Action<int, MB_Actor> EventActorTookDamage;
    [HideInInspector] public static event Action<RequestForceMove> EventBeforeForcedMoved;
    [HideInInspector] public static event Action<Tile> EventActorForcedMoved;

    [HideInInspector] public static event Action EventPotentialTriggersChanged;

    [HideInInspector] public static Queue<IRequest> RequestQueue = new();



    static bool requestLock = false;
    public static async void TestingGoThroughQueue()
    {
        //For now, bullshit
        if(requestLock) { return; }

        requestLock = true;
        while (RequestQueue.Count > 0)
        {
            IRequest request = RequestQueue.Dequeue();
            await request.InvokeBeforeTriggers();

            //If rest of request is not canceled
            if (request.Cancel != true)
            {
                await request?.Resolve();
            }
            
        }

        requestLock = false;

        //Needs a return to default function, can't use GF_PlayerInput because SelectMove requires an actor
        if (SO_TurnManager.Instance.IsPlayerTurn)
            MB_PlayerInput.Instance.SetSelectState(E_SelectState.SelectingMove);

    }

    public static void AddRequest(IRequest request)
    {
        RequestQueue.Enqueue(request);
        TestingGoThroughQueue();
    }

    static public WaitFor HoldOnTriggerList;
    private void OnEnable()
    {
        CS_BattleLog.BattleEvents = this;
    }

    public void TriggerActorLeftTileEvents(Tile exit, Tile entered, MB_Actor actor)
    {
        //Will ask all enemies with triggers that trigger when an enemy enters or leaves a space to check their triggers
        EventActorLeftTile.Invoke(exit, entered, actor);
    }

    public static async Task TriggerBeforePowerRollEvents(RequestPowerRoll request)
    {
        EventBeforePowerRoll?.Invoke(request);

        await HoldUntilTriggersAreDone();
    }


    public static async Task TriggerBeforeTakeDamageEvents(RequestDamage request)
    {
        EventBeforeTakeDamage.Invoke(request);

        await HoldUntilTriggersAreDone();

    }



    public static async Task TriggerActorTookDamageEvents(int damage, MB_Actor actor)
    {
        EventActorTookDamage.Invoke(damage, actor);

        await HoldUntilTriggersAreDone();

    }

    public static async Task TriggerBeforeForcedMovedEvents(RequestForceMove request)
    {
        EventBeforeForcedMoved?.Invoke(request);

        await HoldUntilTriggersAreDone();

    }

    public static async Task TriggerActorForcedMovedEvents(Tile target)
    {
        EventActorForcedMoved?.Invoke(target);


        await HoldUntilTriggersAreDone();

    }

    private static async Task HoldUntilTriggersAreDone()
    {
        HoldOnTriggerList = new WaitFor();

        //Required because the script doesn't wait long enough to let triggers populate by default
        await Task.Delay(100);

        // This line will await until HoldOnTriggerList is empty
        if (triggers.Count > 0)
        {
            await HoldOnTriggerList.WaitForUserConfirmation();
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
        EventBeforeForcedMoved = null;
        EventActorForcedMoved = null;


        RequestQueue.Clear();

    }
}
