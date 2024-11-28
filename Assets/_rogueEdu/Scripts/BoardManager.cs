using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BoardManager : MonoBehaviour
{
    public class CellData {
        public bool passable;
        public CellObject containedObject;
    }

    private Tilemap m_Tilemap;
    private Grid m_Grid;

    private List<Vector2Int> m_EmptyCellList;


    public int width;
    public int height;

    public Tile[] groundTiles;
    public Tile[] wallTiles;
    public Tile[] foodTiles;

    private CellData[,] m_BoardData;

    public FoodObject[] foodPrefabs;
    public WallObject[] wallPrefabs;
    public EnemyObject[] enemyPrefabs;
    public ExitCellObject exitCellPrefab;

    public Vector2Int foodCountMinMax;
    public int enemyCount = 1;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Init()
    {
        m_Tilemap = GetComponentInChildren<Tilemap>();
        m_Grid = GetComponentInChildren<Grid>();

        m_BoardData = new CellData[width, height];
        m_EmptyCellList = new List<Vector2Int>();

        for (int j = 0; j < height; j++)
        {
            for (int i = 0; i < width; i++) 
            {
                Tile tile;
                m_BoardData[i, j] = new CellData();

                if (i == 0 || j == 0 || i == width - 1 || j == height - 1)
                {
                    tile = wallTiles[Random.Range(0, wallTiles.Length)];
                    m_BoardData[i, j].passable = false;
                }
                else
                {
                    tile = groundTiles[Random.Range(0, groundTiles.Length)];
                    m_BoardData[i, j].passable = true;

                    m_EmptyCellList.Add(new Vector2Int(i, j));
                }

                m_Tilemap.SetTile(new Vector3Int(i, j, 0), tile);
            }
        }

        m_EmptyCellList.Remove(new Vector2Int(1, 1));

        Vector2Int endCoord = new Vector2Int(width - 2, height - 2);
        AddObject(Instantiate(exitCellPrefab), endCoord);
        m_EmptyCellList.Remove(endCoord);

        GenerateWall();
        GenerateFood();
        GenerateEnemy();
    }

    public Vector3 CellToWorld(Vector2Int cellIndex)
    {
        return m_Grid.GetCellCenterWorld((Vector3Int)cellIndex);
    }

    public CellData GetCellData(Vector2Int cellIndex)
    {
        if (cellIndex.x < 0 || cellIndex.x >= width || cellIndex.y < 0 || cellIndex.y >= height)
        {
            return null;
        }

        return m_BoardData[cellIndex.x, cellIndex.y];
    }

    void GenerateFood() {
        int foodCount = Random.Range(foodCountMinMax.x, foodCountMinMax.y+1);

        for (int i = 0; i < foodCount; i++) {
            int randomIndex = Random.Range(0, m_EmptyCellList.Count);
            Vector2Int coord = m_EmptyCellList[randomIndex];

            m_EmptyCellList.RemoveAt(randomIndex);

            int foodPrefabInd = Random.Range(0, foodPrefabs.Length);
            FoodObject newFood = Instantiate(foodPrefabs[foodPrefabInd]);

            AddObject(newFood, coord);
        }
    }

    private void Update()
    {

    }

    void GenerateWall() {
        int wallCount = Random.Range(6, 10);
        for (int i = 0; i < wallCount; i++) {
            int randomIndex = Random.Range(0, m_EmptyCellList.Count);
            Vector2Int coord = m_EmptyCellList[randomIndex];

            m_EmptyCellList.RemoveAt(randomIndex);

            int wallPrefabInd = Random.Range(0, wallPrefabs.Length);
            WallObject newWall = Instantiate(wallPrefabs[wallPrefabInd]);

            AddObject(newWall, coord);
        }
    }

    void GenerateEnemy() {
        for (int i = 0; i < enemyCount; i++) {
            int randomIndex = Random.Range(0, m_EmptyCellList.Count);
            Vector2Int coord = m_EmptyCellList[randomIndex];

            m_EmptyCellList.RemoveAt(randomIndex);

            int enemyPrefabInd = Random.Range(0, enemyPrefabs.Length);
            EnemyObject newEnemy = Instantiate(enemyPrefabs[enemyPrefabInd]);

            AddObject(newEnemy, coord);
        }
    }

    public void SetCellTile(Vector2Int cellIndex, Tile tile) {
        m_Tilemap.SetTile(
            new Vector3Int(cellIndex.x, cellIndex.y, 0),
            tile
            );
    }

    public Tile GetCellTile(Vector2Int cellIndex) {
        return m_Tilemap.GetTile<Tile>(
            new Vector3Int(cellIndex.x, cellIndex.y)
            );
    }

    void AddObject(CellObject obj, Vector2Int coord) {
        CellData data = m_BoardData[coord.x, coord.y];
        obj.transform.position = CellToWorld(coord);
        data.containedObject = obj;
        obj.Init(coord);
    }

    public void Clean() {
        if (m_BoardData == null) {
            return;
        }

        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                var cellData = m_BoardData[x, y];

                if (cellData.containedObject != null) {
                    Destroy(cellData.containedObject.gameObject);
                }

                SetCellTile(new Vector2Int(x, y), null);
            }
        }
    }
}
