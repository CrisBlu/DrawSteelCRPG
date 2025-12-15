using System.Collections.Generic;
using UnityEngine;

//A player or AI controller, used for figuring out who's turn it is and what characters are under their command

//This is an SO because it needs an inspector and will have multiple instances with inspecator differences
[CreateAssetMenu(fileName = "SO_User", menuName = "Scriptable Objects/User")]
public class SO_User : ScriptableObject
{
    public TurnData activeTurn;
    public List<MB_Actor> actorsUnderControl = new List<MB_Actor>();


    private void OnEnable()
    {
        activeTurn = null;
    }


    
}


//What I need is state manager, such that when a state changes it gets an exit function and enter function