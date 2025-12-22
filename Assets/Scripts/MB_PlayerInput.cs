using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;


//Used for getting player input, from clicks and mousing over elements
//Also will potentially be used to manage Input Interpreter, whether in or out of combat

//This is a MB because it needs start and update
public class MB_PlayerInput : MonoBehaviour
{
    [SerializeField] Camera SceneCamera;
    [SerializeField] Grid Map;
    [SerializeField] SO_User Player;
    [SerializeField] SO_ZipperInit Zipper;

    
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
        //Debug.Log(currentTileMouseOver);
    }

    void TileSelect(InputAction.CallbackContext context)
    {
        //Block input if it's not your turn
        if(Zipper.activeUser != Player)
        {
            return;
        }

        Vector2Int TwoDTile = new Vector2Int(currentTileMouseOver.x, currentTileMouseOver.z);
        PlayerInputInterpreter.ProcessInput(GridData.GetTile(TwoDTile), Player, TurnManager);
    }


    
    public Vector3 MapPositionFromMouse(Camera sceneCamera)
    {
        Vector3 mousePos = Input.mousePosition;
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            mousePos.z = sceneCamera.nearClipPlane;
            Ray ray = sceneCamera.ScreenPointToRay(mousePos);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100))
            {
                return hit.point;
            }
        }
      
        return new Vector3(999, 999, 999);
    }


    //Things that do not belong here but are here just for the time being
    //Probably belongs localized in some sort of battle manager
    [SerializeField] SO_TurnManager TurnManager;
    [SerializeField] SO_GridData GridData;
}
