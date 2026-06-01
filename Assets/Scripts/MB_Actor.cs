using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MB_Actor : MB_Entity //All functions relating and requiring a certain instance of actor
{
    
    [SerializeField] private SO_User Controller;
    [SerializeField] private SO_ActorEvents ActorEvents;

    //Temp
    public bool turnTaken = false;

    //public List<CS_Ability> abilities = new List<CS_Ability>();
    public Dictionary<string, CS_Ability> abilities = new Dictionary<string, CS_Ability>();
    public SO_CharacterSheet sheet;


    public int Speed = 5;
    protected override void Start()
    {
        base.Start();
        
        abilities.Add("Knockback" ,new A_Knockback());
        abilities.Add("Charge", new A_Charge());

        sheet.character = this;
        sheet.LoadAbilities(abilities);

        Controller.actorsUnderControl.Add(this);

    }



    public void ForcedMovement(Tile pushedInto, int distance)
    {
        //Shoves the actor into the next square to their destination, up to the distance
        //If something exists in that space, take damage and don't move 
        Vector2Int nextCell = currentTile.position;
        while (currentTile != pushedInto)
        {
            if (distance == 0)
            {
                break;
            }

            if (currentTile.position.x != pushedInto.position.x)
            {
                nextCell.x = pushedInto.position.x > currentTile.position.x ? nextCell.x + 1 : nextCell.x - 1;
            }

            if (currentTile.position.y != pushedInto.position.y)
            {
                nextCell.y = pushedInto.position.y > currentTile.position.y ? nextCell.y + 1 : nextCell.y - 1;
            }
            distance -= 1;

            if(!UpdatePosition(gridData.GetTile(nextCell)))
            {
                TakeDamage(1);

                gridData.GetTile(nextCell).entity.TakeDamage(1);

                nextCell = currentTile.position;
            }
        }

        //UpdatePosition(origin);
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
