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
    [HideInInspector] public Tile[] activeTarget = new Tile[3];
    [HideInInspector] public CS_Ability activeAbility;


    [HideInInspector] public E_SelectState selectState;

    private int main = 0;
    private int manuever = 0;
    private int move = 0;

    //Events
    [HideInInspector] public UnityEvent<MB_Actor> EventActivateActor;

    //State Enum
    //LookingForActor
    //LookingForCell
    //LookingForTarget


    private void OnEnable()
    {
        //On Begin turn 
        activeActor = null;
        activeTarget[0] = null;
        activeTarget[1] = null;
        activeAbility = null;

        main = 1;
        manuever = 1;
        move = 1;

        selectState= E_SelectState.LookingForActor;

        if (EventActivateActor != null)
        {
            EventActivateActor = new UnityEvent<MB_Actor>();
        }


    }

    public void SetActiveActor(MB_Actor actor)
    {
        activeTarget[0] = null;
        activeTarget[1] = null;
        activeAbility = null;

        activeActor = actor;
        selectState = E_SelectState.LookingForMove;



        EventActivateActor.Invoke(activeActor);

  

    }

    public void SetActiveTarget(Tile target)
    {
        
        //Double click correct target
        if (activeTarget[0] == null)
        {
            activeTarget[0] = target;
            
        }
        else
        {
            UseAbility();
        }

        
    }

    public void MoveAction(Tile cellToMoveTo, SO_GridSystem gridSystem)
    {
        if(move <= 0)
        {
            Debug.Log("No movement left");
            return;
        }

        
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
        move -= 1;
        activeActor.ActorStartWalking(stepsToTake);
    }

    public void StartLookingForTarget(CS_Ability ability)
    {
        selectState = E_SelectState.LookingForTarget;
        Debug.Log("Looking For Target?" + selectState);
        
        //Need script specfifcally for changing values on state change
        /*if (selectState == E_SelectState.LookingForTarget)
        {
            Debug.Log(ability.Name);
        }
        else
        {
            activeTarget[0] = null;
            activeTarget[1] = null;
        }*/

        activeAbility = ability;
    }

    public void UseAbility()
    {
        if(activeAbility.Type == E_ActionType.main)
        {
            if(main <= 0)
            {
                Debug.Log("Main action used");
                return;
            }
        }

        if(activeAbility.Type == E_ActionType.manuever)
        {
            if(manuever <= 0)
            {
                Debug.Log("Manuever used");
                return;
            }
        }
        //New script, ability information gatherer
        //When using an ability check it's effects
        //for each effect do something else
        //Push - Ask user to select another space on the grid
        //

        if (activeAbility.Effects.Contains("push"))
        {
            if (activeTarget[1] == null)
            {
                selectState = E_SelectState.LookingForCell;
                Debug.Log("Need to select cell to push to");
                return;
            }
            
        }


        if (activeAbility.Type == E_ActionType.main)
        { 
            main -= 1;
        }

        if (activeAbility.Type == E_ActionType.manuever)
        {
            manuever -= 1;
        }

        //Tile[] testArray = new Tile[1] { activeTarget[0] };
        activeAbility.Use(activeTarget);
    }

    public void TopOfTheRound()
    {
        return;
    }

    public void OnTurnBegin()
    {

        main = 1;
        manuever = 1;
        move = 1;

        selectState = E_SelectState.LookingForActor;


    }

    public void OnTurnEnd()
    {
        activeActor.TestTurnEnd(this);

        activeActor = null;
        activeTarget[0] = null;
        activeTarget[1] = null;
        activeAbility = null;

        main = 0;
        manuever = 0;
        move = 0;

        selectState = E_SelectState.None;


    }



    private void OnDisable()
    {
        EventActivateActor.RemoveAllListeners();
    }


}
