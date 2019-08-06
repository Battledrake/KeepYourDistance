using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Consecration : Spell
{
    [SerializeField] private int numberOfWaves = 3;

    private int currentWave = 0;

    public override void Cast(Tile casterTile, Tile targetTile)
    {
        this.casterTile = casterTile;
        currentState = SpellState.WaitingOnImpactDelay;
        //OnImpact(currentWave);
    }

    private void Update()
    {
        if (currentState == SpellState.WaitingOnImpactDelay)
        {
            delayTimer += Time.deltaTime;
            if (delayTimer >= impactDelay)
            {
                currentState = SpellState.Normal;
                OnImpact(currentWave);
            }
        }
    }

    private void OnImpact(int wave)
    {
        if (currentWave < numberOfWaves)
        {
            Tile[] tiles = new Tile[12 + (wave * 8)];
            currentWave = wave;
            int incrementor = 0;
            for (int y = -2 - currentWave; y <= 2 + currentWave; y++)
            {
                for (int x = -2 - currentWave; x <= 2 + currentWave; x++)
                {
                    if (Mathf.Abs(x) == Mathf.Abs(y))
                    {
                        continue;
                    }
                    if ((y != -2 - currentWave && y != 2 + currentWave) && (x != -2 - currentWave && x != 2 + currentWave))
                    {
                        continue;
                    }
                    if (GridMap.instance.GetTileAtPosition(new Vector2Int(x, y) + casterTile.Position) != null)
                    {
                        tiles[incrementor] = GridMap.instance.GetTileAtPosition(new Vector2Int(x, y) + casterTile.Position);
                        incrementor++;
                    }
                }
            }

            AudioSource.PlayClipAtPoint(impactSound, transform.position);

            foreach (Tile tile in tiles)
            {
                if (tile != null)
                {
                    
                    GameObject impact = Instantiate(impactEffect, tile.transform.position, Quaternion.identity);
                    Destroy(impact, effectDuration);

                    if (tile.CharacterOnTile != null)
                    {
                        tile.CharacterOnTile.Kill();
                    }

                }
            }

            currentWave++;
            delayTimer = 0;
            currentState = SpellState.WaitingOnImpactDelay;
        }
        else
        {
            Destroy(gameObject, effectDuration + 1.0f);
        }
    }
}
