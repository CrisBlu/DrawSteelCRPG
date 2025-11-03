using UnityEngine;

public class MBEntity : MonoBehaviour
{
    [SerializeField] protected MBGridSystem gridSystem;
    public int stamina = 10;
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

    public void TakeDamage(int damage)
    {
        stamina -= damage;
    }
}
