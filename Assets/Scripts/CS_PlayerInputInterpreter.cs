using System.Collections.Generic;
using UnityEngine;
using static GF_PlayerInput;

//Intakes player input and figures out what to do with it based on where they are in the turn structure
public static class PlayerInputInterpreter
{
    
    public static async void ProcessInput(Tile input, SO_User user, SO_TurnManager TM)
    {
        if(input == null)
        {
            return;
        }

        
        switch(selectState)
        {
            case E_SelectState.SelectingActor:
                MB_Actor validActor = SelectYourActor(input, user);

                if(validActor == null) { return; }

                //If Mouse over an actor
                //View Actor's health (Assuming hidden by default)
                //View actor's move range

                UI_AbilityMenu.instance.LoadAbilitiesForViewing(validActor);
                relevantActor = validActor;

                List<Tile> walkingTiles = CS_GridUtility.GetWalkableTilesFromOrigin(relevantActor.currentTile, relevantActor.Speed, false);
                if (walkingTiles.Count != 0)
                {
                    Color green = new Color(0, 1, 0, .25f);
                    CS_ColorGrid.ColorCells(walkingTiles, green);
                }

                AwaitConfirm activateConfirm = new AwaitConfirm("Would you like to activate " + validActor.name + "?");
                ConfirmQueue.AddToConfirmQueue(activateConfirm);

                bool activateConfirmation = await activateConfirm.WaitForUserConfirmation();
                if(activateConfirmation)
                {
                    //Create a turn and return
                    TM.CreateAndStoreTurn(validActor);
                    return;
                }

                CS_ColorGrid.ClearGridColors(relevantActor.currentTile.parentGrid);
                relevantActor = null;
                UI_AbilityMenu.instance.UnloadAbilities();



                break;

            case E_SelectState.SelectingMove:
                SelectingMove(user.activeTurn, input);
                break;

            case E_SelectState.SelectingAbility:
                //If a tile is selected when an ability should be, leave SelectingAbility State
                ReturnToMove();
                break;

            case E_SelectState.UsingAbility:
                SelectingTarget(input);
                break;

            case E_SelectState.ResolvingAbility:
                ResolvingAbility(user.activeTurn, input);
                break;

        }

    }

    // will be in state class when that gets made
    private static MB_Actor SelectYourActor(Tile tile, SO_User user)
    {
        //If actor in tile exists and is your unit
        if(tile.entity is MB_Actor && user.actorsUnderControl.Contains((MB_Actor)tile.entity))
        {
            MB_Actor actor = (MB_Actor)tile.entity;

            //turn taken check
            if(!actor.turnTaken)
                return actor;
            else return null;

        }
        else
        {
            return null;
        }

    }

    private static async void SelectingMove(TurnData turn, Tile input)
    {

        if(input == turn.actor.currentTile)
        {
            MB_PlayerInput.Instance.SetSelectState(E_SelectState.SelectingAbility);
        }

        else if(turn.validTiles.Contains(input))
        {
            MB_PlayerInput.Instance.SetSelectState(E_SelectState.HoldingForAnimation);
        
            await Movement.ActorMovement(turn, input);

            turn.DefaultToState();
        }

        
    }
    
    private static void ReturnToMove()
    {
        MB_PlayerInput.Instance.SetSelectState(E_SelectState.SelectingMove);
    }

    public static async void SelectingAbility(CS_Ability ability, TurnData turn)
    {
        if (turn.actions[ability.Type] <= 0 || turn.actor.resource < ability.Cost)
        {
            return;
        }
   

        turn.usingAbility = ability;
        

        bool proceed = await CS_AbilityParser.ReadAbility(ability);

        if(proceed)
            await turn.UseAbility(null, ability);
    }


    private static void SelectingTarget(Tile input)
    {

        if (MB_PlayerInput.inputRequest == null)
        {
            return;
        }

        AwaitTile local = MB_PlayerInput.inputRequest;
        MB_PlayerInput.inputRequest = null;


        if (!local.validTiles.Contains(input))
        {
            //If input isn't valid move back to selecting ability state, feels bad, change
            MB_PlayerInput.Instance.SetSelectState(E_SelectState.SelectingAbility);
            local.OnUserActionCompleted(null);

            return;
        }


        local.OnUserActionCompleted(input);
            
        


    }

    private static void ResolvingAbility(TurnData turn, Tile input)
    {
        /*if(!turn.validTiles.Contains(input)) {return; }
        turn.ResolveAbility(input);*/
        Debug.LogError("This pathway triggered resolving ability, which is currently marked for deletion");


    }


    public static void HoveringOverAbility(CS_Ability ability, Tile origin)
    {
        CS_AbilityTargetingData output = ability.Target(origin);
        CS_ColorGrid.ColorCells(output.validArea, Color.magenta);
    }
    public static void HoverOffAbility(SO_GridData grid)
    {
        CS_ColorGrid.ClearGridColors(grid);

        if(selectState == E_SelectState.SelectingActor && relevantActor)
        {
            List<Tile> walkingTiles = CS_GridUtility.GetWalkableTilesFromOrigin(relevantActor.currentTile, relevantActor.Speed, false);
            if (walkingTiles.Count != 0)
            {
                Color green = new Color(0, 1, 0, .25f);
                CS_ColorGrid.ColorCells(walkingTiles, green);
            }
        }
    }





}

// Every turn state must 