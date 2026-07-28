using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

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



    public static async Task<bool> ReadAbility(CS_Ability ability)
    {
        for(int i = 0; i < ability.Instructions.Count; i++)
        {
            Debug.Log(ability.Instructions.Count);
            switch (ability.Instructions[i])
            {
                case E_AbilityInstructions.SelectTarget:


                    CS_AbilityTargetingData targetOutput = ability.Target(ability.Owner.currentTile);

                   
                    List<Tile> validTiles = targetOutput.validTargets.Except(ability.targets).ToList();

                    //We can imagine a system where tiles store highlight colors and display those above their usual color
                    CS_ColorGrid.ColorCells(targetOutput.validArea.Except(ability.targets).ToList(), Color.red);
                    CS_ColorGrid.ColorCells(validTiles, Color.yellow, false);
                    CS_ColorGrid.ColorCells(ability.targets, Color.white, false);


                    AwaitTile userInput = new AwaitTile(validTiles);
                    MB_PlayerInput.inputRequest = userInput;
                    Tile input = await userInput.WaitForUserConfirmation();



                    if (input == null)
                        return false;


                    if (!AbilityInstructions.SetTarget(input, ability.targets))
                        i--;

                    break;



                case E_AbilityInstructions.SpendResource:
                    AwaitConfirm spendConfirm = new AwaitConfirm("Would you like to spend X (Resource)?");
                    ConfirmQueue.AddToConfirmQueue(spendConfirm);
                    bool spendConfirmation = await spendConfirm.WaitForUserConfirmation();

                    if(spendConfirmation)
                    {
                        ability.Owner.resource -= ability.Cost;
                    }

                    ability.Spend(spendConfirmation);

                    break;




                case E_AbilityInstructions.Confirm:
                    AwaitConfirm userConfirm = new AwaitConfirm("Confirm Action?");

                    ConfirmQueue.AddToConfirmQueue(userConfirm);
                    bool confirmation = await userConfirm.WaitForUserConfirmation();

                    if (!confirmation)
                        i--;

                    break;


            }
        }

        return true;

        
    }







    public async Task<bool> TryAbility(CS_Ability ability, MB_Actor activeActor,  TurnData turn)
    {

        activeTurn = turn;
        int edges = 0;
        int banes = 0;
        //Check for flanking
        //ability.targets 0 is incorrect, likely each target should check it's own flank
        if (ability.Tags.Contains("melee") && ability.Tags.Contains("strike") && CS_GridUtility.CheckForFlanking(activeActor, ability.targets[0])) { edges++; }

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
