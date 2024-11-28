using UnityEngine;

public class EnemyObject : CellObject
{
    public int health = 3;

    private int m_currentHealth;

    private void Awake()
    {
        GameManager.Instance.TurnManager.OnTick += TurnHappened;
    }

    private void OnDestroy()
    {
        GameManager.Instance.TurnManager.OnTick -= TurnHappened;
    }

    public override void Init(Vector2Int coord)
    {
        base.Init(coord);
        m_currentHealth = health;
    }

    public override bool PlayerWantsToEnter()
    {
        m_currentHealth -= 1;
        if (m_currentHealth <= 0) {
            Destroy(gameObject);
        }

        return false;
    }

    bool MoveTo(Vector2Int coord)
    {
        var board = GameManager.Instance.boardManager;
        var targetCell = board.GetCellData(coord);

        if (targetCell == null
            || !targetCell.passable
            || targetCell.containedObject != null
            )
        {
            return false;
        }

        var currentCell = board.GetCellData(m_Cell);

        currentCell.containedObject = null;

        targetCell.containedObject = this;
        m_Cell = coord;
        transform.position = board.CellToWorld(coord);

        return true;
    }

    void TurnHappened() {
        var playerCell = GameManager.Instance.playerController.cell;

        int xDist = playerCell.x - m_Cell.x;
        int yDist = playerCell.y - m_Cell.y;

        int absXDist = Mathf.Abs(xDist);
        int absYDist = Mathf.Abs(yDist);

        if (false
            || (xDist == 0 && absYDist == 1)
            || (yDist == 0 && absXDist == 1)
            )
        {
            GameManager.Instance.ChangeFood(-3);
        }
        else {
            if (absXDist > absYDist)
            {
                if (!TryMoveInX(xDist))
                {
                    TryMoveInY(yDist);
                }
            }
            else {
                if (!TryMoveInY(yDist))
                {
                    TryMoveInX(xDist);
                }
            }
        }
    }

    bool TryMoveInX(int xDist)
    {
        if (xDist > 0) {
            return MoveTo(m_Cell + Vector2Int.right);
        }

        return MoveTo(m_Cell + Vector2Int.left);
    }

    bool TryMoveInY(int yDist)
    {
        if (yDist > 0) {
            return MoveTo(m_Cell + Vector2Int.up);
        }

        return MoveTo(m_Cell + Vector2Int.down);
    }
}