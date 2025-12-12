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

    public bool TryAbility(CS_Ability ability, MB_Old_Actor activeActor, Tile activeTarget)
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
        int banes = 0;
        //Check for flanking
        if (ability.Effects.Contains("melee") && ability.Effects.Contains("strike") && CS_GridUtility.CheckForFlanking(activeActor, activeTarget)) { edges++; }

        if(ability.Effects.Contains("ranged"))
        {
            //Code to determine if you get a bane while doing a ranged attack; almost certain should be somewhere where any ranged ability could access it ----------------------------------------------------------
            List<Tile> nextTo = activeActor.currentTile.FindNeighbors(activeActor.gridSystem);
            

            foreach (Tile neighbor in nextTo)
            {
                //This sucks and really, I should just be able to use tags
                if (neighbor.entity && neighbor.entity.GetType().IsSubclassOf(typeof(MB_Old_Actor)))
                {
                    //If actor has different tag from entity in neighbor tile
                    if (!activeActor.CompareTag(neighbor.entity.tag))
                    {
                        banes = 1;
                        break;
                    }
                }
            }
        }


        CS_AbilityReturnData returnData = ability.Use(new CS_AbilityInputData(activeActor, activeTarget, action, edges, banes));



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
