using Mono.Cecil.Cil;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "SO_BattleManager", menuName = "Scriptable Objects/SO_BattleManager")]

///This should control the state of the fight and what the active User or AI is doing
public class SO_BattleManager : ScriptableObject
{

    [HideInInspector] public SO_TurnManager activePlayer;
    private int activePlayerIndex;
    [SerializeField] private List<SO_TurnManager> Players = new List<SO_TurnManager>();

    [HideInInspector] public MB_Actor activeActor;
    [HideInInspector] public Tile[] activeTarget = new Tile[3];
    [HideInInspector] public CS_Ability activeAbility;


    [HideInInspector] public E_SelectState selectState;

    private int main = 0;
    private int manuever = 0;
    private int move = 0;
    public int currentSpeed;

    //Events
    [HideInInspector] public UnityEvent<MB_Actor> EventActivateActor;

    //State Enum
    //LookingForActor
    //LookingForCell
    //LookingForTarget


    private void OnEnable()
    {
        //On Begin turn 
        activePlayerIndex = 0;
        activePlayer = Players[activePlayerIndex];
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

    private void SetSelectState(E_SelectState newState)
    {
        selectState = newState;
    }

    public bool SetActiveActor(MB_Actor actor)
    {
        if(actor.turnTaken)
        {
            return false;
        }
        activeTarget[0] = null;
        activeTarget[1] = null;
        activeAbility = null;
        currentSpeed = actor.Speed;

        activeActor = actor;
        //selectState = E_SelectState.LookingForMove;



        EventActivateActor.Invoke(activeActor);
        return true;
  

    }

    public void SetActiveTarget(Tile target)
    {

        
        //Double click correct target
        if (activeTarget[0] != target)
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
        if(activeActor.movement <= 0)
        {
            Debug.Log("No movement left");
            return;
        }

        selectState = E_SelectState.None;

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
       
        activeActor.ActorStartWalking(stepsToTake, SetSelectState);

    }

    public void StartLookingForTarget(CS_Ability ability)
    {
        activeAbility = ability;

        //Self target
        if (activeAbility.Range == 0)
        {
            activeTarget[0] = activeActor.currentTile;
            UseAbility();
            return;
        }

        selectState = E_SelectState.LookingForTarget;
        Debug.Log("Looking For Target");
        
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

        if (activeAbility.Type == E_ActionType.move)
        {
            if (move <= 0)
            {
                Debug.Log("Move used");
                return;
            }
        }
        //Don't allow more selection by default when an ability is used
        

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

        if (activeAbility.Type == E_ActionType.move)
        {
            move -= 1;
        }

        activeAbility.Use(activeTarget);

        //Default assumption is that you'll want to finish you move after, and if you've already selected your move action, this is the only way to get back the move state
        selectState = E_SelectState.LookingForMove;


        //Tile[] testArray = new Tile[1] { activeTarget[0] };

    }

    public void TopOfTheRound()
    {
        foreach(SO_TurnManager player in Players)
        {
            player.finished = false;
            foreach(MB_Actor actor in player.actorsUnderControl)
            {
                actor.SetTurnTaken(false);
            }
        }
        Debug.Log("Top of the round");
        return;
    }

    public void OnTurnBegin()
    {


        activePlayerIndex = (activePlayerIndex + 1) % Players.Count;
        CheckIfAllActorsWent(Players[activePlayerIndex]);

        int roundCheck = activePlayerIndex;
        while (Players[activePlayerIndex].finished)
        {
            activePlayerIndex = (activePlayerIndex + 1) % Players.Count;
            CheckIfAllActorsWent(Players[activePlayerIndex]);

            if(roundCheck == activePlayerIndex)
            {
                TopOfTheRound();
                break;
            }
        }
        

        main = 1;
        manuever = 1;
        move = 1;

        selectState = E_SelectState.LookingForActor;

        activePlayer = Players[activePlayerIndex];

        if(activePlayerIndex == 1)
        {
            activePlayer.YourTurn(this);
        }



    }

    public void OnTurnEnd()
    {
        activeActor.SetTurnTaken(true);
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

    private bool CheckIfAllActorsWent(SO_TurnManager next)
    {
        foreach(MB_Actor actor in next.actorsUnderControl)
        {
            if(!actor.turnTaken)
            {
                return false;
            }
        }

        next.finished = true;
        return true;
    }



    private void OnDisable()
    {
        EventActivateActor.RemoveAllListeners();
    }


}
