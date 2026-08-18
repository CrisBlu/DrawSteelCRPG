using UnityEngine;


//This data is for querying the current state of the Player's Input option
public static class GF_PlayerInput
{
    
    public static Tile currentTileMouseOver;

    //Weird one, just a way of saving when an action doesn't have a reference to an actor, but the user obviously is acting with regards to this actor
    public static MB_Actor relevantActor;

    public static E_SelectState selectState;


    public static bool inputEnabled;
    public static bool isPlayerTurn;
    public static bool actorCommited;


}
