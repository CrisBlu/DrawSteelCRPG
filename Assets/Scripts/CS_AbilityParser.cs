using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class CS_AbilityParser
{
    public CS_CallbackData currentCallback;
    
    Queue<CS_CallbackData> callbackQueue;
    TurnData activeTurn;
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

    public static bool SetTarget(CS_Ability ability, MB_Actor activeActor, Tile activeTarget)
    {
        //Returns true if ability requires no more targets to function
        if (ability.SetTarget(activeTarget) == 0)
            return true;
        else
            return false;
    }

    public static void RemoveTarget(CS_Ability ability, MB_Actor activeActor, Tile activeTarget)
    {
        //Returns true if ability requires no more targets to function
        /*if (ability.targets.Count == 0)
            
        else*/
           
    }







    public async Task<bool> TryAbility(CS_Ability ability, MB_Actor activeActor, Tile activeTarget, TurnData turn)
    {

        activeTurn = turn;
        int edges = 0;
        int banes = 0;
        //Check for flanking
        if (ability.Tags.Contains("melee") && ability.Tags.Contains("strike") && CS_GridUtility.CheckForFlanking(activeActor, activeTarget)) { edges++; }

        if(ability.Tags.Contains("ranged"))
        {
            List<Tile> nextTo = activeActor.currentTile.FindNeighbors();
            

            foreach (Tile neighbor in nextTo)
            {
                //This sucks and really, I should just be able to use tags
                if (neighbor.entity && neighbor.entity.GetType().IsSubclassOf(typeof(MB_Actor)))
                {
                    //If actor has different tag from entity in neighbor tile
                    if (!activeActor.CompareTag(neighbor.entity.tag))
                    {
                        banes += 1;
                        break;
                    }
                }
            }
        }

        //TBH this should be a part of Use
        activeActor.ActorAnimator.SetTrigger("Attack");
        
        if (ability.Tags.Contains("ranged"))
        {
            foreach(Tile target in ability.targets)
                await activeActor.TestShoot(target);
        }
        
        CS_AbilityReturnData returnData = await ability.Use(turn);
        ability.targets.Clear();



        if(returnData.callbackQueue != null)
        {
            //Callback data parsing
            callbackQueue = returnData.callbackQueue;
            currentCallback = callbackQueue.Dequeue();
        }

            
            
        
        
        return returnData.isSuccessful;
    }


    private void DealWithCallbacks(Tile cell)
    {
        currentCallback.abilityCallback(activeTurn, cell);

        if(callbackQueue.Count <= 0)
        {
            currentCallback = null;
        }
        else
        {
           currentCallback = callbackQueue.Dequeue();
        }

        activeTurn.DefaultToState();
    }


}
