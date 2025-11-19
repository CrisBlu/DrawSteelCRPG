using UnityEngine;

public class MB_Hero : MB_Actor
{
    [SerializeField] private SO_TurnManager Team;
    [SerializeField] private int MaxRecoveries;
    private int recoveries;
    private int recoveryValue = 5;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    protected override void Awake()
    {
        base.Awake();
        abilities.Add(new A_CatchBreath());
        Team.actorsUnderControl.Add(this);
        recoveries = MaxRecoveries;
    }

    protected override void RemoveFromWorld()
    {
        
        Team.actorsUnderControl.Remove(this);

        base.RemoveFromWorld();
    }

    public bool SpendRecovery()
    {
        if(recoveries <= 0) { return false; }

        recoveries -= 1;

        stamina += recoveryValue;
        return true;
    }

}
