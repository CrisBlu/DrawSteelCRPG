using UnityEngine;
using UnityEngine.UI;

public class MBEndTurnButton : MonoBehaviour
{
    [SerializeField] private SO_BattleManager BattleManager;
    private Button button;

    void Awake()
    {
        button = GetComponent<Button>();
        BattleManager.endTurnEvent.AddListener(SetButtonInactive);
        BattleManager.beginTurnEvent.AddListener(SetButtonActive);
    }

    void SetButtonInactive()
    {
        button.interactable = false;
    }

    void SetButtonActive()
    {
        button.interactable = true;
    }
}
