using UnityEngine;

public abstract class SO_AI : ScriptableObject
{
    public abstract Play RunBehavior(MB_Actor self, MB_Actor target);
}
