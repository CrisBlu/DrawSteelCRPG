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
            TurnManager.EventNotifyAI.AddListener(OnTurnStateUpdate);
        }

        
    }




    private void OnDisable()
    {
        actorsUnderControl.Clear();
        squadsUnderControl.Clear();
    }

    //Below will be on another script that inherents from user
    //... ideally

    public CS_UserAI userAI;
    private List<MB_Actor> actingActors;
    private List<MB_Actor> targets;

    List<GameInput> aiActions;

    public void EnableAI(List<MB_Actor> targets)
    {

        aiActions = null;

        actingActors = userAI.StartAI(squadsUnderControl, targets);
        this.targets = targets;
  
        foreach(MB_Actor actor in actingActors)
        {
            TurnManager.CreateAndStoreTurn(actor);
        }

        OnTurnStateUpdate();



    }

    
    private void OnTurnStateUpdate()
    {
        //For now this will work but won't for more than one AI controlled party
        if (!AI)
            return;


        if (aiActions == null || aiActions.Count <= 0)
        {
            aiActions = activeTurn.actor.sheet.RunBehavior(activeTurn, targets);
            if (aiActions == null)
            {
                TurnManager.EndCurrentTurn();
                return;
            }
        }
   
        GameInput currentInput = aiActions[0];
        aiActions.RemoveAt(0);


        activeTurn.InvokeState(currentInput.data, currentInput.state);

 

    }



}


//What I need is state manager, such that when a state changes it gets an exit function and enter function