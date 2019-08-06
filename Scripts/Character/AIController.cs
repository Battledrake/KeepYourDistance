using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Pathfinder))]
[RequireComponent(typeof(Character))]
[RequireComponent(typeof(CharacterSensor))]
public class AIController : MonoBehaviour
{
    private enum AIState
    {
        Idle,
        WaitingOnCommand,
        Moving,
        Casting
    }

    [SerializeField] private AIState currentState;
    [SerializeField] private Spell fingerOfDeath;
    [SerializeField] private bool canRoll;
    [SerializeField] private float castDelay = 1.0f;

    private CharacterSensor characterSensor;
    private Queue<Tile> destinationTiles;
    private Pathfinder pathFinder;
    private Character controlledCharacter;
    private Character targetCharacter;
    private Tile currentTile;
    private Tile targetTile;
    private float castTimer;
    private bool canCast;

    //TestingStuff
    private Spell[] characterSpells;
    private Spell primarySpell; //first or shortest range spell
    private Spell secondarySpell; //longest range spell

    private void Awake()
    {
        destinationTiles = new Queue<Tile>();
        pathFinder = GetComponent<Pathfinder>();
        controlledCharacter = GetComponent<Character>();
        characterSensor = GetComponent<CharacterSensor>();
        castTimer = 0;
        canCast = true;

        //SpellTesting
        characterSpells = controlledCharacter.GetCharacterSpells();
        if (characterSpells.Length > 1)
        {
            if (characterSpells[0].aiRangeBeforeCast > characterSpells[1].aiRangeBeforeCast)
            {
                secondarySpell = characterSpells[0];
                primarySpell = characterSpells[1];
            }
            else
            {
                secondarySpell = characterSpells[1];
                primarySpell = characterSpells[0];
            }
        }
        else if (characterSpells.Length > 0)
        {
            primarySpell = characterSpells[0];
        }


    }

    private void OnEnable()
    {
        controlledCharacter.OnMovementComplete += NextMove;
        controlledCharacter.OnEndCast += NextMove;
        controlledCharacter.OnCharacterDeath += ReleaseControl;
        if (characterSensor != null)
        {
            characterSensor.OnCharacterSensed += CharacterSensed;
        }

    }
    private void OnDisable()
    {
        if (characterSensor != null)
        {
            characterSensor.OnCharacterSensed -= CharacterSensed;
        }
    }

    private void CharacterSensed(Character newCharacter)
    {
        if (targetCharacter == null)
        {
            targetCharacter = newCharacter;
            targetCharacter.OnCharacterDeath += TargetKilled;
        }

        currentState = AIState.WaitingOnCommand;
    }

    private void RecalculatePath()
    {
        if (controlledCharacter != null)
        {
            currentTile = controlledCharacter.GetCurrentTile();
            if (currentTile != null)
            {
                pathFinder.PathSearch(currentTile, targetCharacter.GetCurrentTile());
                destinationTiles = pathFinder.GetQueuedPathTiles();
            }
        }
    }

    private void NextMove()
    {
        currentState = AIState.WaitingOnCommand;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            controlledCharacter.PlaceCharacterOnTile(GridMap.instance.GetTileAtPosition(controlledCharacter.transform.position));
        }

        if(!canCast)
        {
            castTimer += Time.deltaTime;
            if(castTimer >= castDelay)
            {
                canCast = true;
                castTimer = 0;
            }
        }

        if (currentState != AIState.Idle && targetCharacter != null)
            RecalculatePath();

        if (controlledCharacter != null)
        {
            if (currentState == AIState.WaitingOnCommand)
            {
                if(destinationTiles.Count <= 2)
                {
                    controlledCharacter.StopMovement();
                    controlledCharacter.StopRunningAnimation();
                    controlledCharacter.SelectedSpell = fingerOfDeath;
                    currentState = AIState.Casting;
                    controlledCharacter.BeginCast(targetCharacter.GetCurrentTile());
                }
                else if (canCast && primarySpell != null && destinationTiles.Count <= primarySpell.aiRangeBeforeCast && currentState != AIState.Moving)
                {
                    canCast = false;
                    controlledCharacter.StopMovement();
                    controlledCharacter.StopRunningAnimation();
                    controlledCharacter.SelectedSpell = primarySpell;
                    currentState = AIState.Casting;

                    if (primarySpell.targetDistance == 0)
                        controlledCharacter.BeginCast(controlledCharacter.GetCurrentTile());
                    else if (primarySpell.targetDistance == -1)
                        controlledCharacter.BeginCast(targetCharacter.GetCurrentTile());
                    else
                        controlledCharacter.BeginCast(pathFinder.GetListPathTiles()[primarySpell.targetDistance]);
                }
                else if (canCast && secondarySpell != null && currentState != AIState.Moving &&
                    destinationTiles.Count <= secondarySpell.aiRangeBeforeCast)
                {
                    canCast = false;
                    controlledCharacter.StopMovement();
                    controlledCharacter.StopRunningAnimation();
                    controlledCharacter.SelectedSpell = secondarySpell;
                    currentState = AIState.Casting;

                    if (secondarySpell.targetDistance == 0)
                        controlledCharacter.BeginCast(controlledCharacter.GetCurrentTile());
                    else if (secondarySpell.targetDistance == -1)
                        controlledCharacter.BeginCast(targetCharacter.GetCurrentTile());
                    else
                        controlledCharacter.BeginCast(pathFinder.GetListPathTiles()[secondarySpell.targetDistance]);
                }
                else if((canRoll && (primarySpell != null && destinationTiles.Count > primarySpell.aiRangeBeforeCast && canCast)) ||
                    (canRoll && destinationTiles.Count >= 4))
                {
                    controlledCharacter.StopMovement();
                    controlledCharacter.StopRunningAnimation();
                    controlledCharacter.RollInDirection(destinationTiles.Dequeue().transform.position);
                    currentState = AIState.Moving;
                }
                else if(destinationTiles.Count > 1)
                {
                    targetTile = destinationTiles.Dequeue();
                    if (targetTile.IsWalkable)
                    {
                        controlledCharacter.MoveTowards(targetTile.transform.position);
                        currentState = AIState.Moving;
                    }
                    else
                    {
                        controlledCharacter.StopMovement();
                    }
                }
                else
                {
                    controlledCharacter.StopMovement();
                }
            }
            else if(currentState == AIState.Moving && controlledCharacter.GetCharacterState() == CharacterState.Idle)
            {
                currentState = AIState.WaitingOnCommand;
            }
        }
    }

    private void TargetKilled(Character character)
    {
        character.OnCharacterDeath -= TargetKilled;
        targetCharacter = null;
        currentState = AIState.Idle;
    }

    private void ReleaseControl(Character character)
    {
        //currentState = AIState.Idle;
        //    controlledCharacter.OnMovementComplete -= NextMove;
        //    controlledCharacter.OnEndCast -= NextMove;
        //    controlledCharacter.OnCharacterDeath -= ReleaseControl;
        //    controlledCharacter = null;
    }
}
