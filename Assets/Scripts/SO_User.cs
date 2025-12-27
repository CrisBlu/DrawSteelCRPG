using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//A player or AI controller, used for figuring out who's turn it is and what characters are under their command

//This is an SO because it needs an inspector and will have multiple instances with inspecator differences
[CreateAssetMenu(fileName = "SO_User", menuName = "Scriptable Objects/User")]
public class SO_User : ScriptableObject
{
    public SO_TurnManager TurnManager;

    //Feels temp, maybe should be in some sort of battle manager
    public bool finished = false;

    public bool AI = false;


    

    public TurnData activeTurn
    {
        get
        {
            TurnData turn;
            if(TurnManager.turnsToResolve.TryPeek(out turn))
            {
                return turn;
            }
            else
            {
                return null;
            }
        }

    }
    public List<MB_Actor> actorsUnderControl = new List<MB_Actor>();
    public List<MB_Squad> squadsUnderControl = new List<MB_Squad>();




    private void OnEnable()
    {
        if(AI)
        {
            userAI = new CS_UserAI();
            TurnManager.EventTurnStateUpdate.AddListener(OnTurnStateUpdate);
        }

        
    }




    private void OnDisable()
    {
        actorsUnderControl.Clear();
    }

    //Below will be on another script that inherents from user
    //... ideally

    public CS_UserAI userAI;
    private List<Play> aiActions;

    public void EnableAI(List<MB_Actor> targets)
    {
        aiActions = userAI.StartAI(squadsUnderControl, targets);

        foreach(Play aiAction in aiActions)
        {
            TurnManager.CreateAndStoreTurn(aiAction.unit, turnState: aiAction.inputs[0].state);
        }
        
    }

    private void OnTurnStateUpdate(E_TurnState state, bool enter)
    {
        if (!enter) return;

        if (state == E_TurnState.HoldingForAnimation) return;


        
        foreach(Play aiAction in aiActions)
        {
            GameInput input;
            if (aiAction.inputs.Count > 0)
            {
                input = aiAction.inputs[0];
            }
            else
            {
                TurnManager.EndCurrentTurn();
                return;
            }


            aiAction.inputs.RemoveAt(0);
            activeTurn.InvokeState(input.data, input.state);

        }
        


        
    }



}


//What I need is state manager, such that when a state changes it gets an exit function and enter function