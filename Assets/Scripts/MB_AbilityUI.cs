using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MB_AbilityUI : MonoBehaviour
{
    [SerializeField] Transform ContentHolder;
    [SerializeField] GameObject AbilityPrefab;
    [SerializeField] SO_ActorEvents ActorEvents;
    [SerializeField] GameObject Sidebar;

    private E_ActionType typeToDisplay = E_ActionType.move;
    private List<GameObject> displayedAbilties = new List<GameObject>();

    public void Awake()
    {
        ActorEvents.DisplayAbilities.AddListener(LoadAbilities);
        ActorEvents.HideAbilities.AddListener(UnloadAbilities);
    }

    public void LoadAbilities(TurnData turn)
    {
        
        ClearAbilities();

        //Hardcode, Monsters don't trigger this
        if (turn.actor.CompareTag("Monster"))
        {
            return;
        }
        ToggleSidebar(true);

        foreach (CS_Ability ability in turn.actor.abilities)
        {
            /*if (ability.Type != typeToDisplay)
            { continue; }*/

            if (turn.abilityTagRestrict != null && !ability.Tags.Contains(turn.abilityTagRestrict))
            { continue; }

            GameObject obj = Instantiate(AbilityPrefab, ContentHolder);

            Button abilityButton = obj.GetComponent<Button>();
            abilityButton.onClick.AddListener(delegate { PlayerInputInterpreter.SelectingAbility(ability, turn); });

            MB_AbilityItem abilityInstance = obj.GetComponent<MB_AbilityItem>();
            abilityInstance.Ability = ability;

            abilityInstance.UpdateText();

            displayedAbilties.Add(obj);
        }
    }

    public void UnloadAbilities()
    {
        ToggleSidebar(false);
    }

    public void ClearAbilities()
    {
        foreach (GameObject ability in displayedAbilties)
        {
            DestroyImmediate(ability);
        }
    }

    public void ToggleSidebar(bool state)
    {
        Sidebar.SetActive(state);
    }

    /*public void SetAbilityType(SO_AbilityType newType)
    {
        typeToDisplay = newType.actionType;
        if (BattleManager.activeActor != null)
        {
            LoadAbilities(BattleManager.temporaryReferenceToActiveActor);
        }

    }*/
}
