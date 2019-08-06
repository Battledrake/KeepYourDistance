using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SerializeField] public enum FadeType{
    FadeInThenOut,
    FadeOutThenIn,
    FadeOut,
    FadeIn,
    None,
}

public class TextController : MonoBehaviour
{
    [SerializeField] FadeType startFade;
    [SerializeField] Color32 FadeInColor;
    [SerializeField] Color32 FadeOutColor;
    [SerializeField] TextAnimator textAnimator;
    // Start is called before the first frame update
    void Start()
    {
        textAnimator = GetComponent<TextAnimator>();
        //textAnimator.fadeInCompleteAction

        Fade();
    }

    public void Fade(FadeType fadeType=FadeType.None)
    {
        if (fadeType == FadeType.None)
        {
            fadeType = startFade;
        }
        switch (fadeType)
        {
            case FadeType.FadeInThenOut:
                FadeInThenOut();
                break;
            case FadeType.FadeOutThenIn:
                FadeOutThenIn();
                break;
            case FadeType.FadeOut:
                FadeOut();
                break;
            case FadeType.FadeIn:
                FadeIn();
                break;
            case FadeType.None:
                break;
            default:
                break;
        }
    }

    private void FadeOutThenIn()
    {
        FadeOut();
        textAnimator.fadeOutCompleteAction += FadeIn;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void FadeInThenOut()
    {
        FadeIn();
        textAnimator.fadeInCompleteAction += FadeOut;
    }
    void FadeOut()
    {
        textAnimator.FadeOut();
        textAnimator.FadeColor(FadeOutColor);

        textAnimator.fadeInCompleteAction -= FadeIn;
    }
    void FadeIn()
    {
        textAnimator.FadeIn();
        textAnimator.FadeColor(FadeInColor);

        textAnimator.fadeInCompleteAction -= FadeOut;
    }
    private void OnValidate()
    {
        Fade();
    }
}
