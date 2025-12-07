using TMPro;
using UnityEngine;

public class MB_SelectModeDisplay : MonoBehaviour
{
    [SerializeField] SO_BattleManager battleManager;
    [SerializeField] TextMeshProUGUI text;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        battleManager.EventSelectStateActor.AddListener(Actor);
        battleManager.EventSelectStateAction.AddListener(Action);
        battleManager.EventSelectStateMove.AddListener(Move);
        battleManager.EventSelectStateTarget.AddListener(Target);
        battleManager.EventSelectStateCell.AddListener(Cell);
    }

    void Actor()
    {
        text.text = "Select a Unit";
    }
    void Action()
    {
        text.text = "Select an Action";
    }
    void Move(int lol)
    {
        text.text = "Select a Tile to Move to";
    }
    void Target(CS_Ability lol)
    {
        text.text = "Select a Target";
    }
    void Cell()
    {
        text.text = "Select a Cell";
    }



}
