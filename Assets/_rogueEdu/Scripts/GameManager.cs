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

    [SerializeField] private int m_StartFoodAmount = 10;
    [SerializeField] private int m_FoodAmount;

    private int m_CurrentLevel;


    private VisualElement m_GameOverPanel;
    private Label m_GameOverMessage;

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
        TurnManager = new TurnManager();
        TurnManager.OnTick += OnTurnHappen;

        
        m_FoodLabel = UIDoc.rootVisualElement.Q<Label>("FoodLabel");
        m_GameOverPanel = UIDoc.rootVisualElement.Q<VisualElement>("GameOverPanel");
        m_GameOverMessage = m_GameOverPanel.Q<Label>("GameOverMessage");

        StartNewGame();
    }

    void OnTurnHappen() {
        ChangeFood(-1);
    }


    public void ChangeFood(int amount)
    {
        m_FoodAmount += amount;
        m_FoodLabel.text = "Food : " + m_FoodAmount;

        if (m_FoodAmount <= 0)
        {
            playerController.GameOver();
            m_GameOverPanel.style.visibility = Visibility.Visible;
            m_GameOverMessage.text = "Game Over!\n\nSurvived " + m_CurrentLevel + " days";
        }
    }

    public void NewLevel() { 
        boardManager.Clean();
        boardManager.Init();

        playerController.Spawn(boardManager, new Vector2Int(1, 1));

        m_CurrentLevel++;
    }

    public void StartNewGame() {
        m_GameOverPanel.style.visibility = Visibility.Hidden;

        m_FoodAmount = m_StartFoodAmount;
        m_CurrentLevel = 1;
        m_FoodLabel.text = "Food : " + m_FoodAmount;

        boardManager.Clean();
        boardManager.Init();

        playerController.Init();
        playerController.Spawn(boardManager, new Vector2Int(1, 1));
    }

}
