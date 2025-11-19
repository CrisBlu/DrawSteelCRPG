using System;
using System.Collections.Generic;
using UnityEngine;

public class CS_AbilityParser
{
    Action<E_SelectState> StateChange;
    private Queue<CS_CallbackData> callbackQueue = new();

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

    public void SetUp(Action<E_SelectState> function)
    {
        StateChange = function;
    }

    public bool TryAbility(CS_Ability ability, MB_Actor activeActor, Tile activeTarget)
    {
        //Try ability happens everytime the user fufills a step required for an ability to activate; once that certain ability has enough information
        //it'll trigger and return true

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

            //If ability needs specfifc target (like free strike)

            //If an ability can be cast on empty square 

        }

        CS_AbilityReturnData returnData = ability.Use(new CS_AbilityInputData(activeActor, activeTarget));

        if(returnData.callbackQueue != null)
        {
            callbackQueue = returnData.callbackQueue;
            StateChange(E_SelectState.LookingForCell);
        }
        else
        {
            StateChange(E_SelectState.LookingForMove);
        }
        
        return returnData.isSuccessful;
    }

    private void DealWithCallbacks(Tile cell)
    {
        CS_CallbackData currentCallback = callbackQueue.Dequeue();

        currentCallback.abilityCallback(currentCallback.target, cell);

        if(callbackQueue.Count <= 0)
        {
            StateChange(E_SelectState.LookingForMove);
        }
    }


}
