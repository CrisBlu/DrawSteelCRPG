using NUnit.Framework.Interfaces;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class TurnData //Store all the data associated with an actor's single turn with no reason to exist beyond that
{
    public MB_Actor actor;
    public CS_Ability usingAbility;
    public SO_TurnManager TurnManager;
    public bool fullTurn;


    //I'm Suspect that these belong here
    public Tile target;
    public int edges;
    public int banes;
    //----------------------------------------------------
    CS_AbilityParser AbilityHandler;
    public List<Tile> validTiles = new List<Tile>();

    //Certain turns can be created with only specfifc kinds of actions able to be performed in them
    public string abilityTagRestrict;

    public Dictionary<E_ActionType, int> actions;


    public TurnData(MB_Actor actingActor, SO_TurnManager turnManager, int mainAction = 1, int maneuverAction = 1, int movement = -1, string abilityTagRestrict = null, E_TurnState turnState = E_TurnState.SelectingMove)
    {
        actor = actingActor;
        TurnManager = turnManager;
        actions = new Dictionary<E_ActionType, int>
        {
            { E_ActionType.main, mainAction },
            { E_ActionType.maneuver, maneuverAction},
            { E_ActionType.move, movement}

        };

        if(mainAction >= 1 && maneuverAction >= 1 && movement == -1) {fullTurn = true; }
        else { fullTurn = false; }


        //If movement has a value input by the turn creator
        if (actions[E_ActionType.move] == -1) { actions[E_ActionType.move] = actor.Speed; }
        

        this.abilityTagRestrict = abilityTagRestrict;
        this.turnState = turnState;

         AbilityHandler = new CS_AbilityParser();

    }



    //TODO: State Class which gets this stuff out of this script
    private E_TurnState _turnState;
    public E_TurnState turnState
    {
        get { return _turnState; }


        set
        {
            //Exit
            switch (_turnState)
            {
                case E_TurnState.SelectingMove:

                    CS_ColorGrid.ClearGridColors(actor.currentTile.parentGrid);
                    validTiles.Clear();


                    break;

                case E_TurnState.SelectingAbility:
                    actor.HideAbilities();

                    break;

                case E_TurnState.UsingAbility:
                    usingAbility = null;
                    CS_ColorGrid.ClearGridColors(actor.currentTile.parentGrid);
                    validTiles.Clear();

                    break;

                case E_TurnState.ResolvingAbility:
                    CS_ColorGrid.ClearGridColors(actor.currentTile.parentGrid);
                    validTiles.Clear();
                    break;

                case E_TurnState.HoldingForAnimation:
                    break;
            }

            TurnManager.EventTurnStateUpdate.Invoke(_turnState, false);
            _turnState = value;

            //Enter
            switch (_turnState)
            {
                case E_TurnState.SelectingMove:

                    
                    validTiles = CS_GridUtility.GetTilesFromOrigin(actor.currentTile, actions[E_ActionType.move], false);
                    if (validTiles.Count != 0)
                    {
                        CS_ColorGrid.ColorCells(validTiles, Color.green);
                    }
          

                    break;

                case E_TurnState.SelectingAbility:
                    actor.DisplayAbilties(this);
                    break;

                case E_TurnState.UsingAbility:
                    /*CS_AbilityTargetingData targetOutput = usingAbility.Target(actor.currentTile);
                    validTiles = targetOutput.validTargets;

                    CS_ColorGrid.ColorCells(targetOutput.validArea, Color.red);*/

                    break;

                case E_TurnState.ResolvingAbility:
                    validTiles = AbilityHandler.currentCallback.validTiles;
                    CS_ColorGrid.ColorCells(validTiles, Color.blue);
                    break;

                case E_TurnState.HoldingForAnimation:
                    CS_ColorGrid.ClearGridColors(actor.currentTile.parentGrid);
                    break;
            }

            TurnManager.EventTurnStateUpdate.Invoke(_turnState, true);


        }

    }

    public void InvokeState(object input, E_TurnState stateToInvoke = E_TurnState.None)
    {

        if(stateToInvoke == E_TurnState.None)
        {
            stateToInvoke = _turnState;
        }

        switch (stateToInvoke)
        {
            case E_TurnState.SelectingMove:
                StartWalking((Tile) input);

                break;

            case E_TurnState.SelectingAbility:
                usingAbility = (CS_Ability)input;
                turnState = E_TurnState.UsingAbility;

                break;

            case E_TurnState.UsingAbility:
                UseAbility((Tile)input);

                break;

            case E_TurnState.ResolvingAbility:
                ResolveAbility((Tile)input);
                break;

            case E_TurnState.HoldingForAnimation:
                
                break;
        }


    }


    //Use if unclear which state we should be in
    public void DefaultToState()
    {
        if(!TurnManager.CheckIfActive(this))
        {
            return;
        }

        if(AbilityHandler.currentCallback != null)
        {
            turnState = E_TurnState.ResolvingAbility;
            return;
        }

        if (actions[E_ActionType.move] > 0)
        {
            turnState = E_TurnState.SelectingMove;
            return;
        }
        
        if (actions[E_ActionType.main] > 0 || actions[E_ActionType.maneuver] > 0)
        {
            turnState = E_TurnState.SelectingAbility;
            return;
        }

        //TODO: End turn button to end turn with actions points
        //Helpful if it says cancel if there is more than one turn, is this the same as the back button? it probably should not be
        TurnManager.TryEndTurn(this);

    }

 
    

    //TODO: Remove functions below and place within a validation script

    public void StartWalking(Tile input)
    {
        
        if(actions[E_ActionType.move] <= 0) 
        {
            return;
        }

        actor.StartMovementInBattle(input, this); 
    }

    public void UseAbility(Tile input, CS_Ability ability = null)
    {
        if(ability == null)
        {
            ability = usingAbility;
        }


        if (actions[ability.Type] <= 0)
        {
            return;
        }

        target = input;

        

        if (AbilityHandler.TryAbility(ability, actor, input, this))
        {
            actions[ability.Type] -= 1;
        }


        DefaultToState();


    }

    public void ResolveAbility(Tile input)
    {
        AbilityHandler.selectedCell = input;
    }
}

