using UnityEngine;

//A player or AI controller, used for figuring out who's turn it is and what characters are under their command
[CreateAssetMenu(fileName = "SO_User", menuName = "Scriptable Objects/User")]
public class SO_User : ScriptableObject
{
    public TurnData activeTurn;

    private void OnEnable()
    {
        activeTurn = null;
    }
}
