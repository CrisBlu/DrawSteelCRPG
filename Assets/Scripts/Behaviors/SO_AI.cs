
using System.Collections.Generic;
using UnityEngine;

public abstract class SO_AI : ScriptableObject
{
    public abstract List<GameInput> RunBehavior(TurnData turn, MB_Actor target);
}
