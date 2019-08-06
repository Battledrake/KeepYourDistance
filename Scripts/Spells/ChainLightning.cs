using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ChainLightning : Spell
{
    [SerializeField] private int chainDistance = 2;
    private List<Tile> tilesAlreadyHit = new List<Tile>();

    private bool hasFoundTarget = false;

    public override void Cast(Tile casterTile, Tile targetTile)
    {
        Tile[] hitTiles = new Tile[aoeTargets.Length];
        for (int i = 0; i < aoeTargets.Length; i++)
        {
            hitTiles[i] = GridMap.instance.GetTileAtPosition(aoeTargets[i] + targetTile.Position);
        }
        foreach (Tile hitTile in hitTiles)
        {
            if (hitTile != null)
            {
                if (hitTile.CharacterOnTile != null)
                {
                    Character firstHitCharacter = hitTile.CharacterOnTile.GetComponent<Character>();
                    if (firstHitCharacter != null)
                    {
                        hasFoundTarget = true;
                        GameObject projectile = Instantiate(spellProjectile, casterTile.transform.position + projectileStartOffset, Quaternion.identity);
                        projectile.transform.DOMove(firstHitCharacter.transform.position, projectileSpeed).SetSpeedBased().SetEase(Ease.Linear).OnComplete(() =>
                        {
                            tilesAlreadyHit.Add(hitTile);
                            ContinueChain(firstHitCharacter.GetCurrentTile());
                            firstHitCharacter.Kill();
                            Destroy(projectile, 1.0f);
                        });
                        break;
                    }
                }
            }
        }
        if (!hasFoundTarget)
        {
            GameObject projectile = Instantiate(spellProjectile, casterTile.transform.position + projectileStartOffset, Quaternion.identity);
            projectile.transform.DOMove(targetTile.transform.position + projectileStartOffset, projectileSpeed).SetSpeedBased().SetEase(Ease.Linear).OnComplete(() => Destroy(projectile, 1.0f));
        }
        Destroy(gameObject, 5.0f);
    }

    private void ContinueChain(Tile startTile)
    {
        List<Tile> hitTiles = new List<Tile>();
        for (int y = -chainDistance; y <= chainDistance; y++)
        {
            for (int x = -chainDistance; x <= chainDistance; x++)
            {
                if (!tilesAlreadyHit.Contains(GridMap.instance.GetTileAtPosition(new Vector2Int(startTile.Position.x + x, startTile.Position.y + y))))
                {
                    hitTiles.Add(GridMap.instance.GetTileAtPosition(new Vector2Int(startTile.Position.x + x, startTile.Position.y + y)));
                }
            }
        }
        foreach (Tile hitTile in hitTiles)
        {
            if (hitTile != null)
            {
                if (hitTile.CharacterOnTile != null)
                {
                    Character chainHitCharacter = hitTile.CharacterOnTile.GetComponent<Character>();
                    if (chainHitCharacter != null)
                    {
                        GameObject projectile = Instantiate(spellProjectile, startTile.transform.position + projectileStartOffset, Quaternion.identity);
                        projectile.transform.DOMove(chainHitCharacter.transform.position + projectileStartOffset, projectileSpeed).SetSpeedBased().SetEase(Ease.Linear).OnComplete(() =>
                        {
                            tilesAlreadyHit.Add(hitTile);
                            ContinueChain(chainHitCharacter.GetCurrentTile());
                            chainHitCharacter.Kill();
                            Destroy(projectile, 1.0f);
                        });
                        break;
                    }
                }
            }
        }
    }
}