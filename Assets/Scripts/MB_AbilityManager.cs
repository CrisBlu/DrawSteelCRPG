using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MB_AbilityManager : MonoBehaviour
{
    [SerializeField] SO_BattleManager BattleManager;
    [SerializeField] Transform ContentHolder;
    [SerializeField] GameObject AbilityPrefab;

    private List<GameObject> displayedAbilties = new List<GameObject>();

    private void OnEnable()
    {
        BattleManager.EventActivateActor.AddListener(LoadAbilities);
    }

    public void LoadAbilities(MB_Actor actor)
    {
        foreach (GameObject ability in displayedAbilties)
        {
            DestroyImmediate(ability);
        }

        foreach(CS_Ability ability in actor.abilities)
        {
            GameObject obj = Instantiate(AbilityPrefab, ContentHolder);
            Button sad = obj.GetComponent<Button>();
            sad.onClick.AddListener(delegate { BattleManager.StartLookingForTarget(ability); });
            MB_Ability abilityInstance = obj.GetComponent<MB_Ability>();
            abilityInstance.Ability = ability;

            abilityInstance.UpdateText();

            displayedAbilties.Add(obj);
        }
    }
}
