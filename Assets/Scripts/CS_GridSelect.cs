using UnityEngine;

public class CS_GridSelect
{
    public Vector3 GetSelectedMapPosition(Camera sceneCamera)
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
}
