using System.Collections.Generic;
using UnityEngine;


public class TurnData
{
    public MB_Actor actingActor;
    public CS_Ability usingAbiliy;
    public Queue<CS_CallbackData> resolveAbilityQueue;

    int mainAction;
    int maneuverAction;
    int moveAction;

    int movement;

    public TurnData(MB_Actor actingActor, int mainAction = 1, int maneuverAction = 1, int moveAction = 1)
    {
        this.actingActor = actingActor;

        this.mainAction = mainAction;
        this.maneuverAction = maneuverAction;
        this.moveAction = moveAction;

    }
}

[CreateAssetMenu(fileName = "SO_TurnManager", menuName = "Scriptable Objects/TurnManager")]
public class SO_TurnManager : ScriptableObject
{
    List<TurnData> turnsToResolve;

    public object CreateAndStoreTurn(MB_Actor actor)
    {
        TurnData turnForActor = new TurnData(actor);
        turnsToResolve.Add(turnForActor);

        object turnReference = turnForActor;
        return turnReference;
    }

    private void OnDisable()
    {
        turnsToResolve.Clear();
    }
}
