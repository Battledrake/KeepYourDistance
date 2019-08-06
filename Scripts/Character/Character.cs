using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public enum CharacterState
{
    Idle,
    Busy,
}

[SelectionBase]
[RequireComponent(typeof(Rigidbody))]
public class Character : MonoBehaviour
{

    [SerializeField] private float runSpeed = 5.0f;
    [SerializeField] private float rollSpeed = 10.0f;
    [SerializeField] private int rollDistance = 3;
    [SerializeField] private Spell[] characterSpells;

    private Tile spellTargetTile;
    private Animator anim;
    private Rigidbody rb;
    [SerializeField] private CharacterState currentState;
    private string animRun = "IsRunning";
    private string animRoll = "IsRolling";
    private bool isRunning = false;
    private bool isRolling = false;
    private bool isAlive = true;

    public event System.Action OnTileChange;
    public event System.Action OnMovementComplete;
    public event System.Action OnSpellCast;
    public event System.Action OnEndCast;
    public event System.Action<Character> OnCharacterDeath;
    public event System.Action<Spell[]> OnSpellsSet;

    public Spell SelectedSpell { get; set; }
    public CharacterState GetCharacterState() { return currentState; }

    private void OnEnable()
    {
        isAlive = true;
    }

    private void Start()
    {
        anim = GetComponent<Animator>();
        currentState = CharacterState.Idle;
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = true;

        if (characterSpells.Length > 0)
        {
            SelectedSpell = characterSpells[0];
        }
    }

    public Spell[] GetCharacterSpells()
    {
        return characterSpells;
    }

    public void SetCharacterSpells(Spell[] newSpells)
    {
        characterSpells = newSpells;
        SelectedSpell = characterSpells[0];

        if (OnSpellsSet != null)
            OnSpellsSet(newSpells);
    }

    public void BeginCast(Tile targetTile)
    {
        if (currentState == CharacterState.Idle && !isRunning && isAlive)
        {
            spellTargetTile = targetTile;
            transform.LookAt(targetTile.transform);
            anim.SetTrigger(SelectedSpell.spellCastAnimation);
            currentState = CharacterState.Busy;
        }
    }

    public void CastSpell()
    {
        if (isAlive)
        {
            Tile characterTile = GridMap.instance.GetTileAtPosition(transform.position);
            Spell spell = Instantiate(SelectedSpell);
            spell.Cast(characterTile, spellTargetTile);

            if (OnSpellCast != null)
            {
                OnSpellCast();
            }
        }
    }

    public void EndCast()
    {
        if (isAlive)
        {
            currentState = CharacterState.Idle;

            if (OnEndCast != null)
            {
                OnEndCast();
            }
        }
    }

    private void LookDirection(Vector3 direction)
    {
        transform.LookAt(direction);
    }

    public void MoveTowards(Vector3 direction)
    {
        if (isAlive)
        {
            isRunning = true;

            if (currentState == CharacterState.Idle)
            {
                Tile newTile = GridMap.instance.GetTileAtPosition(direction);

                if (newTile != null && newTile.IsWalkable)
                {
                    GetCurrentTile().CharacterExited();
                    newTile.CharacterEntered(this);

                    BeginMovement(direction, animRun, runSpeed);

                    if (OnTileChange != null)
                    {
                        OnTileChange();
                    }
                }
                else
                {
                    StopRunningAnimation();
                }
            }
        }
    }

    public void RollInDirection(Vector3 destination)
    {
        if (isAlive)
        {
            Vector2Int[] checkPositions = new Vector2Int[rollDistance];
            Vector3 direction = destination - transform.position;
            for (int i = 0; i < checkPositions.Length; i++)
            {
                checkPositions[i] = new Vector2Int((int)transform.position.x + ((int)direction.x * (i + 1)), (int)transform.position.z + ((int)direction.z * (i + 1)));
            }
            Tile[] checkTiles = GridMap.instance.GetTilesAtPositions(checkPositions);
            bool isBlockedTile = false;
            foreach (Tile checkedTile in checkTiles)
            {
                if (checkedTile == null || !checkedTile.IsWalkable)
                {
                    isBlockedTile = true;
                }
            }
            if (!isBlockedTile && currentState == CharacterState.Idle && !isRunning)
            {
                Tile newTile = GridMap.instance.GetTileAtPosition(transform.position + direction * rollDistance);

                GetCurrentTile().CharacterExited();

                isRolling = true;
                BeginMovement(transform.position + direction * rollDistance, animRoll, rollSpeed);

                if (OnTileChange != null)
                {
                    OnTileChange();
                }
            }
        }
    }

    public void PlaceCharacterOnTile(Tile targetTile)
    {
        if (isAlive)
        {
            if (targetTile.IsWalkable && !targetTile.CharacterOnTile && targetTile != null)
            {
                GetCurrentTile().CharacterExited();
                transform.position = targetTile.transform.position;
                targetTile.CharacterEntered(this);
            }
        }
    }

    private Vector2Int GetCurrentTilePosition()
    {
        return GetCurrentTile().Position;
    }

    public Tile GetCurrentTile()
    {
        return GridMap.instance.GetTileAtPosition(transform.position);
    }

    private void BeginMovement(Vector3 direction, string animToPlay, float speed)
    {
        if (isAlive)
        {
            currentState = CharacterState.Busy;
            transform.DOMove(direction, speed).SetSpeedBased(true).SetEase(Ease.Linear).OnComplete(() => MovementCompleted(animToPlay));
            anim.SetBool(animToPlay, true);
            LookDirection(direction);
        }
    }

    public void SetKinematicFalse()
    {
        rb.isKinematic = false;
    }

    public void StopMovement()
    {
        isRunning = false;
    }

    //Messy fix for an ongoing problem. 
    //TODO fix this issue once and for all!
    public void StopRunningAnimation()
    {
        anim.SetBool("IsRunning", false);
    }

    public void MovementCompleted(string animToPlay)
    {
        GetCurrentTile().CharacterEntered(this);

        if (!isRunning)
        {
            anim.SetBool("IsRunning", false);
        }
        anim.SetBool("IsRolling", false);
        currentState = CharacterState.Idle;

        if (OnMovementComplete != null)
        {
            OnMovementComplete();
        }
    }

    public void Kill()
    {
        if (isAlive)
        {
            isAlive = false;
            GetCurrentTile().CharacterExited();
            anim.SetTrigger("Death");
        }
    }

    public void DeathAnimFinished()
    {
        if (OnCharacterDeath != null)
        {
            GetCurrentTile().CharacterExited();
            currentState = CharacterState.Idle;
            OnCharacterDeath(this);
        }
    }
}
