using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private bool m_IsGameOver;

    private BoardManager m_Board;
    private Vector2Int m_CellPosition;

    public void Init()
    {
        m_IsGameOver = false;
    }

    public void Spawn(BoardManager boardManager, Vector2Int cell) {
        m_IsGameOver = false;
        m_Board = boardManager;

        MoveTo(cell);
    }

    public void MoveTo(Vector2Int cell)
    { 
        m_CellPosition = cell;
        transform.position = m_Board.CellToWorld(m_CellPosition);
    }

    public void GameOver() {
        m_IsGameOver = true;
    }

    private void Update()
    {
        if (Keyboard.current.enterKey.wasPressedThisFrame && m_IsGameOver)
        {
            GameManager.Instance.StartNewGame();
        }

        if (m_IsGameOver) {
            return;
        }
        Vector2Int newCellTarget = m_CellPosition;
        bool hasMoved = false;

        if (Keyboard.current.upArrowKey.wasPressedThisFrame)
        {
            newCellTarget.y += 1;
            hasMoved = true;
        }
        else if (Keyboard.current.downArrowKey.wasPressedThisFrame)
        {
            newCellTarget.y -= 1;
            hasMoved = true;
        }
        else if (Keyboard.current.rightArrowKey.wasPressedThisFrame)
        {
            newCellTarget.x += 1;
            hasMoved = true;
        }
        else if (Keyboard.current.leftArrowKey.wasPressedThisFrame)
        {
            newCellTarget.x -= 1;
            hasMoved = true;
        }



        if (hasMoved)
        {
            BoardManager.CellData cellData = m_Board.GetCellData(newCellTarget);

            if (cellData != null && cellData.passable) {
                GameManager.Instance.TurnManager.Tick();

                if (cellData.containedObject == null)
                {
                    MoveTo(newCellTarget);
                }
                else if (cellData.containedObject.PlayerWantsToEnter()) {
                    MoveTo(newCellTarget);
                    cellData.containedObject.PlayerEntered();
                }
            }
        }
    }
}
