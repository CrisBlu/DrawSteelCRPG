using UnityEngine;

public class TEST_TriggerButton : MonoBehaviour
{
    [SerializeField] GameObject Button;

    //Temp as hell
    private void Update()
    {
        if(SO_BattleEvents.triggers.Count > 0)
        {
            Button.SetActive(true);
        }
        else
        {
            Button.SetActive(false);
        }
    }

    public void ConfirmButton()
    {
        UserService userService = SO_BattleEvents.triggers.Dequeue();

        userService.OnUserActionCompleted(true);

    }
}
