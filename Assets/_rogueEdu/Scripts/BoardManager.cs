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
    public Vector2Int foodCountMinMax;

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

        GenerateWall();
        GenerateFood();
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
            CellData data = m_BoardData[coord.x, coord.y];

            int foodPrefabInd = Random.Range(0, foodPrefabs.Length);

            FoodObject newFood = Instantiate(foodPrefabs[foodPrefabInd]);

            newFood.transform.position = CellToWorld(coord);
            data.containedObject = newFood;
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
            CellData data = m_BoardData[coord.x, coord.y];

            int wallPrefabInd = Random.Range(0, wallPrefabs.Length);
            WallObject newWall = Instantiate(wallPrefabs[wallPrefabInd]);

            newWall.transform.position = CellToWorld(coord);

            data.containedObject = newWall;
        }
    }
}
