using UnityEngine;
using UnityEngine.InputSystem;

public class MB_Camera : MonoBehaviour
{
    InputAction CameraMoveAction;
    InputAction CameraZoomAction;
    [SerializeField] Transform Viewport;
    bool playerIsControllingCamera = true;

    Vector2 mousePositionAtStart;
    private void Awake()
    {
        CameraMoveAction = InputSystem.actions.FindAction("Camera");
        CameraMoveAction.started += CameraControl;

        CameraZoomAction = InputSystem.actions.FindAction("Zoom");
        CameraZoomAction.performed += CameraZoom;

    }


    // Update is called once per frame
    void Update()
    {
        playerIsControllingCamera = (CameraMoveAction.ReadValue<float>() > 0.5f);

        if (playerIsControllingCamera)
        {
            Vector2 cameraMotion = (mousePositionAtStart - (Vector2)Input.mousePosition) / 100;
            mousePositionAtStart = (Vector2)Input.mousePosition;
            Viewport.localPosition += new Vector3(cameraMotion.x, 0, cameraMotion.y);

        }



    }

    void CameraControl(InputAction.CallbackContext context)
    {
        mousePositionAtStart = Input.mousePosition;
    }

    void CameraZoom(InputAction.CallbackContext context)
    {
        Viewport.localPosition += (Viewport.rotation * new Vector3(0, 0, CameraZoomAction.ReadValue<Vector2>().y)) / 2;
    }

    private void OnDisable()
    {
        CameraMoveAction.started -= CameraControl;
    }
}