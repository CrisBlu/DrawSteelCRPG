using UnityEngine;

public class TEST_TriggerButton : MonoBehaviour
{
    [SerializeField] GameObject Button;

    private void OnEnable()
    {
        SO_BattleEvents.EventPotentialTriggerAdded += EnableButton;
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
       // UserService userService = SO_BattleEvents.triggers.Dequeue();

       // userService.OnUserActionCompleted(true);

    }

    private void EnableButton()
    {
        Button.SetActive(true);
    }

    private void OnDisable()
    {
        
    }
}
