using System.Collections.Generic;
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

    
    private InputAction selectAction;
    private Vector3Int currentTileMouseOver;
    private bool playerInputEnabled;

    private void OnEnable()
    {
        SO_TurnManager.Instance.EventActivateUser += EnablePlayerInteraction;
    }

    void Start()
    {
        selectAction = InputSystem.actions.FindAction("Select");
        selectAction.performed += TileSelect;
    }

    void EnablePlayerInteraction(SO_User player)
    {
        if (player == Player)
        {
            playerInputEnabled = true;
        }
        else
        {
            playerInputEnabled = false;
            illustrator.line.enabled = false;
        }
            
    }


    void Update()
    {
        if (!playerInputEnabled)
            return;

        Vector3 mousePosition = MapPositionFromMouse(SceneCamera);
        currentTileMouseOver = Map.WorldToCell(mousePosition);


        Vector2Int TwoDTile = new Vector2Int(currentTileMouseOver.x, currentTileMouseOver.z);
        Tile tile = GridData.GetTile(TwoDTile);
        if (tile != null && Player.activeTurn != null)
        {
            illustrator.line.enabled = true;
            List<Tile> pathToDraw = new List<Tile> { Player.activeTurn.actor.currentTile };
            pathToDraw.AddRange(CS_GridUtility.FindShortestPath(tile, pathToDraw[0]));
            illustrator.IllustratePath(pathToDraw);
        }

        //Debug.Log(currentTileMouseOver);
    }

    void TileSelect(InputAction.CallbackContext context)
    {
        //Block input if it's not your turn and player doesn't have an owned turn
        if(!playerInputEnabled)
            return;

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
    [SerializeField] MB_PathIllustrator illustrator;
}
