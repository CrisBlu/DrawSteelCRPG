using TMPro;
using UnityEngine;

public class MB_AbilityItem : MonoBehaviour
{
    [SerializeField] TMP_Text Title;
    [SerializeField] TMP_Text Description;

    public CS_Ability Ability;

    public void UpdateText()
    {
        Title.text = Ability.Name;
        Description.text = Ability.Description;
    }


}