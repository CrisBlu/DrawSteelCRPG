using UnityEngine;

public class MBEntity : MonoBehaviour
{
    [SerializeField] MBGridSystem gridSystem;
    private int stamina = 1;
    public int X = 0;
    public int Y = 0;

    void Start()
    {
        gridSystem.AddToGrid(this);
        Vector3 newPos = new Vector3(X, 0, Y);
        transform.position = newPos;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected void UpdatePosition(Vector2Int lastPos)
    {
        gridSystem.UpdateOnGrid(this, lastPos);
        Vector3 newPos = new Vector3 (X, 0 , Y);
        transform.position = newPos;
    }
}
