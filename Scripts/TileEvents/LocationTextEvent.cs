using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationTextEvent : MonoBehaviour
{
    //[SerializeField] Tile tile;
    [SerializeField] Tile[] tiles;
    [SerializeField] TextController textController;
    [SerializeField]  FadeType FadeType = FadeType.FadeInThenOut;
    [SerializeField] Transform characterTransform;
    // Start is called before the first frame update
    void Start()
    {
        textController = GetComponentInChildren<TextController>();
        foreach (var tile in tiles)
        {
            tile.OnCharacterEntered += OnEnterTileFade;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnEnterTileFade(Character character)
    {
        if (character.tag != "Player")
            return;
        textController.Fade(FadeType);
        foreach (var tile in tiles)
        {
            tile.OnCharacterEntered -= OnEnterTileFade;
        }
        
        
    }
}
