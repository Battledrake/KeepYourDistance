using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellFactory : MonoBehaviour
{
    [SerializeField] private Spell[] availableSpells;
    [SerializeField] private bool isRandom = false;

    private Character playerCharacter;

    public event System.Action<Spell> OnSpellChange;


    public void ActivateSpellFactory(bool isRandomSpells)
    {
        playerCharacter = GameObject.FindGameObjectWithTag("Player").GetComponent<Character>();

        if (isRandomSpells)
        {
            playerCharacter.OnSpellCast += RandomSpell;
            RandomSpell();
        }
        else
        {
            FillPlayerSpells();
        }
    }

    private void RandomSpell()
    {
        Spell randomSpell = availableSpells[Random.Range(0, availableSpells.Length)];
        playerCharacter.SelectedSpell = randomSpell;

        if (OnSpellChange != null)
        {
            OnSpellChange(randomSpell);
        }
    }

    private void FillPlayerSpells()
    {
        playerCharacter.SetCharacterSpells(availableSpells);
    }

    private void OnDisable()
    {
        if (isRandom)
            playerCharacter.OnSpellCast -= RandomSpell;
    }
}
