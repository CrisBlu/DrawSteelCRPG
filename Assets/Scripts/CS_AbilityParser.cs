using System;
using System.Collections.Generic;
using UnityEngine;

public class CS_AbilityParser
{
    Func<E_SelectState, bool> StateChange;
    Action<CS_ActorTurnStats> action;
    private Queue<CS_CallbackData> callbackQueue = new();
    CS_CallbackData currentCallback;

    private Tile _selectedCell;
    public Tile selectedCell
    {
        get
        {
            return _selectedCell;
        }
        set
        {
            _selectedCell = value;
            DealWithCallbacks(_selectedCell);
        }
    }

    public List<Tile> validTiles = new List<Tile>();

    public void SetUp(Func<E_SelectState, bool> function, Action<CS_ActorTurnStats> action)
    {
        validTiles.Clear();
        StateChange = function;
        this.action = action;
    }

    public bool TryAbility(CS_Ability ability, MB_Actor activeActor, Tile activeTarget)
    {
        //Try ability happens everytime the user fufills a step required for an ability to activate; once that certain ability has enough information
        //it'll trigger and return true

        //Try ability needs a look, I think it has some checks that are now redudant

        //Select Target
        if (activeTarget == null)
        {
            //Self target
            if (ability.Range == 0)
            {
                activeTarget = activeActor.currentTile;
                //return UseAbility(ability, self);
            }
            else
            {
                //User needs to select target
                StateChange(E_SelectState.LookingForTarget);
                return false;
            }

        }

        int edges = 0;
        //Check for flanking
        if(ability.Effects.Contains("melee") && ability.Effects.Contains("strike") && CS_GridUtility.CheckForFlanking(activeActor, activeTarget)) { edges++; }


        CS_AbilityReturnData returnData = ability.Use(new CS_AbilityInputData(activeActor, activeTarget, action, edges));



        if(returnData.callbackQueue != null)
        {
            //Callback data parsing
            callbackQueue = returnData.callbackQueue;
            currentCallback = callbackQueue.Dequeue();
            validTiles = currentCallback.validTiles;

            //State change
            StateChange(E_SelectState.LookingForCell);
        }
        else
        {
            StateChange(E_SelectState.LookingForAction);
            if (ability.Type == E_ActionType.move)
            {
                StateChange(E_SelectState.LookingForMove);
            }
            
        }
        
        return returnData.isSuccessful;
    }



    private void DealWithCallbacks(Tile cell)
    {
        currentCallback.abilityCallback(currentCallback.target, cell);

        if(callbackQueue.Count <= 0)
        {
            StateChange(E_SelectState.LookingForAction);
        }
        else
        {
           currentCallback = callbackQueue.Dequeue();
        }
    }


}
