using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridGenerator : MonoBehaviour
{
    public Transform gridParent;
    public GridLayoutGroup gridLayout;
    public int width;
    public int height;
    public int mines;

    public Tile[,] tiles;
    public GameState state = GameState.IDLE;
    public int discoveredTiles;

    public static GridGenerator instance;
    void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of GridGenerator found!");
            return;
        }
        instance = this;
    }

    private void Start()
    {
        StartNewGame();
    }

    public void CreateNewGrid()
    {
        GenerateMinesGrid();
        GenerateNumbersGrid();
        CounterManager.instance.Reset();
    }

    public void StartNewGame()
    {
        state = GameState.IDLE;
        discoveredTiles = 0;
        gridLayout.constraintCount = width;
        CreateGridUI();
        CreateNewGrid();
    }

    private void CreateGridUI()
    {
        foreach (Transform child in gridLayout.transform)
            Destroy(child.gameObject);

        tiles = new Tile[height, width];
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                GameObject go = Instantiate(Resources.Load<GameObject>("Prefabs/Tile"), gridLayout.transform);
                go.GetComponent<Tile>().Setup(j, i);
            }
        }
    }

    private void GenerateMinesGrid()
    {
        if (mines > width * height)
            throw new Exception("Too many mines!");

        foreach (Tile tile in tiles)
            tile.mine = false;

        System.Random rand = new System.Random();
        for (int i = 0; i < mines; i++)
        {
            while (true)
            {
                int x = rand.Next(width);
                int y = rand.Next(height);
                if (!tiles[y, x].mine)
                {
                    tiles[y, x].mine = true;
                    break;
                }
            }
        }
    }

    public List<Tile> GetNeighborTiles(int x, int y)
    {
        List<Tile> neighborTiles = new List<Tile>();

        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (j == 0 && i == 0)
                    continue;

                int neigbor_x = x + i;
                int neigbor_y = y + j;

                if (neigbor_x > -1 && neigbor_x < width && neigbor_y > -1 && neigbor_y < height)
                    neighborTiles.Add(tiles[neigbor_y, neigbor_x]);
            }
        }

        return neighborTiles;
    }

    private void GenerateNumbersGrid()
    {
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                int number = 0;

                foreach (Tile tile in GetNeighborTiles(j, i))
                {
                    if (tile.mine)
                        number++;
                }
                tiles[i, j].number = number;
            }
        }
    }

    public void DiscoverAllCells(int x, int y)
    {
        // Discover all tiles around one cell that can be discovered at once
        Tile _tile = tiles[y, x];
        if (!_tile.discovered)
        {
            List<Tile> to_check = new List<Tile>{ _tile };
            List<Tile> checked_tiles = new List<Tile>();
            List<Tile> to_discover = new List<Tile>();

            while (to_check.Count > 0)
            {
                Tile tile = to_check[0];

                if (tile.number == 0)
                {
                    foreach (Tile neighbor_tile in GetNeighborTiles(tile.x, tile.y))
                    {
                        if (!to_check.Contains(neighbor_tile) && !checked_tiles.Contains(neighbor_tile))
                            to_check.Add(neighbor_tile);
                    }
                }

                to_discover.Add(tile);
                to_check.RemoveAt(0);
                checked_tiles.Add(tile);
            }

            Tile lostTile = null;
            foreach (Tile tile in to_discover)
            {
                tile.Discover();
                if (tile.mine)
                    lostTile = tile;
            }

            if (lostTile != null)
                LoseGame(lostTile);
            else if (width * height - discoveredTiles == mines)
                WinGame();
        }
    }

    private void LoseGame(Tile lostTile)
    {
        foreach (Tile tile in tiles)
        {
            if (tile.flagged && !tile.mine)
                tile.GetComponent<Image>().sprite = Resources.Load<Sprite>($"Images/fake_mine");
            else if (tile.mine)
                tile.GetComponent<Image>().sprite = Resources.Load<Sprite>($"Images/mine");
        }
        lostTile.GetComponent<Image>().sprite = Resources.Load<Sprite>($"Images/red_mine");
        state = GameState.LOST;
        print($"Game lost in {CounterManager.instance.time} seconds!");
    }

    private void WinGame()
    {
        state = GameState.WON;
        print($"Game won in {CounterManager.instance.time} seconds!");
    }
}

public enum GameState
{
    WON,
    LOST,
    RUNNING,
    IDLE
}
