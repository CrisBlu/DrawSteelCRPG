using System.Collections.Generic;
using UnityEngine;


public class TurnData //Store all the data associated with an actor's single turn with no reason to exist beyond that
{
    public MB_Actor actor;
    public CS_Ability usingAbiliy;

    //Suspect that these belong here
    public Tile target;
    public int edges;
    public int banes;

    public Queue<CS_CallbackData> resolveAbilityQueue;
    public List<Tile> validTiles = new List<Tile>();

    //Certain turns can be created with only specfifc kinds of actions able to be performed in them
    public string abilityTagRestrict;

    private E_TurnState _turnState;
   public E_TurnState turnState
   {
       get { return _turnState; }

       //TODO: State Class which gets this stuff out of this script
       set
       {
           //Exit
           switch (_turnState)
           {
               case E_TurnState.SelectingMove:
                    if(validTiles.Count != 0)
                    {
                        CS_ColorGrid.ClearGridColors(validTiles[0].parentGrid);
                        validTiles.Clear(); 
                    }
                   
                   break;

               case E_TurnState.SelectingAbility:
                    actor.HideAbilities();
                   break;

               case E_TurnState.UsingAbility:
                   validTiles.Clear();
                   break;

               case E_TurnState.ResolvingAbility:
                   break;

               case E_TurnState.HoldingForAnimation:
                   break;
           }

           _turnState = value;

           //Enter
           switch (_turnState)
           {
               case E_TurnState.SelectingMove:

                    validTiles = CS_GridUtility.GetTilesFromOrigin(actor.currentTile, actor.Speed, false);
                    if (validTiles.Count != 0)
                    {
                        CS_ColorGrid.ColorCells(validTiles, Color.green);
                    }
                    
                   break;

               case E_TurnState.SelectingAbility:
                    actor.DisplayAbilties(this);
                   break;

               case E_TurnState.UsingAbility:
                   validTiles = CS_GridUtility.GetTilesFromOrigin(actor.currentTile, usingAbiliy.Range, true);
                    if (validTiles.Count != 0)
                    {
                        CS_ColorGrid.ColorCells(validTiles, Color.red);
                    }
                    break;

               case E_TurnState.ResolvingAbility:
                   break;

               case E_TurnState.HoldingForAnimation:
                   break;
           }


       }

   }

    int mainAction;
    int maneuverAction;
    int moveAction;

    int movement;

    public TurnData(MB_Actor actingActor, int mainAction = 1, int maneuverAction = 1, int moveAction = 1, string abilityTagRestrict = null, E_TurnState turnState = E_TurnState.SelectingMove)
    {
        actor = actingActor;

        this.mainAction = mainAction;
        this.maneuverAction = maneuverAction;
        this.moveAction = moveAction;

        this.abilityTagRestrict = abilityTagRestrict;
        this.turnState = turnState;
    }
}

//This is a SO because it needs to be assigned in inspector
//I think maybe the constructor can just be in TurnData class and then the List stored in User perhaps
[CreateAssetMenu(fileName = "SO_TurnManager", menuName = "Scriptable Objects/TurnManager")]
public class SO_TurnManager : ScriptableObject
{
    List<TurnData> turnsToResolve = new List<TurnData>();
   
    public TurnData CreateAndStoreTurn(MB_Actor actor)
    {
        TurnData turnForActor = new TurnData(actor);
        turnsToResolve.Add(turnForActor);

      
        return turnForActor;
    }

    private void OnDisable()
    {
        turnsToResolve?.Clear();
    }
}
