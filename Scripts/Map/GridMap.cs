using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMap : MonoBehaviour
{
    public static GridMap instance;

    private int gridWidth;
    private int gridHeight;
    private Tile[,] gridData;
    private Tile[] mapTiles;

    public Tile[] MapTiles { get { return mapTiles; } }

    public static readonly Vector2Int[] compassDirections =
    {
        new Vector2Int(0, 1),
        new Vector2Int(1, 1),
        new Vector2Int(1, 0),
        new Vector2Int(1, -1),
        new Vector2Int(0, -1),
        new Vector2Int(-1, -1),
        new Vector2Int(-1, 0),
        new Vector2Int(-1, 1)
    };

    public static readonly Vector2Int[] cardinalDirections =
    {
        new Vector2Int(0, 1),
        new Vector2Int(1, 0),
        new Vector2Int(0, -1),
        new Vector2Int(-1, 0)
    };

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }

    private void Start()
    {
        mapTiles = GetComponentsInChildren<Tile>();

        foreach (Tile tile in mapTiles)
        {
            int x = tile.Position.x;
            int y = tile.Position.y;
            if (x > gridWidth)
            {
                gridWidth = x;
            }
            if (y > gridHeight)
            {
                gridHeight = y;
            }
        }
        gridWidth++;
        gridHeight++;

        gridData = new Tile[gridWidth, gridHeight];

        int i = 0;
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                gridData[x, y] = mapTiles[i];
                i++;
            }
        }

        for(int y = 0; y < gridHeight; y++)
        {
            for(int x = 0; x < gridWidth; x++)
            {
                if (gridData[x, y].IsWalkable)
                {
                    gridData[x, y].Neighbors = GetNeighbors(x, y);
                }
            }
        }
    }

    private List<Tile> GetNeighbors(int x, int y)
    {
        List<Tile> neighborTiles = new List<Tile>();

        foreach(Vector2Int direction in cardinalDirections)
        {
            int newX = x + direction.x;
            int newY = y + direction.y;

            if(IsWithinBounds(new Vector2Int(newX, newY)) && gridData[newX, newY] != null &&
                gridData[newX, newY].IsWalkable)
            {
                neighborTiles.Add(gridData[newX, newY]);
            }
        }
        return neighborTiles;
    }

    private bool IsWithinBounds(Vector2Int tilePosition)
    {
        if (tilePosition.x >= 0 && tilePosition.x < gridWidth && tilePosition.y >= 0 && tilePosition.y < gridHeight)
            return true;
        else
            return false;
    }

    public Tile GetTileAtPosition(Vector2Int tilePosition)
    {
        if (IsWithinBounds(tilePosition))
        {
            return gridData[tilePosition.x, tilePosition.y];
        }
        else
        {
            return null;
        }
    }

    public Tile GetTileAtPosition(Vector3 position)
    {
        Vector2Int convertPos = new Vector2Int((int)position.x, (int)position.z);
        return GetTileAtPosition(convertPos);
    }

    public Tile[] GetTilesAtPositions(Vector2Int[] tilePositions)
    {
        Tile[] tileArray = new Tile[tilePositions.Length];

        for (int i = 0; i < tilePositions.Length; i++)
        {
            if (IsWithinBounds(tilePositions[i]))
            {
                tileArray[i] = gridData[tilePositions[i].x, tilePositions[i].y];
            }
            else
            {
                tileArray[i] = null;
            }
        }
        return tileArray;
    }

    public float GetManhattanTileDistance(Tile source, Tile target)
    {
        int distanceX = Mathf.Abs(source.Position.x - target.Position.x);
        int distanceY = Mathf.Abs(source.Position.y - target.Position.y);

        return distanceX + distanceY;
    }

    private void SetTileWalkableState(Vector2Int tilePosition, bool isWalkable)
    {
        gridData[tilePosition.x, tilePosition.y].IsWalkable = isWalkable;
    }

    public void ResetTiles()
    {
        for(int y = 0; y < gridHeight; y++)
        {
            for(int x = 0; x < gridWidth; x++)
            {
                gridData[x, y].Reset();
            }
        }
    }
}
