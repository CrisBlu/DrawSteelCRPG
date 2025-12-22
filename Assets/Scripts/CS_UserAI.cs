
using System;
using System.Collections.Generic;
using UnityEditor.Playables;
using UnityEngine;


//Score right now is based in possible damage
public class Play
{
    //Monster taking their turn
    public MB_Actor unit;

    //The list of inputs to follow if this is selected
    public List<GameInput> inputs = new List<GameInput>();

    //How valuable this Play is
    public float score;

    

    public Play(MB_Actor unit, float score = -99)
    {
        this.unit = unit;

        this.score = score;
    }
}

public class TileInput : GameInput
{
    public override E_TurnState state => _state;
    E_TurnState _state;

    public override object data => tile;
    Tile tile;

    public TileInput(E_TurnState state, Tile tile)
    {
        this.tile = tile;
        _state = state;

    }
}

public class AbilityInput : GameInput
{
    public override E_TurnState state => E_TurnState.SelectingAbility;
    public override object data => ability;
    CS_Ability ability;

    public AbilityInput(CS_Ability ability)
    {
        this.ability = ability;
    }
}

public abstract class GameInput
{
    public abstract E_TurnState state { get;}
    public abstract object data { get; }
}
//an A* search might be helpful for determining distances
/// <summary>
/// For each Acotr under my control
/// run an analysis unique to their sheet
/// they will return a play that has the highest score based on "how much they would like to do this"
/// 
///     The charger will either attack an enemy next to him, charge and attack an enemy, or move, then charge and attack an enemy
///     Charger: 
///     Consider spaces within melee range, if there is a target +1
///     Consider targets within charge range, if there is a target +1
///     Consider your walk range, if there is a tile you can walk to that will put you in range of a charge to a target +0
///     
/// Pick the play with the highest score and execute it
/// 
/// </summary>

public class CS_UserAI
{
    public Play StartAI(List<MB_Actor> actorsUnderControl, List<MB_Actor> targets)
    {
        
        //A sense of who is being targeted and by which of your creatures
        List<Play> plays = new List<Play>();

        foreach (MB_Actor actor in actorsUnderControl)
        {
            if(actor.turnTaken)
            {
                continue;
            }

            //At least one play for each actor under control, that play is worth -99 points and will just be to pass their turn
            plays.Add(new Play(actor));

            plays.AddRange(actor.sheet.EvaluateOptions(actor, targets));


        }

        Play bestPlay = null;
        foreach(Play play in plays)
        {
            if (bestPlay == null || bestPlay.score < play.score){ bestPlay = play; }


        }

        return bestPlay;


        
    }
}
