
using System;
using System.Collections.Generic;
using UnityEditor.Playables;
using UnityEngine;


//Score right now is based in possible damage
public class Play
{

    public MB_Actor unit;

    //The list of inputs to follow if this is selected
    public List<GameInput> inputs = new List<GameInput>();

    //How valuable this Play is
    public float score;

    

    public Play(MB_Actor unit, float score = -10)
    {
        this.unit = unit;

        this.score = score;
    }
}

public class SquadPlay
{
    public List<Play> playForEachActor = new List<Play>();

    public float totalScore = 0;
    public float score = 0;

    public void AddPlay(Play play)
    {
        playForEachActor.Add(play);

        totalScore += play.score;
        score = totalScore / playForEachActor.Count;

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
    public List<Play> StartAI(List<MB_Squad> squadsUnderControl, List<MB_Actor> targets)
    {

        List<SquadPlay> squadPlays = new List<SquadPlay>();


        foreach (MB_Squad squad in squadsUnderControl)
        {
            SquadPlay squadPlay = new SquadPlay();
            foreach (MB_Actor actor in squad.actorsInSquad)
            {
                if (actor.turnTaken)
                {
                    continue;
                }
                List<Play> plays = new List<Play>() { new Play(actor) };

                //At least one play for each actor under control, that play is worth -10 points and will just be to pass their turn


                plays.AddRange(actor.sheet.EvaluateOptions(actor, targets));

                Play bestPlayForActor = null;
                foreach (Play play in plays)
                {
                    if (bestPlayForActor == null || bestPlayForActor.score < play.score) { bestPlayForActor = play; }

                }

                squadPlay.AddPlay(bestPlayForActor);

            }

            if (squadPlay.playForEachActor.Count > 0) { squadPlays.Add(squadPlay); }


        }


        SquadPlay bestSquadPlay = null;
        foreach (SquadPlay play in squadPlays)
        {
            if (bestSquadPlay == null || bestSquadPlay.score < play.score) { bestSquadPlay = play; }

        }

        return bestSquadPlay.playForEachActor;



    }

}