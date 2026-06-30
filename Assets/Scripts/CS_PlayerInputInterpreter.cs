using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;

//Intakes player input and figures out what to do with it based on where they are in the turn structure
public static class PlayerInputInterpreter
{
    
    public static void ProcessInput(Tile input, SO_User user, SO_TurnManager TM)
    {
        if(input == null)
        {
            return;
        }

        //If there is no turn for user currently 
        if (user.activeTurn == null)
        {
            //AND valid actor is in selected Tile
            MB_Actor validActor = SelectYourActor(input, user);
            //Create a turn and return
            if (validActor != null) { TM.CreateAndStoreTurn(validActor); return; } else { return; }
        }

        
        switch(user.activeTurn.turnState)
        {
            case E_TurnState.SelectingMove:
                SelectingMove(user.activeTurn, input);
                break;

            case E_TurnState.SelectingAbility:
                ReturnToMove(user.activeTurn);
                break;

            case E_TurnState.UsingAbility:
                SelectingTarget(user.activeTurn, input);
                break;

            case E_TurnState.ResolvingAbility:
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
            turn.turnState = E_TurnState.SelectingAbility;
        }

        else if(turn.validTiles.Contains(input))
        {
            turn.turnState = E_TurnState.HoldingForAnimation;
        
            await Movement.ActorMovement(turn, input);

            turn.DefaultToState();
        }

        
    }
    
    private static void ReturnToMove(TurnData turn)
    {
        turn.turnState = E_TurnState.SelectingMove;
    }

    public static void SelectingAbility(CS_Ability ability, TurnData turn)
    {
        if (turn.actions[ability.Type] <= 0)
        {
            return;
        }
   

        turn.usingAbility = ability;
        turn.turnState = E_TurnState.UsingAbility;
    }


    private static void SelectingTarget(TurnData turn, Tile input)
    {
        if(!turn.validTiles.Contains(input))
        {
            turn.turnState = E_TurnState.SelectingAbility;
            return;
        }

        if(CS_AbilityParser.SetTarget(turn.usingAbility, turn.actor, input))
            turn.UseAbility(input);
    }

    private static void ResolvingAbility(TurnData turn, Tile input)
    {
        if(!turn.validTiles.Contains(input)) {return; }
        turn.ResolveAbility(input);
    }


    public static void HoveringOverAbility(CS_Ability ability, Tile origin)
    {
        CS_AbilityTargetingData output = ability.Target(origin);
        CS_ColorGrid.ColorCells(output.validArea, Color.magenta);
    }
    public static void HoverOffAbility(SO_GridData grid)
    {
        CS_ColorGrid.ClearGridColors(grid);
    }





}

// Every turn state must 