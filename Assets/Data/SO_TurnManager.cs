
using System.Collections.Generic;
using UnityEngine;



public class C_MoveScore
{
    public MB_Actor unit;
    public Tile position;
    public float score;

    public C_MoveScore(MB_Actor unitMakingPlay, Tile positionUnitGoesTo, float scoreGiven)
    {
        unit = unitMakingPlay;
        position = positionUnitGoesTo;
        score = scoreGiven;
    }
}


[CreateAssetMenu(fileName = "SO_TurnManager", menuName = "Scriptable Objects/SO_TurnManager")]
public class SO_TurnManager : ScriptableObject
{
    public string role; 
    public List<MB_Actor> actorsUnderControl = new List<MB_Actor>();
    public bool finished;

    [SerializeField] private SO_GridSystem gridSystem;
    private SO_BattleManager battleManager;
    private List<C_MoveScore> possiblePlays = new List<C_MoveScore>(); 

    Dictionary<MB_Actor, List<Tile>> possibleTargets = new Dictionary<MB_Actor, List<Tile>>();
    List<Tile> possibleSteps;


    //Goals
    //Kill all heroes
    //get to a place


    //For every square unit could go to
    //Next to a hero +3
    //Closer to a hero +2


    private void OnEnable()
    {
        finished = false;
    }



    public void YourTurn(SO_BattleManager BattleManager)
    {
        possibleSteps?.Clear();
        possibleTargets?.Clear();
        possiblePlays?.Clear();
        C_MoveScore bestPlay = new C_MoveScore(null, null, 9999);

        battleManager = BattleManager;
        //For each Unit Possible
        //This is for each unit to "see" where the heroes are but ulimtely this is probably unneeded
        foreach (MB_Actor unit in actorsUnderControl) 
        {
            if(unit.turnTaken)
            {
                continue;
            }
            //"See range"
            possibleSteps = gridSystem.GridBFSFromCell(unit.currentTile, 10, false);
            //possibleSteps = gridSystem.GridBFSFromCell(unit.currentTile, unit.speed + unit.abilities[0].Range, false);
            List<Tile> newList = new List<Tile>();
            possibleTargets.Add(unit, newList);

            foreach (Tile tile in possibleSteps)
            {
                if (tile.entity != null && tile.entity.CompareTag("Hero"))
                {
                    possibleTargets[unit].Add(tile);
                }
            }

        
        }

        foreach (MB_Actor unit in actorsUnderControl)
        {
            if (unit.turnTaken)
            {
                continue;
            }

            possibleSteps = gridSystem.GridBFSFromCell(unit.currentTile, unit.Speed, true);
           
            //Check every tile for it's distance from your goal
            foreach (Tile tile in possibleSteps)
            {
                float bestScore = 100;
                foreach(Tile target in possibleTargets[unit])
                {
                    //check each possible target against square, distance wise, use lowest value as score
                    float currentScore = (target.position - tile.position).magnitude;
                    bestScore = currentScore < bestScore ? currentScore : bestScore;


                }

                if(bestScore < bestPlay.score)
                {
                    bestPlay = new C_MoveScore(unit, tile, bestScore);
                }


                
            }


        }

        MB_Monster activeMonster = (MB_Monster)bestPlay.unit;

        //Activate
        gridSystem.GridOnSelection(activeMonster.currentTile);
        Debug.Log(BattleManager.activeActor);

        //Move
        BattleManager.StartLookingForTarget(activeMonster.abilities[3]);
        gridSystem.GridOnSelection(bestPlay.position);

        



        //Wait a moment and then select Target and attack
        
        activeMonster.StartStagger(TargetAndAttack);





    }

    private void TargetAndAttack(CS_Ability ability, MB_Monster activeMonster)
    {
        //Select Ability
        battleManager.StartLookingForTarget(ability);

        //Select nearbytarget and strike
        List<Tile> nearbyTargets = gridSystem.GridBFSFromCell(activeMonster.currentTile, ability.Range, false);

        foreach (Tile nearby in nearbyTargets)
        {
            if (nearby.entity != null && nearby.entity.CompareTag("Hero"))
            {
                gridSystem.GridOnSelection(nearby);
                gridSystem.GridOnSelection(nearby);
                break;
            }
        }

    }

    public void EndTurn()
    {
        battleManager.OnTurnEnd();
    }


    public void OnDisable()
    {
        actorsUnderControl?.Clear();
        possibleSteps?.Clear();
        possibleTargets?.Clear();
        possiblePlays?.Clear();
    }



}
