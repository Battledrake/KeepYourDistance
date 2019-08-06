using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GauntletGridController : MonoBehaviour
{
    [SerializeField] private GameObject tileEffect;
    [SerializeField] private float timeBeforeChange;
    [SerializeField] private int numberOfTilesToChange;
    [Range(2.0f, 5.0f)]
    [SerializeField] private float warningDelay = 2.0f;

    private Tile[] gridTiles;
    private List<Tile> disabledTiles;
    private List<Tile> tilesToDisable;

    private void Start()
    {
        gridTiles = GridMap.instance.MapTiles;

        InvokeRepeating("TileWarning", timeBeforeChange, timeBeforeChange);
    }

    private void OnEnable()
    {
        disabledTiles = new List<Tile>();
        tilesToDisable = new List<Tile>();
    }


    private void TileWarning()
    {
        tilesToDisable.Clear();
        tilesToDisable = GetRandomTiles();

        foreach(Tile tile in tilesToDisable)
        {
            GameObject tileWarning = Instantiate(tileEffect, tile.transform.position + new Vector3(0, 0.1f, 0), Quaternion.identity);
            Destroy(tileWarning, warningDelay);
        }

        Invoke("ChangeTiles", warningDelay);
    }

    private List<Tile> GetRandomTiles()
    {
        List<Tile> tiles = new List<Tile>();
        for(int i = 0; i < numberOfTilesToChange; i++)
        {
            int randomNum = Random.Range(0, gridTiles.Length);
            while(tiles.Contains(gridTiles[randomNum]))
            {
                randomNum = Random.Range(0, gridTiles.Length);
            }
            tiles.Add(gridTiles[randomNum]);
        }
        return tiles;
    }

    private void ChangeTiles()
    {
        foreach(Tile disabledTile in disabledTiles)
        {
            disabledTile.EnableTile();
        }
        disabledTiles.Clear();
        foreach (Tile tileToDisable in tilesToDisable)
        {
            tileToDisable.DisableTile();
            disabledTiles.Add(tileToDisable);
        }
    }
}
