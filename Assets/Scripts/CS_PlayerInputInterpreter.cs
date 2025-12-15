using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;

//Intakes player input and figures out what to do with it based on where they are in the turn structure
public static class PlayerInputInterpreter
{
    
    public static TurnData ProcessInput(Tile input, SO_User user, SO_TurnManager TM)
    {
        //If there is no turn for user currently 
        if (user.activeTurn == null)
        {
            //AND valid actor is in selected Tile
            MB_Actor validActor = SelectYourActor(input, user);
            //Create a turn and return
            if (validActor != null) { return TM.CreateAndStoreTurn(validActor); } else { return null; }
        }

        
        switch(user.activeTurn.turnState)
        {
            case E_TurnState.SelectingMove:
                SelectingMove(user.activeTurn, input);
                break;

            case E_TurnState.SelectingAbility:
                break;

            case E_TurnState.UsingAbility:
                break;

            case E_TurnState.ResolvingAbility:
                break;

        }


        return user.activeTurn;
    }

    // will be in state class when that gets made
    private static MB_Actor SelectYourActor(Tile tile, SO_User user)
    {
        //If actor in tile exists and is your unit
        //Does (MB_Actor) on null throw an error?
        if((MB_Actor)tile.entity && user.actorsUnderControl.Contains((MB_Actor)tile.entity))
        {
            return (MB_Actor)tile.entity;
        }
        else
        {
            return null;
        }

    }

    private static void SelectingMove(TurnData turn, Tile input)
    {
        if(input == turn.actor.currentTile)
        {
            turn.turnState = E_TurnState.SelectingAbility;
        }
        if(turn.validTiles.Contains(input))
        {
            turn.actor.StartWalking(input);
        }
    }
    

    public static void SelectingAbility(CS_Ability ability, TurnData turn)
    {
        turn.usingAbiliy = ability;
        turn.turnState = E_TurnState.UsingAbility;
    }





}
