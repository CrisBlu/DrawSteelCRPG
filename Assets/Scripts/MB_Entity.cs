using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

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

    public bool UpdatePosition(Tile newTile)
    {
       
        if(gridData.AddToGrid(newTile, this))
        {
            //If GridData tells us that our desired tile is empty
            currentTile.entity = null;
            currentTile = newTile;
            transform.position = new Vector3(newTile.position.x, 0, newTile.position.y);

            return true;
        }

        return false;
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
