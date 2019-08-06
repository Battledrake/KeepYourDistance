using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : Spell
{
    public override void Cast(Tile casterTile, Tile targetTile)
    {
        if (targetTile != null && targetTile.IsWalkable)
        {
            if (casterTile != null && casterTile.CharacterOnTile != null)
            {
                AudioSource.PlayClipAtPoint(impactSound, Camera.main.transform.position, 1.0f);
                Character characterToTeleport = casterTile.CharacterOnTile.GetComponent<Character>();
                GameObject teleportEffect = Instantiate(impactEffect, casterTile.transform.position + projectileStartOffset, Quaternion.identity);
                characterToTeleport.PlaceCharacterOnTile(targetTile);
                GameObject teleportEffect2 = Instantiate(impactEffect, targetTile.transform.position + projectileStartOffset, Quaternion.identity);
                Destroy(teleportEffect, 1.0f);
                Destroy(teleportEffect2, 1.0f);
            }
        }
        Destroy(gameObject, 5.0f);
    }
}
