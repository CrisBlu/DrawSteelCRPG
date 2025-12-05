using UnityEngine;


public static class CS_DiceRoller
{
    public static int RollDice(int numberOfDice, int numberOnDice)
    {
        int results = 0;

        for(int i = 0; i < numberOfDice; i++)
        {
            results += Random.Range(1, numberOnDice + 1);
        }

        return results;
    }


    public static int PowerRoll(int bonus = 0, int edges = 0, int banes = 0)
    {
        int diceResult = RollDice(2, 10);
        Debug.Log("Raw Roll:" + diceResult);
        //Check for crit before any bonuses or edges applies
        if(diceResult >= 19) { return 4;  }

        int tierResult = 0;
        int totalEdge = Mathf.Clamp(edges + -banes, -2, 2);

        if(totalEdge == 2) { tierResult += 1; } else

        if(totalEdge == -2) { tierResult += -1; } else
        {
            //this should be -1, 0, or 1
            bonus += totalEdge * 2;
        }
        
        diceResult += bonus;
        Debug.Log("Roll With Bonus:" + diceResult);



        if (diceResult < 12) { tierResult += 1; } else

        if (diceResult >= 12 && diceResult <= 16) { tierResult += 2; } else

        if(diceResult > 16) { tierResult += 3; }

        tierResult = Mathf.Clamp(tierResult, 1, 3);

        return tierResult;
    }
}
