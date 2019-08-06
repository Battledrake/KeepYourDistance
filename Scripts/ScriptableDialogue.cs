using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Dialogue", menuName = "ScriptableObjects/Dialogue")]
public class ScriptableDialogue : ScriptableObject
{
    public Sprite speakingCharacterSprite;
    [TextArea(1, 5)]
    public string[] dialogueLines;
}
