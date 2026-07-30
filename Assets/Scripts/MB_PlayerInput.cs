using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using static GF_PlayerInput;


//Used for getting player input, from clicks and mousing over elements
//Also will potentially be used to manage Input Interpreter, whether in or out of combat

//This is a MB because it needs start and update
public class MB_PlayerInput : MonoBehaviour
{
    [SerializeField] Camera SceneCamera;
    [SerializeField] Grid Map;
    [SerializeField] public SO_User Player;

    
    private InputAction selectAction;

    //Use GF_PlayerInput for these
    //public static bool inputEnabled;
    public static AwaitTile inputRequest = null;





    public static MB_PlayerInput Instance;


    private void OnEnable()
    {
        SO_TurnManager.Instance.EventActivateUser += StartPlayerTurn;
    }

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        selectAction = InputSystem.actions.FindAction("Select");
        selectAction.performed += TileSelect;
    }

    void StartPlayerTurn(SO_User player)
    {
        if (player == Player)
        {
            inputEnabled = true;
            selectState = E_SelectState.SelectingActor;
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

        /*if (mousePosition.x == 999)
        {
            illustrator.line.enabled = false;
            return;
        }*/
        Vector3Int mouseOverCoords = Map.WorldToCell(mousePosition);
        



        currentTileMouseOver = GridData.GetTile(new Vector2Int(mouseOverCoords.x, mouseOverCoords.z));


        if (currentTileMouseOver != null && selectState == E_SelectState.SelectingMove)
        {
            
            List<Tile> pathToDraw = new List<Tile> { Player.activeTurn.actor.currentTile };
            pathToDraw.AddRange(CS_GridUtility.FindShortestPath(currentTileMouseOver, pathToDraw[0]));
            if (pathToDraw.Count <= Player.activeTurn.actions[E_ActionType.move] + 1)
            {
                illustrator.line.enabled = true;
                illustrator.IllustratePath(pathToDraw);
            }
            else
            {
                illustrator.line.enabled = false;
            }

            
        }
        else if(selectState != E_SelectState.SelectingMove)
        {
            illustrator.line.enabled = false;
        }

        //Debug.Log(currentTileMouseOver);
    }

    void TileSelect(InputAction.CallbackContext context)
    {
        //Block input if it's not your turn and player doesn't have an owned turn
        if(!inputEnabled)
            return;

        PlayerInputInterpreter.ProcessInput(currentTileMouseOver, Player, TurnManager);
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


    public void SetSelectState(E_SelectState newState)
    {

            switch (selectState)
            {
                case E_SelectState.SelectingActor:

                    break;

                case E_SelectState.SelectingMove:

                    CS_ColorGrid.ClearGridColors(GridData);
                    Player.activeTurn.validTiles.Clear();


                    break;

                case E_SelectState.SelectingAbility:
                    Player.activeTurn.actor.HideAbilities();

                    break;

                case E_SelectState.UsingAbility:
                    Player.activeTurn.usingAbility = null;
                    CS_ColorGrid.ClearGridColors(GridData);
                    Player.activeTurn.validTiles.Clear();

                    break;

                case E_SelectState.ResolvingAbility:
                    CS_ColorGrid.ClearGridColors(GridData);
                    Player.activeTurn.validTiles.Clear();
                    break;

                case E_SelectState.HoldingForAnimation:
                    break;
            }

            selectState = newState;

            //Enter
            switch (selectState)
            {
                case E_SelectState.SelectingActor:

                    break;

                case E_SelectState.SelectingMove:


                    Player.activeTurn.validTiles = CS_GridUtility.GetWalkableTilesFromOrigin(Player.activeTurn.actor.currentTile, Player.activeTurn.actions[E_ActionType.move], false);
                    if (Player.activeTurn.validTiles.Count != 0)
                    {
                        Color green = new Color(0, 1, 0, .25f);
                        CS_ColorGrid.ColorCells(Player.activeTurn.validTiles, green);
                    }


                    break;

                case E_SelectState.SelectingAbility:
                    Player.activeTurn.actor.DisplayAbilties(Player.activeTurn);
                    break;

                case E_SelectState.UsingAbility:



                    /*CS_AbilityTargetingData targetOutput = usingAbility.Target(actor.currentTile);

                    if(targetOutput != null)
                    {
                        validTiles = targetOutput.validTargets;
                        CS_ColorGrid.ColorCells(targetOutput.validArea, Color.red);
                    }*/


                    break;


                case E_SelectState.HoldingForAnimation:
                    CS_ColorGrid.ClearGridColors(GridData);
                    break;
            }



    }

    




    //Things that do not belong here but are here just for the time being
    //Probably belongs localized in some sort of battle manager
    [SerializeField] SO_TurnManager TurnManager;
    [SerializeField] SO_GridData GridData;
    [SerializeField] MB_PathIllustrator illustrator;
}
