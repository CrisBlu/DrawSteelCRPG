
using System.Collections.Generic;
using UnityEngine;


public static class AbilityInstructions
{



    public static bool SetTarget(Tile target, List<Tile> targets)
    {
        //Seems to be a general rule for multitarget abilities
        if (!targets.Contains(target))
        {
            targets.Add(target);
            CS_ColorGrid.ColorCells(targets, Color.yellow, false);
            return true;
        }

        return false;
            

    }

    /*public static bool SpendResource(int amount, MB_Actor acting)
    {
        acting.resource -= amount;
    }*/


}


public enum E_AbilityInstructions
{
    SelectTarget,
    Confirm,
    SpendResource
}
