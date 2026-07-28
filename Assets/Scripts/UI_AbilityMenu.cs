using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


public class UI_AbilityMenu : MonoBehaviour
{
    [SerializeField] Transform ContentHolder;
    [SerializeField] GameObject AbilityPrefab;
    [SerializeField] SO_ActorEvents ActorEvents;
    [SerializeField] GameObject Sidebar;

    [SerializeField] SO_User Player;

    private E_ActionType typeToDisplay = E_ActionType.main;
    private List<GameObject> displayedAbilties = new List<GameObject>();

    public void Awake()
    {
        ActorEvents.DisplayAbilities.AddListener(LoadAbilities);
        ActorEvents.HideAbilities.AddListener(UnloadAbilities);

        SO_BattleEvents.EventPotentialTriggersChanged += LoadTriggers;
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

        List<CS_Ability> abilities = turn.actor.abilities.Values.ToList();
        foreach (CS_Ability ability in abilities)
        {
            if (ability.Type != typeToDisplay)
            { continue; }

            if (turn.abilityTagRestrict != null && !ability.Tags.Contains(turn.abilityTagRestrict))
            { continue; }

            GameObject obj = Instantiate(AbilityPrefab, ContentHolder);

            Button abilityButton = obj.GetComponent<Button>();
            abilityButton.onClick.AddListener(delegate { PlayerInputInterpreter.SelectingAbility(ability, turn); });




            MB_AbilityItem abilityInstance = obj.GetComponent<MB_AbilityItem>();
            abilityInstance.Ability = ability;
            abilityInstance.Actor = turn.actor;

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

    public void SetAbilityType(SO_ActionType newType)
    {

        typeToDisplay = newType.actionType;
        if (Player.activeTurn != null)
        {
            MB_PlayerInput.Instance.SetSelectState(E_SelectState.SelectingAbility);
        }

    }

    //I have some issues with the relationship between this UI and SO_BattleEvents; BattleEvents contains an event that is and will proably only be 
    //invoked here, what this connection should be is a serialized field on this Monobehavior combined with passing the MB reference to BattleEvents on enable.

    public void LoadTriggers()
    {

        ClearAbilities();

        
        ToggleSidebar(true);

       //List<CS_Ability> abilities = actor.abilities.Values.ToList();
        foreach (AwaitTrigger trigger in SO_BattleEvents.triggers)
        {

            /*if (turn.abilityTagRestrict != null && !ability.Tags.Contains(turn.abilityTagRestrict))
            { continue; }*/

            GameObject obj = Instantiate(AbilityPrefab, ContentHolder);

            Button abilityButton = obj.GetComponent<Button>();
            abilityButton.onClick.AddListener(delegate { trigger.OnUserActionCompleted(true); UnloadAbilities(); });




            MB_AbilityItem abilityInstance = obj.GetComponent<MB_AbilityItem>();
            abilityInstance.Ability = trigger.ability;
            abilityInstance.Actor = trigger.user;

            abilityInstance.UpdateText();

            displayedAbilties.Add(obj);
        }
    }
}
