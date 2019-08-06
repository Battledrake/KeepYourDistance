using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Summon : Spell
{
    [SerializeField] private GameObject characterToSummon;

    public override void Cast(Tile casterTile, Tile targetTile)
    {

        if (targetTile == null || !targetTile.IsWalkable)
        {
            Destroy(gameObject);
            return;
        }

        GameObject summon = Instantiate(characterToSummon, targetTile.transform.position, Quaternion.identity);
        Character summonCharacter = summon.GetComponent<Character>();
        GameObject impactObject = Instantiate(impactEffect, targetTile.transform.position, Quaternion.identity);
        Destroy(impactObject, 1.0f);

        if (summonCharacter == null)
        {
            Debug.Log("Uh oh, your SUMMONCHARACTER spell summon doesn't have a CHARACTER script.");
        }
        summonCharacter.PlaceCharacterOnTile(targetTile);
        AudioSource.PlayClipAtPoint(impactSound, summonCharacter.transform.position);

        summonCharacter.OnCharacterDeath += ((x) =>
        {
            Destroy(summon, 1.0f);
            Destroy(gameObject, 2.0f);
        });

        if (effectDuration > 0)
        {
            Destroy(summon, effectDuration);
            Destroy(gameObject, effectDuration + 1.0f);
        }
    }
}
