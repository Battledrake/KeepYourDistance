using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public enum TeleportationType
{
    TwoWays,
    SingleUse,
}
[Serializable]
public enum TeleportPortalState
{
    Closed,
    Open,
}
[Serializable]
struct TeleportationPair
{
    bool finalTeleport;
    public string name;
    [SerializeField] public Tile tile1;
    [SerializeField] public Tile tile2;
    [SerializeField] public TeleportationType teleportationType;
    [SerializeField] public TeleportPortalState teleportPortalState;
    [SerializeField] public Direction portalDirection1;
    [SerializeField] public Direction portalDirection2;
    public TeleportPortal teleportPortalEffect1;
    public TeleportPortal teleportPortalEffect2;
    public Tile[] triggerTiles;

}

public class TeleportTilesManager : MonoBehaviour
{

    [SerializeField] List<TeleportationPair> teleportationTilePairs;
    Tile justTeleportedTo=null;
    Character justTeleportedCharacter=null;
    public float debugCounter=0;

    [SerializeField] TeleportPortal teleportPortalEffect;
    // Start is called before the first frame update
    void Start()
    {
        ListenToAllTiles();
        InstantiateTeleportEffectInAllPairs();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void ListenToAllTiles()
    {
   
        foreach (var pair in teleportationTilePairs)
        {
            foreach (var tile in pair.triggerTiles)
            {
                if (tile!=null)
                    tile.OnCharacterEntered += ActivatedTeleportationPortal;
            }

            if (pair.teleportPortalState == TeleportPortalState.Closed)
                continue;
            pair.tile1.GetComponent<Tile>().OnCharacterEntered += TeleportationTriggeredByCharacter;
            pair.tile2.GetComponent<Tile>().OnCharacterEntered += TeleportationTriggeredByCharacter;

            
        }
    }
    void InstantiateTeleportEffectInAllPairs()
    {
        for (int i = 0; i < teleportationTilePairs.Count; i++)
        {
            var pair = teleportationTilePairs[i];
            InstantiateTeleportEffectInPair(pair);

        }
        //foreach (var pair in teleportationTilePairs)
        //{
            //if(pair.teleportPortalState == TeleportPortalState.Closed)
            
        //}
    }
    void InstantiateTeleportEffectInPair(TeleportationPair pair)
    {

        var tile1 = pair.tile1.GetComponent<Tile>();
        var tile2 = pair.tile2.GetComponent<Tile>();
        
        
        TeleportPortal teleportPortal1 =Instantiate(teleportPortalEffect, tile1.transform.position,Quaternion.identity);
        TeleportPortal teleportPortal2 =Instantiate(teleportPortalEffect, tile2.transform.position,Quaternion.identity);

        teleportPortal1.ChooseDirection(pair.portalDirection1);
        teleportPortal2.ChooseDirection(pair.portalDirection2);

        pair.teleportPortalEffect1 = teleportPortal1;
        pair.teleportPortalEffect2 = teleportPortal2;

        var index=teleportationTilePairs.FindIndex(p => p.tile1.name==pair.tile1.name);
        teleportationTilePairs[index] = pair;

        if (pair.teleportPortalState== TeleportPortalState.Closed)
        {
            teleportPortal1.gameObject.SetActive(false);
            teleportPortal2.gameObject.SetActive(false);
        }
        else
        {
            teleportPortal1.gameObject.SetActive(true);
            teleportPortal2.gameObject.SetActive(true);
        }

    }
    void ActivatedTeleportationPortal(Character character)
    {
        if (character.tag != "Player")
        {
            return;
        }
        
        for (int i = 0; i < teleportationTilePairs.Count; i++)
        {
            var pair = teleportationTilePairs[i];
            foreach (var triggerTile in pair.triggerTiles)
            {
                if(triggerTile == character.GetCurrentTile())
                {
                    pair.teleportPortalEffect1.gameObject.SetActive(true);
                    pair.teleportPortalEffect2.gameObject.SetActive(true);

                    pair.tile1.OnCharacterEntered += TeleportationTriggeredByCharacter;
                    pair.tile2.OnCharacterEntered += TeleportationTriggeredByCharacter;

                    pair.teleportPortalState = TeleportPortalState.Open;
                    teleportationTilePairs[i] = pair;
                }
            }
        }
    }
    void TeleportationTriggeredByCharacter(Character character)
    {
        if (character.tag != "Player")
        {
            return;
        }

        Tile triggeringTile= character.GetComponent<Character>().GetCurrentTile();

        for (int i = 0; i < teleportationTilePairs.Count; i++)
        {
            var pair = teleportationTilePairs[i];

            if (justTeleportedTo != null)
            {
                if (justTeleportedTo.name == pair.tile1.name || justTeleportedTo.name == pair.tile2.name)
                {
                    continue;
                }
            }

            else if (triggeringTile.name == pair.tile1.name)
            {
                
                //character.transform.position = new Vector3(pair.tile2.Position.x, character.transform.position.y, pair.tile2.Position.y);
               
                if (pair.teleportationType==TeleportationType.SingleUse)
                {
                    justTeleportedTo = pair.tile2;
                    justTeleportedCharacter = character;
                    character.OnTileChange += MovedOutOfTeleportedTile;

                    character.PlaceCharacterOnTile(pair.tile2);

                    pair.teleportPortalEffect1.onUsageEffect.Play();
                    pair.teleportPortalEffect2.onUsageEffect.Play();
                    pair.teleportPortalEffect1.OpenPortalPrewarmed();
                    StopPortal(pair, true);
                    teleportationTilePairs.Remove(pair);
                    LevelsManager.instance.AdvanceLevel();
                }
            }
            else
            {
                if (triggeringTile.name == pair.tile2.name)
                {
                    //character.transform.position = new Vector3(pair.tile1.Position.x, character.transform.position.y, pair.tile1.Position.y);
                    
                    if (pair.teleportationType == TeleportationType.SingleUse)
                    {
                        justTeleportedTo = pair.tile1;
                        justTeleportedCharacter = character;
                        character.OnTileChange += MovedOutOfTeleportedTile;

                        character.PlaceCharacterOnTile(pair.tile1);

                        pair.teleportPortalEffect1.OnTeleporting();
                        pair.teleportPortalEffect2.OnTeleporting();
                        pair.teleportPortalEffect1.OpenPortalPrewarmed();
                        StopPortal(pair,true);
                        teleportationTilePairs.Remove(pair);
                        LevelsManager.instance.AdvanceLevel();
                    }
                    

                }
            }
        }
    }
    void MovedOutOfTeleportedTile()
    {
        if (justTeleportedCharacter.GetCurrentTile().name == justTeleportedTo.name)
            return;
        justTeleportedTo = null;
        justTeleportedCharacter.OnTileChange -= MovedOutOfTeleportedTile;
        justTeleportedCharacter = null;
    }
    void StopPortal(TeleportationPair pair,bool permanant=false)
    {
        pair.tile1.OnCharacterEntered -= TeleportationTriggeredByCharacter;
        pair.tile2.OnCharacterEntered -= TeleportationTriggeredByCharacter;
        pair.teleportPortalEffect1.ClosePortal();
        pair.teleportPortalEffect2.ClosePortal();

        if (permanant)
        {
            pair.triggerTiles = new Tile[0];
        }
        pair.teleportPortalState = TeleportPortalState.Closed;
        var index = teleportationTilePairs.FindIndex(p => p.tile1.name == pair.tile1.name);
        teleportationTilePairs[index] = pair;
        //teleportationTilePairs.Remove(pair);

    }
}
