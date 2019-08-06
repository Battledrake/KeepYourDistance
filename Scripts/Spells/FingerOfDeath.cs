using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FingerOfDeath : Spell
{
    public override void Cast(Tile casterTile, Tile targetTile)
    {
        this.targetTile = targetTile;
        OnImpact();
    }

    protected override void OnImpact()
    {
        AudioSource.PlayClipAtPoint(impactSound, transform.position);
        Tile[] tilesToHit = new Tile[aoeTargets.Length];
        for (int i = 0; i < tilesToHit.Length; i++)
        {
            tilesToHit[i] = GridMap.instance.GetTileAtPosition(aoeTargets[i] + targetTile.Position);
        }
        foreach (Tile hitTile in tilesToHit)
        {
            if (hitTile != null)
            {
                GameObject go = Instantiate(impactEffect, hitTile.transform.position + new Vector3(0, 0.5f, 0), Quaternion.identity);
                Destroy(go, effectDuration);
            }
            if (hitTile.CharacterOnTile != null)
            {
                hitTile.CharacterOnTile.Kill();
            }
        }
        Destroy(gameObject, effectDuration);
    }
}