//This is a SO because it needs to be assigned in inspector
//I think maybe the constructor can just be in TurnData class and then the List stored in User perhaps
[CreateAssetMenu(fileName = "SO_TurnManager", menuName = "Scriptable Objects/TurnManager")]
public class SO_TurnManager : ScriptableObject
{
    public Stack<TurnData> turnsToResolve = new Stack<TurnData>();

    //When one user stops acting and another starts
    [HideInInspector] public UnityEvent EventPassInitative;

    //TurnState Events
    [HideInInspector] public UnityEvent<E_TurnState, bool> EventTurnStateUpdate;
    
   
    public TurnData CreateAndStoreTurn(MB_Actor actor, int mainAction = 1, int maneuverAction = 1, int movement = -1, string abilityTagRestrict = null, E_TurnState turnState = E_TurnState.SelectingMove)
    {
        TurnData turnForActor = new TurnData(actor, this, mainAction, maneuverAction, movement, abilityTagRestrict, turnState);
        turnsToResolve.Push(turnForActor);

        
      
        return turnForActor;
    }

    public bool CheckIfActive(TurnData turn)
    {
        if (turnsToResolve.Peek() == turn)
        {
            return true;
        }

        return false;
    }

    public void TryEndTurn(TurnData turn)
    {
        //If turn is active turn, discard it
        if(CheckIfActive(turn))
        {

            EndCurrentTurn();
        }

        
    }

    public void EndCurrentTurn()
    {                                  
        TurnData discardedTurn = turnsToResolve.Pop();

            //Temp so ui elements do not stick around
        discardedTurn.turnState = E_TurnState.HoldingForAnimation;

        if(discardedTurn.fullTurn) { discardedTurn.actor.turnTaken = true; }

       WakeUpTurn();

        
    }

    public void WakeUpTurn()
    {
        //Set the state of the new active turn, potentially discarding it if it's empty
        TurnData activeTurn;
        if (turnsToResolve.TryPeek(out activeTurn))
        {
            activeTurn.DefaultToState();
        }
        else
        {
            PassToOpponent();
        }
    }

    /*public void ForceEndTurn()
    {
        MB_Actor activeActor = null;
        while(turnsToResolve.Count > 0)
        {
            TurnData turn = turnsToResolve.Pop();

            if (turnsToResolve.Count == 0)
            {
                activeActor = turn.actor;
                break;
            }
        }

        PassToOpponent(activeActor);
            
        
    }*/

    //passing actor as a reference solely so that I can mark it as having taken it's turn
    public void PassToOpponent()
    {
        EventPassInitative.Invoke();
    }

    private void OnDisable()
    {
        turnsToResolve?.Clear();
        EventPassInitative.RemoveAllListeners();
    }
}
