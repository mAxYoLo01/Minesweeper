using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Tile : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler, IDragHandler
{
    public int x;
    public int y;
    public int number;
    public bool mine = false;
    public bool flagged = false;
    public bool discovered = false;

    private List<Tile> neighborImagesNotDiscovered = new List<Tile>();
    private bool hovered;

    public void Setup(int x, int y)
    {
        this.x = x;
        this.y = y;
        name = $"{x}, {y}";
        GridGenerator.instance.tiles[y, x] = this;
        GetComponent<Image>().sprite = Resources.Load<Sprite>($"Images/undiscovered");
    }

    public void Discover()
    {
        if (!discovered)
        {
            discovered = true;
            GridGenerator.instance.discoveredTiles++;
            if (mine)
                GetComponent<Image>().sprite = Resources.Load<Sprite>($"Images/mine");
            else
                GetComponent<Image>().sprite = Resources.Load<Sprite>($"Images/{number}");
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (GridGenerator.instance.state == GameState.RUNNING)
        {
            // Flag the cell
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                if (!discovered)
                {
                    if (flagged)
                    {
                        GetComponent<Image>().sprite = Resources.Load<Sprite>($"Images/undiscovered");
                        CounterManager.instance.mine++;
                    } else
                    {
                        GetComponent<Image>().sprite = Resources.Load<Sprite>($"Images/flag");
                        CounterManager.instance.mine--;
                    }
                    CounterManager.instance.SetMineUI();
                    flagged = !flagged;
                }
            }
            // Indicate which neighbors are not discovered and not flagged
            else if (eventData.button == PointerEventData.InputButton.Left)
            {
                if (number != 0 && discovered)
                {
                    neighborImagesNotDiscovered = new List<Tile>();
                    foreach (Tile neighbor in GridGenerator.instance.GetNeighborTiles(x, y))
                    {
                        if (!neighbor.discovered && !neighbor.flagged)
                        {
                            neighborImagesNotDiscovered.Add(neighbor);
                            neighbor.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/0");
                        }
                    }
                }
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // Set tile back to undiscovered if leaving tile
        if (number != 0)
        {
            foreach (Tile neighbor in neighborImagesNotDiscovered)
                neighbor.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/undiscovered");
        }
        // Check if the pointer has changed from a tile to another one
        if (hovered)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                // Initialize grid until the first click is not on a mine (ran only once per game)
                if (GridGenerator.instance.state == GameState.IDLE)
                {
                    while (number != 0 || mine)
                        GridGenerator.instance.CreateNewGrid();

                    GridGenerator.instance.state = GameState.RUNNING;
                }

                if (!flagged && GridGenerator.instance.state == GameState.RUNNING)
                {
                    // Discover the clicked cell
                    if (!discovered)
                        GridGenerator.instance.DiscoverAllCells(x, y);
                    else
                    {
                        // Discover all neighbor cells if the number of flagged cells is the good one
                        int num_flagged = 0;
                        List<Tile> neighbors = GridGenerator.instance.GetNeighborTiles(x, y);
                        foreach (Tile tile in neighbors)
                            if (tile.flagged)
                                num_flagged++;

                        if (num_flagged == number)
                        {
                            foreach (Tile tile in neighbors)
                                if (!tile.flagged)
                                    GridGenerator.instance.DiscoverAllCells(tile.x, tile.y);
                        }
                    }
                }
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Change the pointer if another tile is entered before releasing the button
        hovered = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hovered = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Needs to be here even empty to make sure that the drag is functioning
    }
}
