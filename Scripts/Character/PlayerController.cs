using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Character controlledCharacter;
    [SerializeField] private Spell[] summonEnemySpells;

    private Spell[] characterSpells;
    private Spell selectedSpell;

    public event System.Action<Spell> OnSpellChange;

    private void Start()
    {
        controlledCharacter.OnSpellsSet += ((spells) =>
        {
            characterSpells = spells;
            selectedSpell = spells[0];
        });
        characterSpells = controlledCharacter.GetCharacterSpells();
        if (characterSpells.Length > 0)
        {
            selectedSpell = characterSpells[0];
            OnSpellChange(characterSpells[0]);
        }
        else
        {
            selectedSpell = controlledCharacter.SelectedSpell;
        }

        controlledCharacter.PlaceCharacterOnTile(GridMap.instance.GetTileAtPosition(controlledCharacter.transform.position));
    }
    public void SetControlledCharacter(Character character)
    {
        controlledCharacter = character;
        characterSpells = controlledCharacter.GetCharacterSpells();
        if (characterSpells.Length > 0)
        {
            selectedSpell = characterSpells[0];
            OnSpellChange(characterSpells[0]);
        }
        else
        {
            selectedSpell = controlledCharacter.SelectedSpell;
        }
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GameObject.Find("_GameManager").GetComponent<GameManager>().SceneSelection("MainMenu");
        }
        if (Input.GetButtonDown("Cast"))
        {
            if (selectedSpell != null)
            {
                if (selectedSpell.IsRanged)
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hitInfo;
                    if (Physics.Raycast(ray, out hitInfo))
                    {
                        Tile spellTargetTile = hitInfo.transform.GetComponent<Tile>();
                        if (spellTargetTile != null)
                        {
                            controlledCharacter.BeginCast(spellTargetTile);
                        }
                        else
                        {
                            //TODO can put logic here to notify caster of invalid tile.
                        }
                    }
                }
                else
                {
                    controlledCharacter.BeginCast(controlledCharacter.GetCurrentTile());
                }

            }
        }

        SummonEnemies();

        SelectSpell();

        Roll();
        Run();
    }

    private void SelectSpell()
    {
        if (characterSpells.Length <= 0)
            return;
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (characterSpells[0] != null && characterSpells.Length > 0)
            {
                controlledCharacter.SelectedSpell = characterSpells[0];
                selectedSpell = characterSpells[0];
                OnSpellChange(characterSpells[0]);
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (characterSpells[0] != null && characterSpells.Length > 1)
            {
                controlledCharacter.SelectedSpell = characterSpells[1];
                selectedSpell = characterSpells[1];
                OnSpellChange(characterSpells[1]);
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (characterSpells[0] != null && characterSpells.Length > 2)
            {
                controlledCharacter.SelectedSpell = characterSpells[2];
                selectedSpell = characterSpells[2];
                OnSpellChange(characterSpells[2]);
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            if (characterSpells[0] != null && characterSpells.Length > 3)
            {
                controlledCharacter.SelectedSpell = characterSpells[3];
                selectedSpell = characterSpells[3];
                OnSpellChange(characterSpells[3]);
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            if (characterSpells[0] != null && characterSpells.Length > 4)
            {
                controlledCharacter.SelectedSpell = characterSpells[4];
                selectedSpell = characterSpells[4];
                OnSpellChange(characterSpells[4]);
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            if (characterSpells[0] != null && characterSpells.Length > 5)
            {
                controlledCharacter.SelectedSpell = characterSpells[5];
                selectedSpell = characterSpells[5];
                OnSpellChange(characterSpells[5]);
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            if (characterSpells[0] != null && characterSpells.Length > 6)
            {
                controlledCharacter.SelectedSpell = characterSpells[6];
                selectedSpell = characterSpells[6];
                OnSpellChange(characterSpells[6]);
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            if (characterSpells[0] != null && characterSpells.Length > 7)
            {
                controlledCharacter.SelectedSpell = characterSpells[7];
                selectedSpell = characterSpells[7];
                OnSpellChange(characterSpells[7]);
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            if (characterSpells[0] != null && characterSpells.Length > 8)
            {
                controlledCharacter.SelectedSpell = characterSpells[8];
                selectedSpell = characterSpells[8];
                OnSpellChange(characterSpells[8]);
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            if (characterSpells[0] != null && characterSpells.Length > 9)
            {
                controlledCharacter.SelectedSpell = characterSpells[9];
                selectedSpell = characterSpells[9];
                OnSpellChange(characterSpells[9]);
            }
        }

    }

    private void Run()
    {
        if (Input.GetButton("MoveRight"))
        {
            controlledCharacter.MoveTowards(controlledCharacter.transform.position + Vector3.right);
        }
        else if (Input.GetButton("MoveLeft"))
        {
            controlledCharacter.MoveTowards(controlledCharacter.transform.position + Vector3.left);
        }
        else if (Input.GetButton("MoveForward"))
        {
            controlledCharacter.MoveTowards(controlledCharacter.transform.position + Vector3.forward);
        }
        else if (Input.GetButton("MoveBackward"))
        {
            controlledCharacter.MoveTowards(controlledCharacter.transform.position + Vector3.back);
        }
        else
        {
            controlledCharacter.StopMovement();
        }
    }

    private void Roll()
    {
        if (Input.GetButton("Roll"))
        {
            if (Input.GetButtonDown("MoveRight"))
            {
                controlledCharacter.RollInDirection(controlledCharacter.transform.position + Vector3.right);
            }
            else if (Input.GetButtonDown("MoveLeft"))
            {
                controlledCharacter.RollInDirection(controlledCharacter.transform.position + Vector3.left);
            }
            else if (Input.GetButtonDown("MoveForward"))
            {
                controlledCharacter.RollInDirection(controlledCharacter.transform.position + Vector3.forward);
            }
            else if (Input.GetButtonDown("MoveBackward"))
            {
                controlledCharacter.RollInDirection(controlledCharacter.transform.position + Vector3.back);
            }
        }
    }

    private void SummonEnemies()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            controlledCharacter.SelectedSpell = summonEnemySpells[0];
        }
        if (Input.GetKeyDown(KeyCode.F2))
        {
            controlledCharacter.SelectedSpell = summonEnemySpells[1];
        }
        if (Input.GetKeyDown(KeyCode.F3))
        {
            controlledCharacter.SelectedSpell = summonEnemySpells[2];
        }
        if (Input.GetKeyDown(KeyCode.F4))
        {
            controlledCharacter.SelectedSpell = summonEnemySpells[3];
        }
    }
}
