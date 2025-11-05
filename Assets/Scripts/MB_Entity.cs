using UnityEngine;

public class MB_Entity : MonoBehaviour
{
    [SerializeField] protected SO_GridSystem gridSystem;
    public int stamina = 10;
    public int X = 0;
    public int Y = 0;

    private void Start()
    {
        AddToWorld();
    }


    //Entity Position
    private void AddToWorld()
    {
        gridSystem.GridAdd(this);
        Vector3 newPos = new Vector3(X, 0, Y);
        transform.position = newPos;
    }

    protected void UpdatePosition(Vector2Int lastPos)
    {
        gridSystem.GridUpdatePos(this, lastPos);
        Vector3 newPos = new Vector3(X, 0, Y);
        transform.position = newPos;
    }

    public virtual void TakeForcedMovement()
    {
        return;
    }


    //Entity Stamina
    public void TakeDamage(int damage)
    {
        stamina -= damage;
    }
}
