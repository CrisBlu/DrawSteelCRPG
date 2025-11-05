using Mono.Cecil.Cil;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "SO_BattleManager", menuName = "Scriptable Objects/SO_BattleManager")]

///This should control the state of the fight and what the active User or AI is doing
public class SO_BattleManager : ScriptableObject
{
    [HideInInspector] public MB_Actor activeActor;
    [HideInInspector] public Tile activeTarget;
    [HideInInspector] public CS_Ability activeAbility;
    

    [HideInInspector] public bool lookingForTarget = false;

    //Events
    [HideInInspector] public UnityEvent<MB_Actor> EventActivateActor;


    private void OnEnable()
    {
        //On Begin turn 
        activeActor = null;
        activeTarget = null;
        activeAbility = null;

        lookingForTarget=false;

        if (EventActivateActor != null)
        {
            EventActivateActor = new UnityEvent<MB_Actor>();
        }


    }

    public void SetActiveActor(MB_Actor actor)
    {
        activeTarget = null;
        activeAbility = null;

        activeActor = actor;
        EventActivateActor.Invoke(activeActor);

  

    }

    public void SetActiveTarget(Tile target)
    {

        //Double click correct target
        if (activeTarget != target)
        {
            activeTarget = target;
            
        }
        else
        {
            activeAbility.Use(activeTarget);
        }

        
    }

    public void MoveAction(Tile cellToMoveTo, SO_GridSystem gridSystem)
    {
        List<Tile> stepsToTake = new List<Tile>();

        Tile current = gridSystem.GridMatrix[cellToMoveTo.position.x, cellToMoveTo.position.y];
        Tile origin = gridSystem.GridMatrix[activeActor.X, activeActor.Y];

        while (current != origin)
        {
            stepsToTake.Add(current);
            if (current.parent != null)
            {
                current = gridSystem.GridMatrix[current.parent.position.x, current.parent.position.y];
            }
            else
                break;
        }

        stepsToTake.Reverse();

        Debug.Log(activeActor + "moves to " +  cellToMoveTo.position);
        activeActor.ActorStartWalking(stepsToTake);
    }

    public void StartLookingForTarget(CS_Ability ability)
    {
        lookingForTarget = !lookingForTarget;
        Debug.Log("Looking For Target?" + lookingForTarget);

        if (lookingForTarget)
        {
            Debug.Log(ability.Name);
        }
        else
        {
            activeTarget = null;
        }

        activeAbility = ability;
    }



    private void OnDisable()
    {
        EventActivateActor.RemoveAllListeners();
    }


}
