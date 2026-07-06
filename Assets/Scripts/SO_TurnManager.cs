using NUnit.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

//Could TurnData be a struct? 

public class TurnData //Store all the data associated with an actor's single turn with no reason to exist beyond that
{
    public MB_Actor actor;
    public CS_Ability usingAbility;
    public SO_TurnManager TurnManager;
    public SO_User TurnController;
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
        TurnController = actor.Controller;
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





    //This feels like an inherently user based feature, given the fact that it's only relevant to the player and AI doesn't even need this
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

            _turnState = value;

            //Enter
            switch (_turnState)
            {
                case E_TurnState.SelectingMove:

                    
                    validTiles = CS_GridUtility.GetWalkableTilesFromOrigin(actor.currentTile, actions[E_ActionType.move], false);
                    if (validTiles.Count != 0)
                    {
                        Color green = new Color(0, 1, 0, .25f);
                        CS_ColorGrid.ColorCells(validTiles, green);
                    }
          

                    break;

                case E_TurnState.SelectingAbility:
                    actor.DisplayAbilties(this);
                    break;

                case E_TurnState.UsingAbility:



                    /*CS_AbilityTargetingData targetOutput = usingAbility.Target(actor.currentTile);

                    if(targetOutput != null)
                    {
                        validTiles = targetOutput.validTargets;
                        CS_ColorGrid.ColorCells(targetOutput.validArea, Color.red);
                    }*/
                    

                    break;

                case E_TurnState.ResolvingAbility:
                    validTiles = AbilityHandler.currentCallback.validTiles;
                    CS_ColorGrid.ColorCells(validTiles, Color.blue);
                    break;

                case E_TurnState.HoldingForAnimation:
                    CS_ColorGrid.ClearGridColors(actor.currentTile.parentGrid);
                    break;
            }



        }

    }


    //This is currently, only used for AI behaviors, where the AI inputs a turn states and the system knows what to do because of that
    //Frankly, this is completely opposite to how the player input works, that reads the turn state and understands the player input through what it read
    //It seems to me that these concepts shouldn't be related in the way that they are
    public async void InvokeState(object input, E_TurnState stateToInvoke = E_TurnState.None)
    {

        if(stateToInvoke == E_TurnState.None)
        {
            stateToInvoke = _turnState;
        }

        switch (stateToInvoke)
        {
            case E_TurnState.SelectingMove:
                await Movement.ActorMovement(this, (Tile)input);
                TurnManager.EventNotifyAI.Invoke();

                break;

            case E_TurnState.SelectingAbility:
                usingAbility = (CS_Ability)input;
                TurnManager.EventNotifyAI.Invoke();
                break;

            case E_TurnState.UsingAbility:
                usingAbility.targets.Add((Tile)input);
                await UseAbility();
                TurnManager.EventNotifyAI.Invoke();

                break;

            case E_TurnState.ResolvingAbility:
                ResolveAbility((Tile)input);
                TurnManager.EventNotifyAI.Invoke();
                break;

            case E_TurnState.HoldingForAnimation:
                
                break;
        }

        /*if (TurnController.AI && _turnState != E_TurnState.HoldingForAnimation)
            TurnManager.EventNotifyAI.Invoke();
        */

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


    public async Task UseAbility(Tile input = null, CS_Ability ability = null)
    {
        if(ability == null)
        {
            ability = usingAbility;
        }


        if (actions[ability.Type] <= 0)
        {
            return;
        }

        //target = input;

        

        if (await AbilityHandler.TryAbility(ability, actor, this))
        {
            actions[ability.Type] -= 1;
        }

        if(SO_TurnManager.Instance.IsPlayerTurn)
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
    
    //Responsible for letting the rest of the game know that this user is now taking their turn
    [HideInInspector] public event Action<SO_User> EventActivateUser;

    [HideInInspector] public UnityEvent EventNotifyAI;

    private ZipperInit Initative;

    public static SO_TurnManager Instance;
    public bool IsPlayerTurn
    {
        get
        {
            if (!turnsToResolve.Peek().TurnController.AI)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
    [SerializeField] private List<SO_User> usersInBattle;
    private void OnEnable()
    {
        Instance = this; //

        
        Initative = new ZipperInit();

        InputSystem.actions.FindAction("Select").performed += StartBattle;

    }

    //On Battle Start temp
    private void StartBattle(InputAction.CallbackContext context)
    {
        InputSystem.actions.FindAction("Select").performed -= StartBattle;
        EventActivateUser.Invoke(Initative.EnableInitative(usersInBattle));
    }


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
            if (IsPlayerTurn)
                activeTurn.DefaultToState();
            else
                EventNotifyAI.Invoke();
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

    public void PassToOpponent()
    {
        EventActivateUser.Invoke(Initative.ShiftInitiative(usersInBattle));
    }

    private void OnDisable()
    {
        //turnsToResolve?.Clear();
        EventActivateUser = null;
    }
}
