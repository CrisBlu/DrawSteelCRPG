using UnityEngine;

public class MB_Entity : MonoBehaviour
{

    [SerializeField] protected SO_GridData gridData;
    [SerializeField] protected int stamina = 10;

    Vector2Int position;
    public Tile currentTile = null;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {
        currentTile = AddToWorld();
    }

    private Tile AddToWorld()
    {

        position = new Vector2Int((int)transform.position.x, (int)transform.position.z);

        transform.position = new Vector3(position.x, 0, position.y);


        //Validate in GridMatrix function
        Tile tile = gridData.GetTile(position);
        gridData.AddToGrid(tile, this);
        return tile;
    }

    public void UpdatePosition(Tile newTile)
    {
        currentTile.entity = null;
        transform.position = new Vector3(newTile.position.x, 0, newTile.position.y);
        currentTile = newTile;

        gridData.AddToGrid(currentTile, this);
    }

    public void TakeDamage(int damage)
    {
        stamina -= damage;

        if (stamina <= 0)
        {
            Debug.Log(gameObject.name + " is dead");
        }
    }


}
