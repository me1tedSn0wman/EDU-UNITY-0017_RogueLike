using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public BoardManager boardManager;
    public PlayerController playerController;

    public TurnManager TurnManager { get; private set; }

    public UIDocument UIDoc;
    private Label m_FoodLabel;

    private int m_FoodAmount = 100;

    public void Awake()
    {
        if (Instance != null) { 
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_FoodLabel = UIDoc.rootVisualElement.Q<Label>("FoodLabel");
        m_FoodLabel.text = "Food : " + m_FoodAmount;

        TurnManager = new TurnManager();
        TurnManager.OnTick += OnTurnHappen;

        boardManager.Init();
        playerController.Spawn(boardManager, new Vector2Int(1, 1));
    }

    void OnTurnHappen() {
        ChangeFood(-1);
    }


    public void ChangeFood(int amount)
    {
        m_FoodAmount += amount;
        m_FoodLabel.text = "Food : " + m_FoodAmount;
    }

}
