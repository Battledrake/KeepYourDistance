using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GauntletSpawnManager : MonoBehaviour
{
    [SerializeField] private Character[] enemiesToSpawn;
    [SerializeField] private float waveDelay;

    private Character[] basicEnemy = new Character[10];
    private Character[] archerEnemy = new Character[10];
    private Character[] hagEnemy = new Character[10];
    private Character[] warlockEnemy = new Character[10];

    private int waveNumber;
    private float waveTimer;
    private bool isWaveComplete;
    private List<Character> activeCharacters = new List<Character>();

    private void Start()
    {
        GameObject basic = new GameObject("Basic");
        GameObject archer = new GameObject("Archer");
        GameObject hag = new GameObject("Hag");
        GameObject warlock = new GameObject("Warlock");
        for (int i = 0; i < 10; i++)
        {
            basicEnemy[i] = Instantiate(enemiesToSpawn[0], basic.transform);
            basicEnemy[i].gameObject.SetActive(false);
            basicEnemy[i].OnCharacterDeath += EnemyDeath;
            archerEnemy[i] = Instantiate(enemiesToSpawn[1], archer.transform);
            archerEnemy[i].gameObject.SetActive(false);
            archerEnemy[i].OnCharacterDeath += EnemyDeath;
            hagEnemy[i] = Instantiate(enemiesToSpawn[2], hag.transform);
            hagEnemy[i].gameObject.SetActive(false);
            hagEnemy[i].OnCharacterDeath += EnemyDeath;
            warlockEnemy[i] = Instantiate(enemiesToSpawn[3], warlock.transform);
            warlockEnemy[i].gameObject.SetActive(false);
            warlockEnemy[i].OnCharacterDeath += EnemyDeath;
        }

        waveNumber = 0;
        isWaveComplete = true;
    }

    private void EnemyDeath(Character character)
    {
        character.gameObject.SetActive(false);
        activeCharacters.Remove(character);

        if (activeCharacters.Count <= 0)
        {
            isWaveComplete = true;
            waveNumber++;
        }
    }

    private void Update()
    {
        if (isWaveComplete)
        {
            waveTimer += Time.deltaTime;
            if (waveTimer > waveDelay)
            {
                SpawnWave();
            }
        }
    }

    private void SpawnWave()
    {
        isWaveComplete = false;

        if (waveNumber >= 4)
        {
            for (int i = 0; i < waveNumber / 4; i++)
            {
                activeCharacters.Add(warlockEnemy[i]);
            }
        }
        if (waveNumber >= 3)
        {
            for (int i = 0; i < waveNumber / 3; i++)
            {
                activeCharacters.Add(hagEnemy[i]);
            }
        }
        if (waveNumber >= 2)
        {
            for (int i = 0; i < waveNumber / 2; i++)
            {
                activeCharacters.Add(archerEnemy[i]);
            }
        }
        for (int i = 0; i < waveNumber / 5 + 1; i++)
        {
            activeCharacters.Add(basicEnemy[i]);
        }

        Tile[] gridTiles = GridMap.instance.MapTiles;
        foreach (Character enemy in activeCharacters)
        {
            int randomNumber = Random.Range(0, gridTiles.Length);
            while (!gridTiles[randomNumber].IsWalkable && gridTiles[randomNumber].CharacterOnTile != null)
                randomNumber = Random.Range(0, gridTiles.Length);
            enemy.gameObject.SetActive(true);
            enemy.PlaceCharacterOnTile(gridTiles[randomNumber]);
            if(enemy.GetCurrentTile() == null)
            {
                enemy.Kill();
            }
        }
    }
}
