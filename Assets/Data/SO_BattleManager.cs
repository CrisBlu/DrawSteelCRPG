using Mono.Cecil.Cil;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "SO_BattleManager", menuName = "Scriptable Objects/SO_BattleManager")]

///This should control the state of the fight and what the active User or AI is doing
//Ultimately I think I need to take the idea of select states more seriously, the grid display feature needs to be 100% linked with my selection state because that's all information for the player
public class SO_BattleManager : ScriptableObject
{

    [HideInInspector] public SO_TurnManager activePlayer;
    private int activePlayerIndex;
    [SerializeField] private List<SO_TurnManager> Players = new List<SO_TurnManager>();

    [HideInInspector] public MB_Actor activeActor;
    [HideInInspector] public Tile activeTarget;
    [HideInInspector] public CS_Ability activeAbility;


    [HideInInspector] public E_SelectState selectState;

    private int main = 0;
    private int manuever = 0;
    private int move = 0;
    public int currentSpeed;

    //Events
    [HideInInspector] public UnityEvent<MB_Actor> EventActivateActor;
    [HideInInspector] public UnityEvent<CS_Ability> EventSelectAbility;
    [HideInInspector] public UnityEvent EventTurnEnd;

    //State Enum
    //LookingForActor
    //LookingForCell
    //LookingForTarget

    public CS_AbilityParser AbilityParser = new CS_AbilityParser();
    private void OnEnable()
    {
        AbilityParser.SetUp(SetSelectState);

        //On Begin turn 
        activePlayerIndex = 0;
        activePlayer = Players[activePlayerIndex];
        activeActor = null;
        activeTarget = null;


        main = 1;
        manuever = 1;
        move = 1;

        selectState= E_SelectState.LookingForActor;


        EventActivateActor = new UnityEvent<MB_Actor>();
        


    }

    private void SetSelectState(E_SelectState newState)
    {
        selectState = newState;
    }

    public bool SetActiveActor(MB_Actor actor)
    {
        if(actor.turnTaken)
        {
            return false;
        }
        activeTarget = null;
 
        activeAbility = null;
        currentSpeed = actor.Speed;

        activeActor = actor;
        //selectState = E_SelectState.LookingForMove;



        EventActivateActor.Invoke(activeActor);
        return true;
  

    }

    public void SetActiveTarget(Tile target)
    {

        
        //Double click correct target
        if (activeTarget != target)
        {
            activeTarget = target;
            
        }
        else
        {
            TryAbility();
        }

        
    }

    public void MoveAction(Tile cellToMoveTo, SO_GridSystem gridSystem)
    {
        //This if statement is redudant
        if(activeActor.movement <= 0)
        {
            Debug.Log("No movement left");
            return;
        }

        selectState = E_SelectState.None;

        List<Tile> stepsToTake = new List<Tile>();

        Tile current = gridSystem.GridMatrix[cellToMoveTo.position.x, cellToMoveTo.position.y];
        Tile origin = gridSystem.GridMatrix[activeActor.X, activeActor.Y];

        while (current != origin)
        {
            stepsToTake.Add(current);
            if (current.parent != null)
            {
                current = gridSystem.GridMatrix[current.parent.position.x, current.parent.position.y];
            }
            else
                break;
        }

        stepsToTake.Reverse();
       
        activeActor.ActorStartWalking(stepsToTake, SetSelectState);

    }

    public void StartLookingForTarget(CS_Ability ability)
    {
        //If you're in the middle of selecting tiles for a move, don't change the state
        if(selectState == E_SelectState.LookingForCell)
        {
            return;
        }



        activeAbility = ability;
        activeTarget = null;


        TryAbility();

        if(ability.Type != E_ActionType.move)
        {
            EventSelectAbility.Invoke(activeAbility);
        }
        

    }


    public void TryAbility()
    {
        //Check if actor has that action type left
        if(activeAbility.Type == E_ActionType.main)
        {
            if(main <= 0)
            {
                Debug.Log("Main action used");
                return;
            }
        }

        if(activeAbility.Type == E_ActionType.manuever)
        {
            if(manuever <= 0)
            {
                Debug.Log("Manuever used");
                return;
            }
        }

        if (activeAbility.Type == E_ActionType.move)
        {
            if (move <= 0)
            {
                Debug.Log("Move used");
                return;
            }
        }

        //Try the ability, deduct action point if successful
        if(AbilityParser.TryAbility(activeAbility, activeActor, activeTarget))
        {
            activeActor.UseAbility();
            if (activeAbility.Type == E_ActionType.main)
            {
                main -= 1;
            }

            if (activeAbility.Type == E_ActionType.manuever)
            {
                manuever -= 1;
            }

            if (activeAbility.Type == E_ActionType.move)
            {
                move -= 1;
            }
        }



        //Tile[] testArray = new Tile[1] { activeTarget[0] };

    }

    public void TopOfTheRound()
    {
        foreach(SO_TurnManager player in Players)
        {
            player.finished = false;
            foreach(MB_Actor actor in player.actorsUnderControl)
            {
                actor.SetTurnTaken(false);
            }
        }
        Debug.Log("Top of the round");
        return;
    }

    public void OnTurnBegin()
    {


        activePlayerIndex = (activePlayerIndex + 1) % Players.Count;
        CheckIfAllActorsWent(Players[activePlayerIndex]);

        int roundCheck = activePlayerIndex;
        while (Players[activePlayerIndex].finished)
        {
            activePlayerIndex = (activePlayerIndex + 1) % Players.Count;
            CheckIfAllActorsWent(Players[activePlayerIndex]);

            if(roundCheck == activePlayerIndex)
            {
                TopOfTheRound();
                break;
            }
        }
        

        main = 1;
        manuever = 1;
        move = 1;

        selectState = E_SelectState.LookingForActor;

        activePlayer = Players[activePlayerIndex];

        if(activePlayerIndex == 1)
        {
            activePlayer.YourTurn(this);
        }



    }

    public void OnTurnEnd()
    {
        activeActor.SetTurnTaken(true);
        activeActor.TestTurnEnd(this);

        activeActor = null;
        activeTarget = null;
        activeAbility = null;

        

        main = 0;
        manuever = 0;
        move = 0;

        selectState = E_SelectState.None;
        EventTurnEnd.Invoke();


    }

    private bool CheckIfAllActorsWent(SO_TurnManager next)
    {
        foreach(MB_Actor actor in next.actorsUnderControl)
        {
            if(!actor.turnTaken)
            {
                return false;
            }
        }

        next.finished = true;
        return true;
    }



    private void OnDisable()
    {
        EventActivateActor.RemoveAllListeners();
        EventSelectAbility.RemoveAllListeners();
        EventTurnEnd.RemoveAllListeners();
    }


}
