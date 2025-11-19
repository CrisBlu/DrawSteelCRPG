using UnityEngine;

public class MB_Entity : MonoBehaviour
{
    public SO_GridSystem gridSystem;
    public int stamina = 10;
    public int X = 0;
    public int Y = 0;


    public Tile currentTile = null;

    protected virtual void Awake()
    {
       currentTile = AddToWorld();
    }


    //Entity Position
    private Tile AddToWorld()
    {
        Vector3 newPos = new Vector3(X, 0, Y);
        transform.position = newPos;

        return gridSystem.GridAdd(this);
    }

    protected void UpdatePosition(Vector2Int lastPos)
    {
        currentTile = gridSystem.GridUpdatePos(this, lastPos);
        Vector3 newPos = new Vector3(X, 0, Y);
        transform.position = newPos;
    }

    protected virtual void RemoveFromWorld()
    {
        gridSystem.GridRemove(this);
        gameObject.SetActive(false);
    }

    public virtual void ForcedMovement(Tile cellPushedInto, int distance)
    {
        TakeDamage(distance);
        return;
    }


    //Entity Stamina
    public virtual void TakeDamage(int damage)
    {
        stamina -= damage;

        if(stamina <= 0)
        {
            RemoveFromWorld();
        }

    }
}
