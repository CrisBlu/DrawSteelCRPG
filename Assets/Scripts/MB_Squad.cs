
using System.Collections.Generic;
using UnityEngine;

public class MB_Squad : MonoBehaviour
{
    public List<MB_Actor> actorsInSquad;
    public SO_User user;
    private void Start()
    {
        user.squadsUnderControl.Add(this);
    }
}
