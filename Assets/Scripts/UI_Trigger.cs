using UnityEngine;

public class UI_Trigger : MonoBehaviour
{
    [SerializeField] GameObject TriggerUITextBox;
    [SerializeField] TMPro.TextMeshProUGUI Text;
    private void OnEnable()
    {
        SO_BattleEvents.EventActorTookDamage += DisplayReasonForTrigger;
    }

    //Temp as hell
    private void Update()
    {
        if (SO_BattleEvents.triggers.Count > 0)
        {

        }
        else
        {
           TriggerUITextBox.SetActive(false);
        }
    }


    void DisplayReasonForTrigger(int damage, MB_Actor victim)
    {
        Text.text = string.Format("{0} took {1} damage, utilize trigger?", victim.name, damage);
        TriggerUITextBox.SetActive(true);
    }
}
