
using System.Collections.Generic;
using System.Linq;
using UnityEngine;





[CreateAssetMenu(fileName = "SO_Old_TurnManager", menuName = "Scriptable Objects/SO_Old_TurnManager")]
public class SO_Old_TurnManager : ScriptableObject
{
    public string role; 
    public List<MB_Old_Actor> actorsUnderControl = new List<MB_Old_Actor>();
    public bool finished;

 

    [SerializeField] private SO_GridSystem gridSystem;
    private SO_BattleManager battleManager;
    private List<C_MoveScore> possiblePlays = new List<C_MoveScore>(); 

    Dictionary<MB_Old_Actor, List<Tile>> possibleTargets = new Dictionary<MB_Old_Actor, List<Tile>>();
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
        

        battleManager = BattleManager;
        //For each Unit Possible
        //This is for each unit to "see" where the heroes are but ulimtely this is probably unneeded
        foreach (MB_Old_Actor unit in actorsUnderControl) 
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
                    Debug.Log(tile.entity.tag);
                    possibleTargets[unit].Add(tile);
                }
            }

        
        }

        //Best play in the scope of using this turn
        C_MoveScore bestPlay = new C_MoveScore(null, null, 9999);
        foreach (MB_Old_Actor unit in actorsUnderControl)
        {
            MB_Monster monster = (MB_Monster)unit;
            if (unit.turnTaken)
            {
                continue;
            }

            possibleSteps = gridSystem.GridBFSFromCell(unit.currentTile, unit.Speed, true);


            if (monster.advancing)
            {
                //Check every tile for it's distance from your goal
                foreach (Tile tile in possibleSteps)
                {
                    float bestScore = 100;
                    foreach (Tile target in possibleTargets[unit])
                    {
                        //check each possible target against square, distance wise, use lowest value as score
                        float currentScore = (target.position - tile.position).magnitude;
                        bestScore = currentScore < bestScore ? currentScore : bestScore;


                    }

                    if (bestScore < bestPlay.score)
                    {
                        bestPlay = new C_MoveScore(unit, tile, bestScore);
                    }



                }
            }
            else if (monster.watching)
            {
                foreach (Tile target in possibleTargets[unit])
                {
                    //Best score within the scope of attacking this target, lower is better
                    float bestScore = 100;
                    List<Tile> rangeOfAttack = CS_GridUtility.GetMovementArea(target, 5, true);
                    List<Tile> vaildTiles = possibleSteps.Intersect(rangeOfAttack).ToList();

                    foreach (Tile tile in vaildTiles)
                    {

                        float currentScore = -(target.position - tile.position).magnitude;
                        bestScore = currentScore < bestScore ? currentScore : bestScore;

                        if (bestScore < bestPlay.score)
                        {
                            bestPlay = new C_MoveScore(unit, tile, bestScore);
                        }

                    }

                }

                if (bestPlay.score > 100)
                {
                    bestPlay = new C_MoveScore(unit, monster.currentTile, 100);
                }

            }
          

            




        }

        MB_Monster activeMonster = (MB_Monster)bestPlay.unit;

        //Activate
        gridSystem.GridOnSelection(activeMonster.currentTile);

        //Move
        BattleManager.StartLookingForTarget(activeMonster.abilities[1]);
        gridSystem.GridOnSelection(bestPlay.position);





        //Wait a moment and then select Target and attack

        activeMonster.StartStagger(TargetAndAttack);
    }



    private void TargetAndAttack(CS_Ability ability, MB_Monster activeMonster)
    {
        //Reselect Actor if needed
        if(battleManager.selectState != E_SelectState.LookingForAction)
        {
            gridSystem.GridOnSelection(activeMonster.currentTile);
        }
        

        //Select Ability
        battleManager.StartLookingForTarget(ability);

        //Select nearbytarget and strike
        //Doesn't work for ranged attacks properly obvs, plays should probably store their target
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


        EndTurn();

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


public class C_MoveScore
{
    public MB_Old_Actor unit;
    public Tile position;
    public float score;


    public C_MoveScore(MB_Old_Actor unitMakingPlay, Tile positionUnitGoesTo, float scoreGiven)
    {
        unit = unitMakingPlay;
        position = positionUnitGoesTo;
        score = scoreGiven;
    }
}

/*
     foreach (MB_Actor unit in actorsUnderControl)
        {
            MB_Monster monster = (MB_Monster)unit;
            if (unit.turnTaken)
            {
                continue;
            }


possibleSteps = gridSystem.GridBFSFromCell(unit.currentTile, unit.Speed, true);

foreach (Tile target in possibleTargets[unit])
{
    //For each target in possible targets
    //  Check an attack based on your desired attacks
    foreach (CS_Ability ability in monster.desiredAbilities)
    {
        List<Tile> rangeOfAttack;
        if (ability.Name == "Charge")
        {
            rangeOfAttack = CS_GridUtility.GetMovementArea(target, unit.Speed, true);
        }
        else
        {
            rangeOfAttack = CS_GridUtility.GetMovementArea(target, ability.Range, true);
        }

        List<Tile> overlap = (List<Tile>)possibleSteps.Intersect(rangeOfAttack);

        //For each tile within the overlap, the desire tile is the one closest to the user's current position
        float bestScore = 100;
        foreach (Tile tile in overlap)
        {
            float score = 0;

            float currentScore = (target.position - tile.position).magnitude;
            bestScore = currentScore < bestScore ? currentScore : bestScore;
        }

    }


}
//The old algorithm scored based on all units and all targets, and basically pick whatever unit it had that to get as close to a target as possible

//Check every tile for it's distance from your goal
foreach (Tile tile in possibleSteps)
{



    
}
            

        }*/

