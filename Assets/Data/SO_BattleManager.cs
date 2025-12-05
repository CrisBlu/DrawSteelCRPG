using Mono.Cecil.Cil;
using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "SO_BattleManager", menuName = "Scriptable Objects/SO_BattleManager")]

///This should control the state of the fight and what the active User or AI is doing
//Ultimately I think I need to take the idea of select states more seriously, the grid display feature needs to be 100% linked with my selection state because that's all information for the player
public class SO_BattleManager : ScriptableObject
{

    [HideInInspector] public SO_TurnManager activePlayer;
    private int activePlayerIndex;
    [SerializeField] private List<SO_TurnManager> Players = new List<SO_TurnManager>();

    [HideInInspector] private List<CS_ActorTurnStats> activeActors = new List<CS_ActorTurnStats>();
    [HideInInspector] public CS_ActorTurnStats temporaryReferenceToActiveActor;
    [HideInInspector] public MB_Actor activeActor;
    [HideInInspector] public Tile activeTarget;
    [HideInInspector] public CS_Ability activeAbility;


    [HideInInspector] public E_SelectState selectState;

    [HideInInspector] public int main = 0;
    private int manuever = 0;
    private int move = 0;

    //Events
    [HideInInspector] public UnityEvent<CS_ActorTurnStats> EventActivateActor;

    //Do we need an event or do we just need a reference to the grid?
    [HideInInspector] public UnityEvent<CS_Ability> EventSelectStateTarget;
    [HideInInspector] public UnityEvent EventSelectStateAction;
    [HideInInspector] public UnityEvent<int> EventSelectStateMove;
    [HideInInspector] public UnityEvent EventSelectStateCell;
    [HideInInspector] public UnityEvent EventTurnEnd;



    public CS_AbilityParser AbilityParser = new CS_AbilityParser();
    private void OnEnable()
    {
        AbilityParser.SetUp(SetSelectState, AddActorToTurn);

        //On Begin turn 
        activePlayerIndex = 0;
        activePlayer = Players[activePlayerIndex];
        activeActor = null;
        activeTarget = null;
        activeActors.Clear();

        /*
        main = 1;
        manuever = 1;
        move = 1;
        */

        selectState= E_SelectState.LookingForActor;


        EventActivateActor = new UnityEvent<CS_ActorTurnStats>();
        


    }

    private bool SetSelectState(E_SelectState newState)
    {
        //Keep feeling like there needs to be some sort of default state

        switch (newState)
        {
            case E_SelectState.LookingForActor:
                if (selectState != E_SelectState.None) { return false; }

                break;

            case E_SelectState.LookingForAction:
                if (selectState == E_SelectState.WaitingForAnimation) { return false; }

                EventSelectStateAction.Invoke();
                break;


            case E_SelectState.LookingForMove:
                if (selectState != E_SelectState.LookingForAction && selectState != E_SelectState.WaitingForAnimation && selectState != newState) { return false; }

                EventSelectStateMove.Invoke(activeActor.movement);
                break;


            case E_SelectState.LookingForTarget:
                //Only look for targets in the same situations where looking for a move would be acceptable
                if (selectState != E_SelectState.LookingForAction && selectState != E_SelectState.LookingForCell && selectState != newState) { return false; }

                activeTarget = null;
                EventSelectStateTarget.Invoke(activeAbility);
                break;


            case E_SelectState.LookingForCell:
                if (selectState != E_SelectState.LookingForTarget && selectState != newState) { return false; }

                EventSelectStateCell.Invoke();
                break;



            case E_SelectState.WaitingForAnimation:
                if (selectState != E_SelectState.LookingForMove) { return false; }

                break;


            case E_SelectState.None:
                if(selectState != E_SelectState.LookingForAction) { return false; }
                break;
        }


        Debug.Log("State: " + newState);
        selectState = newState;
        return true;
    }

    public bool SetActiveActor(MB_Actor actor)
    {
        if(actor.turnTaken)
        {
            return false;
        }

        /*
        activeTarget = null;
 
        activeAbility = null;
        */

        AddActorToTurn(new CS_ActorTurnStats(actor));

        
        SetSelectState(E_SelectState.LookingForAction);
        return true;
  

    }

    public void AddActorToTurn(CS_ActorTurnStats actorTurnInstance)
    {
        activeActors.Add(actorTurnInstance);
        activeActor = actorTurnInstance.actor;
        temporaryReferenceToActiveActor = actorTurnInstance;

        DisplayAbilities(temporaryReferenceToActiveActor);

    }
    
