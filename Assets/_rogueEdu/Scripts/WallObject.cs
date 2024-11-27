using UnityEngine;
using UnityEngine.Tilemaps;

public class WallObject : CellObject
{
    public Tile obstacleTile;
    public Tile damagedTile;
    public int maxHealth = 3;


    [Header("Set Dynamically")]
    [SerializeField] protected int health;
    public Tile originalTile;


    public override void Init(Vector2Int cell) {
        base.Init(cell);

        health = maxHealth;
        originalTile = GameManager.Instance.boardManager.GetCellTile(cell);
        GameManager.Instance.boardManager.SetCellTile(cell, obstacleTile);
    }

    public void ReplaceTile() { }

    public override bool PlayerWantsToEnter()
    {
        health -= 1;


        if (health > 0) {
            if (health == 1) {
                GameManager.Instance.boardManager.SetCellTile(m_Cell, damagedTile);
            }

            return false;
        }
        GameManager.Instance.boardManager.SetCellTile(m_Cell, originalTile);
        Destroy(gameObject);
        return true;
        
    }
}