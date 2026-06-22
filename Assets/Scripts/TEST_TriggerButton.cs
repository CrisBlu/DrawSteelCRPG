using UnityEngine;

public class TEST_TriggerButton : MonoBehaviour
{
    [SerializeField] GameObject Button;
    [SerializeField] MB_AbilityUI UI;

    private void OnEnable()
    {
        SO_BattleEvents.EventPotentialTriggersChanged += EnableButton;
    }

    //Temp as hell
    private void Update()
    {
        if(SO_BattleEvents.triggers.Count > 0)
        {
           
        }
        else
        {
            Button.SetActive(false);
        }
    }

    public void ConfirmButton()
    {
        /*foreach (AwaitTrigger trigger in SO_BattleEvents.triggers)
        {
            trigger.OnUserActionCompleted(false);
        }*/

        while (SO_BattleEvents.triggers.Count != 0)
        {
            SO_BattleEvents.triggers[0].OnUserActionCompleted(false);
        }

        UI.UnloadAbilities();

    }

    private void EnableButton()
    {
        Button.SetActive(true);
    }

    private void OnDisable()
    {
        
    }
}
