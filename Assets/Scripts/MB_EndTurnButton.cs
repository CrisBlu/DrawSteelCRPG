using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MB_EndTurnButton : MonoBehaviour
{
    [SerializeField] SO_TurnManager TurnManager;
    [SerializeField] TMP_Text text;
    Button button;

    private bool isPlayerAI = true;

    private void Start()
    {
        button = GetComponent<Button>();
        SO_TurnManager.Instance.EventActivateUser += ManageButtonInteract;
    }


    private void Update()
    {
        if (isPlayerAI)
            return;


        int turnCount = TurnManager.turnsToResolve.Count;

        if (turnCount > 0)
        {
            
            if(turnCount == 1)
            {
                text.text = "End Turn";
            }
            else if(turnCount > 1)
            {
                text.text = "Done";
            }
        }

    }

    private void ManageButtonInteract(SO_User player)
    {
        if(!player.AI)
        {
            button.interactable = true;
            isPlayerAI = false;
        }
        else
        {
            button.interactable = false;
            isPlayerAI = true;
        }
    }

}
