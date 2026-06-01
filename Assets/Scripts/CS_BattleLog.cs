
using System.Collections.Generic;
using UnityEngine;

public static class CS_BattleLog
{
    public static SO_BattleEvents BattleEvents;
    static List<(Tile, Tile)> triggerEvents = new List<(Tile, Tile)>();

    public static void LogStep(Tile left, Tile entered, MB_Actor actor)
    {
        //BattleEvents.TriggerActorLeftTileEvents(left, actor);
    }
}
