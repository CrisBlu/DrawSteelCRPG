using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MB_Actor : MB_Entity //All functions relating and requiring a certain instance of actor
{
    
    [SerializeField] private SO_User Controller;
    [SerializeField] private SO_ActorEvents ActorEvents;

    public List<CS_Ability> abilities = new List<CS_Ability>();
    public SO_CharacterSheet sheet;


    public int Speed = 5;
    protected override void Start()
    {
        base.Start();
        abilities.AddRange(sheet.LoadAbilities());
        Controller.actorsUnderControl.Add(this);
    }



    public void StartWalking(Tile destination, Action callbackAtEnd = null)
    {
        //Get path to desired target
        List<Tile> stepsToTake = new List<Tile>();

        Tile current = destination;
        Tile origin = currentTile;

        while (current != origin)
        {
            stepsToTake.Add(current);
            if (current.parent != null)
            {
                current = current.parent;
            }
            else
                break;
        }

        stepsToTake.Reverse();

        StartCoroutine(ActorWalking(stepsToTake, callbackAtEnd));

        //ActorEventManager.EventActorWalk.Invoke();
    }

    private IEnumerator ActorWalking(List<Tile> stepsToTake, Action callbackAtEnd = null)
    {
        //isWalking = true;

        yield return new WaitForSeconds(1f);
        while (stepsToTake.Count > 0)
        {
            /*if (movement <= 0)
            {
                break;
            }

            movement--;*/

            UpdatePosition(stepsToTake[0]);

            stepsToTake.RemoveAt(0);
            yield return new WaitForSeconds(.2f);
        }

        //When done moving
        //isWalking = false;

        callbackAtEnd?.Invoke();



        yield return null;
    }

    public void DisplayAbilties(TurnData turn)
    {
        ActorEvents.TriggerDisplayAbilities(turn);
    }

    public void HideAbilities()
    {
        ActorEvents.TriggerHideAbilities();
    }
}
