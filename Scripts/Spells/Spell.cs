using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public abstract class Spell : MonoBehaviour
{
    protected enum SpellState
    {
        Normal,
        WaitingOnImpactDelay,
        Persistence
    }
    [SerializeField] protected string spellName;
    [SerializeField] public Sprite spellIcon;
    [SerializeField] protected bool isProjectile;
    [SerializeField] protected GameObject spellProjectile;
    [SerializeField] protected float projectileSpeed;
    [SerializeField] protected Vector3 projectileStartOffset = new Vector3(0.0f, 0.5f, 0.0f);
    [SerializeField] protected GameObject impactEffect;
    [SerializeField] protected GameObject warningEffect;
    [SerializeField] protected Vector2Int[] aoeTargets;
    [SerializeField] protected bool isRanged;
    [SerializeField] public string spellCastAnimation;
    [SerializeField] protected float effectDuration;
    [SerializeField] protected bool isDoT;
    [SerializeField] protected float impactDelay;
    [SerializeField] protected AudioClip impactSound;
    [SerializeField] public int aiRangeBeforeCast;
    [SerializeField] public int targetDistance = -1; //0 means cast directly on self. -1 cast on target.

    protected Tile casterTile, targetTile;
    protected float delayTimer = 0.0f;
    protected float durationTimer = 0.0f;
    protected SpellState currentState = SpellState.Normal;
    private List<Tile> trackedTiles = new List<Tile>();

    public bool IsRanged { get { return isRanged; } }

    public virtual void Cast(Tile casterTile, Tile targetTile)
    {
        this.casterTile = casterTile;
        this.targetTile = targetTile;

        if (!isProjectile)
        {
            if (impactDelay > 0)
            {
                currentState = SpellState.WaitingOnImpactDelay;
                DisplayWarningBeforeImpact();
            }
            else
            {
                OnImpact();
            }
        }
        else
        {
            GameObject newProjectile = Instantiate(spellProjectile, this.casterTile.transform.position + projectileStartOffset, Quaternion.identity);
            newProjectile.transform.DOMove(targetTile.transform.position, projectileSpeed).SetSpeedBased(true).SetEase(Ease.Linear).OnComplete(() =>
            {
                OnImpact();
                Destroy(newProjectile);
            });
        }
    }

    private void Update()
    {
        if (currentState == SpellState.Persistence)
        {
            durationTimer += Time.deltaTime;
            if (durationTimer >= effectDuration)
            {
                EndDamageOverTime();
                currentState = SpellState.Normal;
            }
        }
        if (currentState == SpellState.WaitingOnImpactDelay)
        {
            delayTimer += Time.deltaTime;
            if (delayTimer >= impactDelay)
            {
                currentState = SpellState.Normal;
                OnImpact();
            }
        }
    }

    private void DisplayWarningBeforeImpact()
    {
        Tile[] tilesToHit = new Tile[aoeTargets.Length];
        for (int i = 0; i < tilesToHit.Length; i++)
        {
            tilesToHit[i] = GridMap.instance.GetTileAtPosition(aoeTargets[i] + targetTile.Position);
        }
        foreach (Tile hitTile in tilesToHit)
        {
            if (hitTile != null)
            {
                GameObject go = Instantiate(warningEffect, hitTile.transform.position + new Vector3(0, 0.25f, 0), Quaternion.identity);
                Destroy(go, impactDelay);
            }
        }
    }

    protected virtual void OnImpact()
    {
        if (impactSound != null)
        {
            AudioSource.PlayClipAtPoint(impactSound, targetTile.transform.position);
        }
        Tile[] tilesToHit = new Tile[aoeTargets.Length];
        for (int i = 0; i < tilesToHit.Length; i++)
        {
            tilesToHit[i] = GridMap.instance.GetTileAtPosition(aoeTargets[i] + targetTile.Position);
        }
        foreach (Tile hitTile in tilesToHit)
        {
            if (hitTile != null)
            {
                GameObject go = Instantiate(impactEffect, hitTile.transform.position, Quaternion.identity);

                if (isDoT)
                {
                    currentState = SpellState.Persistence;
                    hitTile.OnCharacterEntered += DamageCharacter;
                    trackedTiles.Add(hitTile);
                }
                else
                {
                    Destroy(gameObject, effectDuration + 1.0f);
                }

                Destroy(go, effectDuration);

                if (hitTile.CharacterOnTile != null)
                {
                    hitTile.CharacterOnTile.Kill();
                }
            }
        }
    }

    private void DamageCharacter(Character character)
    {
        character.Kill();
    }

    private void EndDamageOverTime()
    {
        foreach (Tile trackedTile in trackedTiles)
        {
            trackedTile.OnCharacterEntered -= DamageCharacter;
        }
        Destroy(gameObject, 1.0f);
    }
}

