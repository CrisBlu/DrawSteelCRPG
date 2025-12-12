using UnityEngine;

public class MB_Hero : MB_Old_Actor
{
    [SerializeField] private SO_Old_TurnManager Team;
    [SerializeField] private int MaxRecoveries;
    private int recoveries;
    private int recoveryValue = 5;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    protected override void Awake()
    {
        
        base.Awake();
        recoveryValue = stamina / 3;
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
        StaminaDisplay.text = stamina.ToString();
        return true;
    }

}
