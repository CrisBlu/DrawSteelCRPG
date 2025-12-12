using UnityEngine;
using UnityEngine.InputSystem;


//Used for getting player input, from clicks and mousing over elements
//Also will potentially be used to manage Input Interpreter, whether in or out of combat
public class MB_PlayerInput : MonoBehaviour
{
    [SerializeField] Camera SceneCamera;
    [SerializeField] Grid Map;
    
    private InputAction selectAction;
    private Vector3Int currentTileMouseOver;

    void Start()
    {
        selectAction = InputSystem.actions.FindAction("Select");
        selectAction.performed += TileSelect;
    }


    void Update()
    {
        Vector3 mousePosition = MapPositionFromMouse(SceneCamera);
        currentTileMouseOver = Map.WorldToCell(mousePosition);
    }

    void TileSelect(InputAction.CallbackContext context)
    {
        
    }


    public Vector3 MapPositionFromMouse(Camera sceneCamera)
    {
        Vector3 mousePos = Input.mousePosition;

        mousePos.z = sceneCamera.nearClipPlane;
        Ray ray = sceneCamera.ScreenPointToRay(mousePos);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100))
        {
            return hit.point;
        }
        return new Vector3(999, 999, 999);
    }


    //Things that do not belong here but are here just for the time being
    [SerializeField] SO_TurnManager TurnManager;
}
