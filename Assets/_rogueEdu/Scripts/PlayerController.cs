using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private bool m_IsGameOver;

    private BoardManager m_Board;
    private Vector2Int m_CellPosition;

    public float moveSpeed = 5.0f;
    private bool m_IsMoving;
    private bool m_IsAttacking;
    private float crntAttackTime;
    [SerializeField] private float timeAttack;

    private Vector3 m_MoveTarget;

    private Animator m_Animator;

    public Vector2Int cell
    {
        get { return m_CellPosition; }
    }

    private void Awake()
    {
        m_Animator = GetComponent<Animator>();
    }

    public void Init()
    {
        m_IsGameOver = false;
        m_IsMoving = false;
        crntAttackTime = 0;
    }

    public void Spawn(BoardManager boardManager, Vector2Int cell) {
        m_IsGameOver = false;
        m_Board = boardManager;

        MoveTo(cell, true);
    }

    public void MoveTo(Vector2Int cell, bool immediate)
    { 
        m_CellPosition = cell;

        if (immediate)
        {
            m_IsMoving = false;
            transform.position = m_Board.CellToWorld(m_CellPosition);
        }
        else {
            m_IsMoving = true;
            m_MoveTarget = m_Board.CellToWorld(m_CellPosition);
        }

        m_Animator.SetBool("Moving", m_IsMoving);
    }

    public void Attack() {
        crntAttackTime = 0;
        m_IsAttacking = true;
        m_Animator.SetBool("Attacking", m_IsAttacking);
    }

    public void GameOver() {
        m_IsGameOver = true;
    }

    private void Update()
    {
        if (m_IsGameOver) {

            if (Keyboard.current.enterKey.wasPressedThisFrame)
            {
                GameManager.Instance.StartNewGame();
            }
            return;
        }

        if (m_IsMoving) {
            transform.position = Vector3.MoveTowards(transform.position, m_MoveTarget, moveSpeed * Time.deltaTime);

            if (transform.position == m_MoveTarget) {
                m_IsMoving = false;
                m_Animator.SetBool("Moving", false);
                var cellData = m_Board.GetCellData(m_CellPosition);
                if (cellData.containedObject != null)
                {
                    cellData.containedObject.PlayerEntered();
                }
            }

            return;
        }

        if (m_IsAttacking) {
            crntAttackTime += Time.deltaTime;

            if (crntAttackTime > timeAttack)
            {
                m_IsAttacking = false;
                m_Animator.SetBool("Attacking", false);
            }

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
                    MoveTo(newCellTarget, false);
                }
                else if (cellData.containedObject.PlayerWantsToEnter())
                {
                    MoveTo(newCellTarget, false);
                }
                else {
                    Attack();
                }
            }
            return;
        }
    }
}