    //Is turn done?
    public bool RemoveActorFromTurn(CS_ActorTurnStats actorTurnInstance)
    {
        activeActors.Remove(actorTurnInstance);

        
         

        if (activeActors.Count > 0) 
        {
            temporaryReferenceToActiveActor = activeActors[activeActors.Count - 1];
            activeActor = temporaryReferenceToActiveActor.actor;
            DisplayAbilities(temporaryReferenceToActiveActor);
            return false;
        }
        else
        {
            activeActor.SetTurnTaken(true);
            activeActor.TestTurnEnd(this);

            temporaryReferenceToActiveActor = null;
            activeActor = null;

            return true;
        }
    }

    public void DisplayAbilities(CS_ActorTurnStats actor)
    {
        //Every time the actor is changed, this needs updating
        EventActivateActor.Invoke(actor);
    }

    public void SelectActiveActor()
    {
 
        SetSelectState(E_SelectState.LookingForMove);

    }

    public void ReturnToDefaultState()
    {
        SetSelectState(E_SelectState.LookingForAction);
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
            TryAbility();
        }

        
    }

    public void MoveAction(Tile cellToMoveTo, SO_GridSystem gridSystem)
    {
        //This if statement is redudant
        if(activeActor.movement <= 0)
        {
            Debug.Log("No movement left");
            return;
        }

        SetSelectState(E_SelectState.WaitingForAnimation);

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
       
        activeActor.ActorStartWalking(stepsToTake, EndMoveAction);

    }

    private void EndMoveAction()
    {

        SetSelectState(E_SelectState.LookingForMove);
        if (activeActor.movement <= 0) { SetSelectState(E_SelectState.LookingForAction); }
    }

    public void StartLookingForTarget(CS_Ability ability)
    {
        activeAbility = ability;



        if (!SetSelectState(E_SelectState.LookingForTarget))
        {
            return;
        }


       
        TryAbility();


    }


    public void TryAbility()
    {
        CS_ActorTurnStats abilityUser = temporaryReferenceToActiveActor;
        //Check if actor has that action type left
        if(activeAbility.Type == E_ActionType.main)
        {
            if(abilityUser.main <= 0)
            {
                Debug.Log("Main action used");
                return;
            }
        }

        if(activeAbility.Type == E_ActionType.manuever)
        {
            if(abilityUser.manuever <= 0)
            {
                Debug.Log("Manuever used");
                return;
            }
        }

        if (activeAbility.Type == E_ActionType.move)
        {
            if (abilityUser.move <= 0)
            {
                Debug.Log("Move used");
                return;
            }
        }

        //Try the ability, deduct action point if successful
        if(AbilityParser.TryAbility(activeAbility, activeActor, activeTarget))
        {
            activeActor.UseAbilityAnimation();
            if (activeAbility.Type == E_ActionType.main)
            {
                abilityUser.main -= 1;
            }

            if (activeAbility.Type == E_ActionType.manuever)
            {
                abilityUser.manuever -= 1;
            }

            if (activeAbility.Type == E_ActionType.move)
            {
                abilityUser.move -= 1;
            }

            if(temporaryReferenceToActiveActor.main == 0 && temporaryReferenceToActiveActor.manuever == 0 && temporaryReferenceToActiveActor.move == 0 && activeActor.movement == 0)
            {
                OnTurnEnd();
            }
        }



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
        
        /*
        main = 1;
        manuever = 1;
        move = 1;
        */
        SetSelectState(E_SelectState.LookingForActor);

        activePlayer = Players[activePlayerIndex];

        if(activePlayerIndex == 1)
        {
            activePlayer.YourTurn(this);
        }



    }

    public void OnTurnEnd()
    {
        //What if select state stored in a player class for eachc player?
        if (!SetSelectState(E_SelectState.None))
        {
            return;
        }

        EventTurnEnd.Invoke();
        if (!RemoveActorFromTurn(temporaryReferenceToActiveActor))
        {
            SetSelectState(E_SelectState.LookingForActor);
            SetSelectState(E_SelectState.LookingForAction);
            return; 
        }

        /*
        main = 0;
        manuever = 0;
        move = 0;
        */
        
        


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
        EventActivateActor?.RemoveAllListeners();
        EventSelectStateTarget?.RemoveAllListeners();
        EventSelectStateMove?.RemoveAllListeners();
        EventSelectStateCell?.RemoveAllListeners();
        EventTurnEnd?.RemoveAllListeners();
    }


}

public class CS_ActorTurnStats
{
    public MB_Actor actor;
    public int main;
    public int manuever;
    public int move;
    public string abilityTagRestrict;
    public Tile activeTarget = null;
    public CS_Ability activeAbility = null;

    public CS_ActorTurnStats(MB_Actor actor, int main = 1, int manuever = 1, int move = 1, string abilityTagRestrict = null)
    {
        this.actor = actor;
        this.main = main;
        this.manuever = manuever;
        this.move = move;
        this.abilityTagRestrict = abilityTagRestrict;
    }
}

