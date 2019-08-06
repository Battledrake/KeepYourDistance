using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilesColorProperties : MonoBehaviour
{
    public static TilesColorProperties instance;
    [Header("On Character Enter")]
    public Color OnCharacterEnterLineColor = Color.blue;
    public Color OnCharacterEnterCellColor = Color.cyan;
    public float OnCharacterEnterDuration = 1f;
    [Header("On Character Exit")]
    public Color OnCharacterExitLineColor = Color.black;
    public Color OnCharacterExitCellColor = Color.white;
    public float OnCharacterExitDuration = 1f;

    private void Awake()
    {
        instance = this;
    }

}
