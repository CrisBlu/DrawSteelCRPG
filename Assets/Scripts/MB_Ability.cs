using TMPro;
using UnityEngine;

public class MB_Ability : MonoBehaviour
{
    [SerializeField] TMP_Text Title;
    [SerializeField] TMP_Text Description;

    [SerializeField] SO_BattleManager BattleManager;

    public CS_Ability Ability;

    public void UpdateText()
    {
        Title.text = Ability.Name;
        Description.text = Ability.Description;
    }


}
