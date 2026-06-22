using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MB_EndTurnButton : MonoBehaviour
{
    [SerializeField] SO_TurnManager TurnManager;
    [SerializeField] TMP_Text text;
    Button button;

    private void Start()
    {
        button = GetComponent<Button>();
        TurnManager.EventPassInitative.AddListener(ManageButtonInteract);
    }

    //Better ways of doing this, there probably will be an event system coming from turn manager, but I'm not building for this
    private void Update()
    {
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

    private void ManageButtonInteract()
    {
        if(!TurnManager.turnsToResolve.Peek().TurnController.AI)
        {
            button.interactable = true;
        }
        else
        {
            button.interactable = false;
        }
    }

}
