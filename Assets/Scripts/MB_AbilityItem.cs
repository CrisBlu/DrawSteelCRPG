using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class MB_AbilityItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] TMP_Text Title;
    [SerializeField] TMP_Text Description;

    public CS_Ability Ability;
    public MB_Actor Actor;

    public void UpdateText()
    {
        Title.text = Ability.Name;
        Description.text = Ability.Description;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        PlayerInputInterpreter.HoveringOverAbility(Ability, Actor.currentTile);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        PlayerInputInterpreter.HoverOffAbility(Actor.currentTile.parentGrid);
    }


}