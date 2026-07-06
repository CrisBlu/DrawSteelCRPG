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
    public static bool inputEnabled;

    //Experimental
    //Add a field for valid input list
    public static AwaitTile inputRequest = null;
    /*public static void AddToInputList(AwaitTile input)
    {
        inputRequests.Add(input);
    }*/

    //public static E_TurnState _turnState;

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
            inputEnabled = true;
        }
        else
        {
            inputEnabled = false;
        }
            
    }



    void Update()
    {
        if (!inputEnabled)
        {
            illustrator.line.enabled = false;
            return;
        }

            

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
        if(!inputEnabled)
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

    /*List<Tile> validTiles;
    public E_TurnState turnState
    {
        get { return _turnState; }


        set
        {
            //Exit
            switch (_turnState)
            {
                case E_TurnState.SelectingMove:

                    CS_ColorGrid.ClearGridColors(actor.currentTile.parentGrid);
                    validTiles.Clear();


                    break;

                case E_TurnState.SelectingAbility:
                    actor.HideAbilities();

                    break;

                case E_TurnState.UsingAbility:
                    usingAbility = null;
                    CS_ColorGrid.ClearGridColors(actor.currentTile.parentGrid);
                    validTiles.Clear();

                    break;

                case E_TurnState.ResolvingAbility:
                    CS_ColorGrid.ClearGridColors(actor.currentTile.parentGrid);
                    validTiles.Clear();
                    break;

                case E_TurnState.HoldingForAnimation:
                    break;
            }

            _turnState = value;

            //Enter
            switch (_turnState)
            {
                case E_TurnState.SelectingMove:


                    validTiles = CS_GridUtility.GetWalkableTilesFromOrigin(actor.currentTile, actions[E_ActionType.move], false);
                    if (validTiles.Count != 0)
                    {
                        Color green = new Color(0, 1, 0, .25f);
                        CS_ColorGrid.ColorCells(validTiles, green);
                    }


                    break;

                case E_TurnState.SelectingAbility:
                    actor.DisplayAbilties(this);
                    break;

                case E_TurnState.UsingAbility:



                    CS_AbilityTargetingData targetOutput = usingAbility.Target(actor.currentTile);

                    if (targetOutput != null)
                    {
                        validTiles = targetOutput.validTargets;
                        CS_ColorGrid.ColorCells(targetOutput.validArea, Color.red);
                    }


                    break;

                case E_TurnState.ResolvingAbility:
                    validTiles = AbilityHandler.currentCallback.validTiles;
                    CS_ColorGrid.ColorCells(validTiles, Color.blue);
                    break;

                case E_TurnState.HoldingForAnimation:
                    CS_ColorGrid.ClearGridColors(actor.currentTile.parentGrid);
                    break;
            }

            if (TurnController.AI && _turnState != E_TurnState.HoldingForAnimation)
                TurnManager.EventNotifyAI.Invoke();


        }

    }*/




    //Things that do not belong here but are here just for the time being
    //Probably belongs localized in some sort of battle manager
    [SerializeField] SO_TurnManager TurnManager;
    [SerializeField] SO_GridData GridData;
    [SerializeField] MB_PathIllustrator illustrator;
}
