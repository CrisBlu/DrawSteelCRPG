using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "SO_BattleManager", menuName = "Scriptable Objects/BattleManager")]
public class SO_BattleManager : ScriptableObject
{
    [HideInInspector] public UnityEvent moveEvent;
    [HideInInspector] public UnityEvent endTurnEvent;
    [HideInInspector] public UnityEvent beginTurnEvent;
    [HideInInspector] public bool moveAction = false;
    [HideInInspector] public bool manueverAction = false;
    [HideInInspector] public bool mainAction = false;
    [HideInInspector] public bool myTurn = false;

    private MBActor _activePlayer;
    [HideInInspector] public MBActor activePlayer
    {
        get
        {
            return _activePlayer;
        }
        set
        {
            _activePlayer = value; 
            _activePlayer.ActivateActor();
        }
    }
    


    private void OnEnable()
    {
        if (moveEvent != null)
        {
            moveEvent = new UnityEvent();
        }

        if (endTurnEvent != null)
        {
            endTurnEvent = new UnityEvent();
        }

        if (beginTurnEvent != null)
        {
            beginTurnEvent = new UnityEvent();
        }

    }



    public void OnMoveUsed(Vector2Int cords)
    {
        moveAction = false;

        _activePlayer.MoveActor(cords);
        moveEvent.Invoke();
    }

    public void EndTurn()
    {
        myTurn = false;
        moveAction = false;
        manueverAction=false;
        mainAction = false;

        _activePlayer = null;

        endTurnEvent.Invoke();
    }

    public void BeginTurn()
    {
        myTurn = true;
        moveAction = true;
        manueverAction = true;
        mainAction = true;

        beginTurnEvent.Invoke();
    }


    private void OnDisable()
    {
        moveEvent?.RemoveAllListeners();
        endTurnEvent?.RemoveAllListeners();
        beginTurnEvent?.RemoveAllListeners();
    }
}
