using UnityEngine;

public class TEST_SelectStateCheck : MonoBehaviour
{
    [SerializeField] TMPro.TMP_Text text;
    void Start()
    {
        
    }

    void Update()
    {
        text.text = GF_PlayerInput.selectState.ToString();
    }
}
