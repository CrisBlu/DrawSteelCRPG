using UnityEngine;

public class MBTurnManager : MonoBehaviour
{

    [SerializeField] SO_BattleManager BattleManager;
    //This is basically a test script at this point as it does nothing but trigger the player turn again
    
   
    void Start()
    {
        OnEndTurn();
        //BattleManager.moveEvent.AddListener(EndTurn);
        BattleManager.endTurnEvent.AddListener(OnEndTurn);
        BattleManager.beginTurnEvent.AddListener(MyTurn);
    }


    void Update()
    {
        
    }

    void OnEndTurn()
    {
        Invoke("TheEnemyWillDoThisNormally", 1f);
    }

    void MyTurn()
    {
        Debug.Log("My turn now!");
       
        //BattleManager.moveAction = true;
    }

    void TheEnemyWillDoThisNormally()
    {
        BattleManager.BeginTurn();
    }


}
