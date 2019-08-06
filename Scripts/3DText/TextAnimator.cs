using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextAnimator : MonoBehaviour
{
    TMP_Text textComponent;

    [SerializeField] bool startInvisible=false;
    [Header("FadeIn")]
    [SerializeField] float fadeInDuration = 5;
    [SerializeField] [Range(0, 1)] float fadeInSpeed = 0.5f;
    public Action fadeInCompleteAction;
    public Action fadingInNextLetterAction;

    [Header("FadeOut")]
    [SerializeField] float fadeOutDuration = 5;
    [SerializeField] [Range(0, 1)] float fadeOutSpeed = 0.5f;
    public Action fadeOutCompleteAction;
    public Action fadingOutNextLetterAction;

    [Header("FadeColor")]
    [SerializeField] float fadeColorDuration = 5;
    [SerializeField] [Range(0, 1)] float fadeColorSpeed = 0.5f;
    [SerializeField] Color32 color;
    public Action fadeColorCompleteAction;
    public Action fadingColorNextLetterAction;
    // Start is called before the first frame update
    void Awake()
    {

        textComponent = GetComponent<TMP_Text>();
        if (startInvisible)
        {
            textComponent.color = new Color(textComponent.color.r, textComponent.color.g, textComponent.color.b, 0);
        }
        textComponent.ForceMeshUpdate();

       
        fadeInCompleteAction += delegate { };
        fadingInNextLetterAction += delegate { };

        fadeOutCompleteAction += delegate { };
        fadingOutNextLetterAction += delegate { };

        fadeColorCompleteAction += delegate { };
        fadingColorNextLetterAction += delegate { };

        
        //StartCoroutine(FadeInRoutine());
        //StartCoroutine(FadeOutRoutine());
        //StartCoroutine(FadeColorRoutine(color));

    }
    // Update is called once per frame
    void Update()
    {
        
    }
    public void FadeIn()
    {
        StartCoroutine( FadeInRoutine());
    }
    public void FadeOut()
    {
        StartCoroutine( FadeOutRoutine());
    }
    public void FadeColor(Color32 color)
    {
        StartCoroutine(FadeColorRoutine(color));
    }
    
    IEnumerator FadeInRoutine()
    {
        if (textComponent.textInfo.characterCount == 0)
            textComponent.ForceMeshUpdate();
        float counter = 0;

        TMP_TextInfo textInfo = textComponent.textInfo;
        Color32[] newVertexColors;

        int characterCount = textInfo.characterCount;
        int previousCharacter = -1;
        int currentCharacter = 0;
        int whiteSpaceCount = GetWhiteSpaceCount(textInfo);
        int startingCharacterRange = currentCharacter;

        bool fadedCompletely = false;

        float timePassedInWhile = 0;
        
        while (counter < fadeInDuration || fadedCompletely == false)
        {
            characterCount = textInfo.characterCount;
            timePassedInWhile += Time.deltaTime;
            counter += Time.deltaTime;
            if (counter > fadeInDuration)
                counter = fadeInDuration;
            float timePassedInZeroOneRange = counter / fadeInDuration;

            currentCharacter = (int)(timePassedInZeroOneRange * (((characterCount - 1 - whiteSpaceCount))));

            if (currentCharacter > previousCharacter)
            {
                fadingInNextLetterAction.Invoke();
                previousCharacter = currentCharacter;
            }

            if (char.IsWhiteSpace(textInfo.characterInfo[currentCharacter].character))
                continue;
            //Debug.Log("currentCharacter " + currentCharacter + " timePassedInZeroOneRange " + timePassedInZeroOneRange + " characterCount " + characterCount);
            byte fadeSteps = (byte)Mathf.Max(1, 255 / 10);
            byte alpha = 0;
            for (int i = startingCharacterRange; i < currentCharacter + 1; i++)
            {
                //if (textInfo.characterInfo[i].isVisible == true) continue;
                if (char.IsWhiteSpace(textInfo.characterInfo[i].character) == true)
                {
                    continue;
                }
                int materialIndex = textInfo.characterInfo[i].materialReferenceIndex;
                newVertexColors = textInfo.meshInfo[materialIndex].colors32;
                int vertexIndex = textInfo.characterInfo[i].vertexIndex;
                
                alpha = (byte)Mathf.Clamp(newVertexColors[vertexIndex + 0].a + fadeInSpeed * 255, 0, 255);
                newVertexColors[vertexIndex + 0].a = alpha;
                newVertexColors[vertexIndex + 1].a = alpha;
                newVertexColors[vertexIndex + 2].a = alpha;
                newVertexColors[vertexIndex + 3].a = alpha;
                textComponent.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);

                if (alpha == 255) startingCharacterRange = i;
            }
            if (currentCharacter == characterCount - 1 && alpha == 255)
            {
                //Debug.Log("FadedIn completely");
                fadedCompletely = true;
            }
            //Debug.Log(timePassedInWhile);
            newVertexColors=textInfo.meshInfo[0].colors32;
            yield return new WaitForEndOfFrame();

        }
        textComponent.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
        fadeInCompleteAction.Invoke();

        yield break;
    }
    IEnumerator FadeOutRoutine()
    {
        if(textComponent.textInfo.characterCount==0)
            textComponent.ForceMeshUpdate();
        float counter = 0;

        TMP_TextInfo textInfo = textComponent.textInfo;
        Color32[] newVertexColors;

        int characterCount = textInfo.characterCount;
        int previousCharacter = -1;
        int currentCharacter = 0;
        int whiteSpaceCount = GetWhiteSpaceCount(textInfo);
        int startingCharacterRange = currentCharacter;

        bool fadedCompletely = false;

        float timePassedInWhile = 0;
        while (counter < fadeOutDuration || fadedCompletely==false)
        {
            characterCount = textInfo.characterCount;
            timePassedInWhile += Time.deltaTime;
            counter += Time.deltaTime;
            if (counter > fadeOutDuration)
                counter = fadeOutDuration;
            float timePassedInZeroOneRange=counter / fadeOutDuration;

            currentCharacter = (int)(timePassedInZeroOneRange* ((characterCount-1- whiteSpaceCount)));

            if (currentCharacter > previousCharacter)
            {
                fadingOutNextLetterAction.Invoke();
                previousCharacter = currentCharacter;
            }

            if (char.IsWhiteSpace(textInfo.characterInfo[currentCharacter].character))
                continue;
            //Debug.Log("currentCharacter " + currentCharacter + " timePassedInZeroOneRange " + timePassedInZeroOneRange + " characterCount " + characterCount);
            byte fadeSteps = (byte)Mathf.Max(1, 255 / 10);
            byte alpha=255;
            for (int i = startingCharacterRange; i < currentCharacter + 1; i++)
            {
                if (textInfo.characterInfo[i].isVisible == false) continue;
                if (char.IsWhiteSpace(textInfo.characterInfo[i].character) == true)
                {
                    continue;
                }
                int materialIndex = textInfo.characterInfo[i].materialReferenceIndex;
                newVertexColors = textInfo.meshInfo[materialIndex].colors32;
                int vertexIndex = textInfo.characterInfo[i].vertexIndex;

                alpha = (byte)Mathf.Clamp(newVertexColors[vertexIndex + 0].a - fadeOutSpeed*255, 0, 255);
                newVertexColors[vertexIndex+0].a = alpha;
                newVertexColors[vertexIndex+1].a = alpha;
                newVertexColors[vertexIndex+2].a = alpha;
                newVertexColors[vertexIndex+3].a = alpha;
                textComponent.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);

                if (alpha == 0) startingCharacterRange = i;
            }
            if (currentCharacter==characterCount-1 && alpha == 0)
            {
                //Debug.Log("Faded out completely");
                fadedCompletely = true;
            }
            //Debug.Log(timePassedInWhile);
            yield return new WaitForEndOfFrame();
        }
        fadeOutCompleteAction.Invoke();
        textComponent.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
        yield break;
    }
    IEnumerator FadeColorRoutine(Color32 targetColor)
    {
        if (textComponent.textInfo.characterCount == 0)
            textComponent.ForceMeshUpdate();
        float counter = 0;

        TMP_TextInfo textInfo = textComponent.textInfo;
        Color32[] newVertexColors;

        int characterCount = textInfo.characterCount;
        int previousCharacter = -1;
        int currentCharacter = 0;
        int whiteSpaceCount = GetWhiteSpaceCount(textInfo);
        int startingCharacterRange = currentCharacter;

        bool fadedCompletely = false;

        float timePassedInWhile = 0;

        Color32 originalColor = textInfo.meshInfo[0].colors32[0];
        while (counter < fadeColorDuration || fadedCompletely == false)
        {
            characterCount = textInfo.characterCount;
            timePassedInWhile += Time.deltaTime;
            counter += Time.deltaTime;
            if (counter > fadeColorDuration)
                counter = fadeColorDuration;
            float timePassedInZeroOneRange = counter / fadeColorDuration;

            currentCharacter = (int)(timePassedInZeroOneRange * (((characterCount - 1 - whiteSpaceCount))));

            if (currentCharacter > previousCharacter)
            {
                fadingColorNextLetterAction.Invoke();
                previousCharacter = currentCharacter;
            }

            if (char.IsWhiteSpace(textInfo.characterInfo[currentCharacter].character))
                continue;
            //Debug.Log("currentCharacter " + currentCharacter + " timePassedInZeroOneRange " + timePassedInZeroOneRange + " characterCount " + characterCount);
            byte fadeSteps = (byte)Mathf.Max(1, 255 / 10);
            byte alpha = 0;
            Color32 newColor= textInfo.meshInfo[0].colors32[0];
            
            for (int i = startingCharacterRange; i < currentCharacter + 1; i++)
            {
                //if (textInfo.characterInfo[i].isVisible == true) continue;
                if (char.IsWhiteSpace(textInfo.characterInfo[i].character) == true)
                {
                    continue;
                }
                int materialIndex = textInfo.characterInfo[i].materialReferenceIndex;
                newVertexColors = textInfo.meshInfo[materialIndex].colors32;
                int vertexIndex = textInfo.characterInfo[i].vertexIndex;

                newColor = Color32.LerpUnclamped(newVertexColors[vertexIndex + 0], targetColor, fadeColorSpeed);
                var red = (byte)Mathf.Clamp(newVertexColors[vertexIndex + 0].r + fadeInSpeed * originalColor.r, 0, 255);
                //newColor =

                alpha = newVertexColors[vertexIndex + 0].a;
                newColor.a = alpha;
                newVertexColors[vertexIndex + 0] = newColor;
                newVertexColors[vertexIndex + 1] = newColor;
                newVertexColors[vertexIndex + 2] = newColor;
                newVertexColors[vertexIndex + 3] = newColor;
                textComponent.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);

                if (Color32.Equals(newColor, targetColor)) startingCharacterRange = i;
            }
            if (currentCharacter == characterCount - 1 && newColor.CompareRGB(targetColor))
            {
                //Debug.Log("Faded Color completely");
                fadedCompletely = true;
            }
            //Debug.Log(timePassedInWhile);
            newVertexColors=textInfo.meshInfo[0].colors32;
            yield return new WaitForEndOfFrame();

        }
        textComponent.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
        fadeColorCompleteAction.Invoke();
        yield break;
    }
   
    int GetWhiteSpaceCount(TMP_TextInfo textInfo)
    {
        int whiteSpaceCount=0;
        for (int i = 0; i < textInfo.characterCount; i++)
        {
            if (char.IsWhiteSpace(textInfo.characterInfo[i].character))
            {
                whiteSpaceCount++;
            }
        }


        return 0;
    }
}
