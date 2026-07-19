using UnityEngine;
using UnityEngine.UI;


public class MB_ActionGem : MonoBehaviour
{
    [SerializeField] SO_User Player;
    [SerializeField] SO_ActionType ActionType;
    [SerializeField] TMPro.TMP_Text text;
    Button button;

    private void Start()
    {
        button = GetComponent<Button>();
    }
    //Clicking button should bring up selectAction mode
    private void Update()
    {
        if (Player.activeTurn != null && Player.activeTurn.actions[ActionType.actionType] > 0)
        {
            button.interactable = true;
        }
        else
        {
            button.interactable = false;
        }

        if(text && Player.activeTurn != null)
        {
            text.text = Player.activeTurn.actions[ActionType.actionType].ToString();
        }

        
    }
}
