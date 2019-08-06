using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using DG.Tweening;

[SelectionBase]
public class Tile : MonoBehaviour, IComparable<Tile>
{
    [SerializeField] private GameObject tileModel;
    [SerializeField] private bool isWalkable = true;

    public Vector2Int Position { get { return new Vector2Int((int)transform.position.x, (int)transform.position.z); } }
    public bool IsWalkable { get { return isWalkable; } set { isWalkable = value; } }
    public Character CharacterOnTile { get; set; } = null;

    public event Action<Character> OnCharacterEntered;
    //PathfindingStuff
    public float Priority { get; set; }
    public float DistanceTraveled { get; set; } = Mathf.Infinity;
    public Tile Previous { get; set; }
    public List<Tile> Neighbors { get; set; }

    private void Awake()
    {
        if(!isWalkable)
        {
            tileModel.SetActive(false);
        }
    }

    public int CompareTo(Tile other)
    {
        if (this.Priority < other.Priority)
        {
            return -1;
        }
        else if (this.Priority > other.Priority)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }

    private void OnMouseEnter()
    {
        MeshRenderer tileMesh = GetComponentInChildren<MeshRenderer>();
        if(tileMesh != null)
        {
            tileMesh.material.SetFloat("_SelectCell", 1);
        }
    }

    private void OnMouseExit()
    {
        MeshRenderer tileMesh = GetComponentInChildren<MeshRenderer>();
        if (tileMesh != null)
        {
            tileMesh.material.SetFloat("_SelectCell", 0);
        }
    }

    public void CharacterEntered(Character character)
    {
        CharacterOnTile = character;
        isWalkable = false;
        TilesColorProperties colors = TilesColorProperties.instance;
        MeshRenderer tileMesh = GetComponentInChildren<MeshRenderer>();
        if (tileMesh != null)
        {
            tileMesh.material.DOColor(colors.OnCharacterEnterLineColor, "_LineColor", colors.OnCharacterEnterDuration);
            tileMesh.material.DOColor(colors.OnCharacterEnterCellColor, "_CellColor", colors.OnCharacterEnterDuration);

        }

        if(OnCharacterEntered != null)
        {
            OnCharacterEntered(character);
        }
    }

    public void CharacterExited()
    {
        CharacterOnTile = null;
        isWalkable = true;
        TilesColorProperties colors = TilesColorProperties.instance;
        MeshRenderer tileMesh = GetComponentInChildren<MeshRenderer>();
        if (tileMesh != null)
        {
            tileMesh.material.DOColor(colors.OnCharacterExitLineColor, "_LineColor", colors.OnCharacterExitDuration);
            tileMesh.material.DOColor(colors.OnCharacterExitCellColor, "_CellColor", colors.OnCharacterExitDuration);
        }
    }

    public void DisableTile()
    {
        if(CharacterOnTile != null)
        {
            CharacterOnTile.SetKinematicFalse();
            CharacterOnTile.Kill();
            CharacterExited();
        }
        transform.GetComponentInChildren<Renderer>().enabled = false;
        isWalkable = false;
    }

    public void EnableTile()
    {
        transform.GetComponentInChildren<Renderer>().enabled = true;
        isWalkable = true;
    }

    public void Reset()
    {
        Previous = null;
        Priority = 0;
        DistanceTraveled = Mathf.Infinity;
    }

}
