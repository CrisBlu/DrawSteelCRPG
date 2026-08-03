using UnityEngine;

public enum E_StatusType
{
    dead,
    downed,
    marked,
    bleeding,
    dazed,
    frightened,
    grabbed,
    prone,
    restrained,
    slowed,
    taunted,
    weakened

}

public enum E_StatusEnd
{
    EoT,
    Save,
    EoE,
    Never //Cannot be healed by any effect that ends conditions either
}

public struct Status
{
    public E_StatusType status;
    public E_StatusEnd end;

    public Status(E_StatusType status, E_StatusEnd end = E_StatusEnd.EoT)
    {
        this.status = status;
        this.end = end;
    }
}
