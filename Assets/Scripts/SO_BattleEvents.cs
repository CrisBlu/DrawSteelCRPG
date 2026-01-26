using System;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "SO_BattleEvents", menuName = "Scriptable Objects/SO_BattleEvents")]
public class SO_BattleEvents : ScriptableObject
{

    [HideInInspector] public event Action<Tile, MB_Actor> EventActorLeftTile;
    private void OnEnable()
    {
        CS_BattleLog.BattleEvents = this;
    }

    public void Trigger(Tile tilePos, MB_Actor actor)
    {
        EventActorLeftTile.Invoke(tilePos, actor);
    }
}
