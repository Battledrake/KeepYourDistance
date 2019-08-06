using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueEvent : MonoBehaviour
{
    [SerializeField] private ScriptableDialogue dialogue;
    [SerializeField] private Tile[] triggerTiles;
    [SerializeField] private GameObject dialogueUI;

    private void OnEnable()
    {
        foreach(Tile tile in triggerTiles)
        {
            if(tile != null)
            {
                tile.OnCharacterEntered += EventTriggered;
            }
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            EndDialogue();
        }
    }

    private void EventTriggered(Character character)
    {
        if(character.CompareTag("Player"))
        {
            //dialogueUI.SetActive(true);
            Time.timeScale = 0;
        }
    }

    private void NextDialogue()
    {

    }

    private void EndDialogue()
    {
        Time.timeScale = 1;
        //dialogueUI.SetActive(false);
    }

    private void OnDisable()
    {
        foreach(Tile tile in triggerTiles)
        {
            if(tile != null)
            {
                tile.OnCharacterEntered -= EventTriggered;
            }
        }
    }
}
