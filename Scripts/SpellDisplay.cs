using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpellDisplay : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Image activeSpellImage;
    [SerializeField] private Image[] spellSlots;
    [SerializeField] private SpellFactory spellFactory;

    private Character playerCharacter;

    private void Start()
    {
        playerCharacter = GameObject.FindGameObjectWithTag("Player").GetComponent<Character>();
        if(playerCharacter != null)
        {
            for(int i = 0; i < playerCharacter.GetCharacterSpells().Length; i++)
            {
                spellSlots[i].sprite = playerCharacter.GetCharacterSpells()[i].spellIcon;
            }
        }
    }

    private void OnEnable()
    {
        playerController.OnSpellChange += (incomingSpell => activeSpellImage.sprite = incomingSpell.spellIcon);
        spellFactory.OnSpellChange += (incomingSpell => activeSpellImage.sprite = incomingSpell.spellIcon);
    }
}
