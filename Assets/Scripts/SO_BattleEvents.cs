using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_BattleEvents", menuName = "Scriptable Objects/SO_BattleEvents")]
public class SO_BattleEvents : ScriptableObject
{

    [HideInInspector] public event Action<Tile, Tile, MB_Actor> EventActorLeftTile;
    [HideInInspector] public static event Func<int, MB_Actor, Task> EventActorTookDamage;
    private void OnEnable()
    {
        CS_BattleLog.BattleEvents = this;
    }

    public void TriggerActorLeftTileEvents(Tile exit, Tile entered, MB_Actor actor)
    {
        //Will ask all enemies with triggers that trigger when an enemy enters or leaves a space to check their triggers
        EventActorLeftTile.Invoke(exit, entered, actor);
    }

    public static async Task TriggerActorTookDamageEvents(int damage, MB_Actor actor)
    {
         await EventActorTookDamage.Invoke(damage, actor);
    }

    public static Queue<UserService> triggers = new Queue<UserService>();

    private void OnDisable()
    {
        EventActorLeftTile = null;
        EventActorTookDamage = null;
    }
}
