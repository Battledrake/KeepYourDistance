using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private Character playerCharacter;

    private bool isRandomSpells;

    private void Start()
    {
        DontDestroyOnLoad(this);
        SceneManager.sceneLoaded += OnSceneLoad;
    }


    public void SceneSelection(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    private void OnSceneLoad(Scene scene, LoadSceneMode sceneMode)
    {
        if (scene != SceneManager.GetSceneByName("MainMenu"))
        {
            playerCharacter = GameObject.FindGameObjectWithTag("Player").GetComponent<Character>();
            //playerCharacter.OnCharacterDeath += ((x) => SceneManager.LoadScene("MainMenu"));
            playerCharacter.OnCharacterDeath += OnPlayerDeath;
        }

        if(scene == SceneManager.GetSceneByName("Gauntlet"))
        {
            SpellFactory spellFactory = GameObject.Find("_Weave").GetComponent<SpellFactory>();
            spellFactory.ActivateSpellFactory(isRandomSpells);
        }

    }
    private void OnPlayerDeath(Character character)
    {
        if (SceneManager.GetActiveScene().name != "MapsScene")
            SceneManager.LoadScene("MainMenu");
    }

    public void SetRandomSpellValue(bool value)
    {
        isRandomSpells = value;
    }

    public void GameExit()
    {
        Application.Quit();
    }
}
