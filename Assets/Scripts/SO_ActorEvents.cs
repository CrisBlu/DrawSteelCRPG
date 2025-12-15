using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "SO_ActorEvents", menuName = "Scriptable Objects/SO_ActorEvents")]
public class SO_ActorEvents : ScriptableObject //A serializable class that allows me to link events from actors to other monobehaviors without directly linking them in scene
{
    public UnityEvent<TurnData> DisplayAbilities;
    public UnityEvent HideAbilities;


    public void TriggerDisplayAbilities(TurnData turn)
    {
        DisplayAbilities.Invoke(turn);
    }

    public void TriggerHideAbilities()
    {
        HideAbilities.Invoke();
    }

    private void OnDisable()
    {
        DisplayAbilities?.RemoveAllListeners();
        HideAbilities?.RemoveAllListeners();
    }
}
