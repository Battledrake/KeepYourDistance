using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinder : MonoBehaviour
{
    private Tile startTile;
    private Tile goalTile;
    private bool isComplete;
    private PriorityQueue<Tile> frontierTiles;
    private List<Tile> exploredTiles;
    private Queue<Tile> queuedPathTiles;
    private List<Tile> listPathTiles;

    private void Awake()
    {

        frontierTiles = new PriorityQueue<Tile>();
        exploredTiles = new List<Tile>();
        queuedPathTiles = new Queue<Tile>();

        startTile = null;
        goalTile = null;
    }


    public Queue<Tile> GetQueuedPathTiles()
    {
        return queuedPathTiles;
    }

    public List<Tile> GetListPathTiles()
    {
        return listPathTiles;
    }

    public void PathSearch(Tile start, Tile goal)
    {
        ClearPath();

        startTile = start;
        startTile.DistanceTraveled = 0;
        goalTile = goal;

        isComplete = false;
        frontierTiles.Enqueue(startTile);

        while (!isComplete)
        {
            if (frontierTiles.Count > 0)
            {
                Tile currentTile = frontierTiles.Dequeue();

                if (!exploredTiles.Contains(currentTile))
                {
                    exploredTiles.Add(currentTile);
                }

                ExpandFrontierAStar(currentTile);

                if (frontierTiles.Contains(goalTile))
                {
                    FillPathLists();
                    isComplete = true;
                }
            }
            else
            {
                isComplete = true;
            }
        }
    }

    private void ExpandFrontierAStar(Tile tile)
    {
        if (tile != null)
        {
            for (int i = 0; i < tile.Neighbors.Count; i++)
            {
                if (!exploredTiles.Contains(tile.Neighbors[i]))
                {
                    float distanceToNeighbor = GridMap.instance.GetManhattanTileDistance(tile, tile.Neighbors[i]);
                    float newDistanceTraveled = distanceToNeighbor + tile.DistanceTraveled;

                    if (float.IsPositiveInfinity(tile.Neighbors[i].DistanceTraveled) ||
                        newDistanceTraveled < tile.Neighbors[i].DistanceTraveled)
                    {
                        tile.Neighbors[i].Previous = tile;
                        tile.Neighbors[i].DistanceTraveled = newDistanceTraveled;
                    }
                    if (!frontierTiles.Contains(tile.Neighbors[i]))
                    {
                        float distanceToGoal = GridMap.instance.GetManhattanTileDistance(tile.Neighbors[i], goalTile);
                        float distanceFromStart = tile.Neighbors[i].DistanceTraveled;

                        tile.Neighbors[i].Priority = distanceFromStart + distanceToGoal;

                        frontierTiles.Enqueue(tile.Neighbors[i]);
                    }
                }
            }
        }
    }

    private void FillPathLists()
    {
        List<Tile> pathList = new List<Tile>();
        Queue<Tile> pathQueue = new Queue<Tile>();
        if (goalTile == null)
        {
            Debug.Log("There appears to not be a goalTile after path has found one?");
        }
        pathList.Add(goalTile);

        Tile currentTile = goalTile.Previous;
        while (currentTile != null && currentTile != startTile)
        {
            pathList.Insert(0, currentTile);
            currentTile = currentTile.Previous;
        }
        foreach (Tile pathTile in pathList)
        {
            pathQueue.Enqueue(pathTile);
        }

        queuedPathTiles = pathQueue;
        listPathTiles = pathList;
    }

    private void ClearPath()
    {
        queuedPathTiles.Clear();
        exploredTiles.Clear();
        frontierTiles.Clear();
        startTile = null;
        goalTile = null;
        GridMap.instance.ResetTiles();
    }
}
