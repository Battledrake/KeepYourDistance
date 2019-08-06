using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class CharacterSensor : MonoBehaviour
{
    [SerializeField] private string[] tagsToSearchFor;

    private BoxCollider collisionDetector;

    public event System.Action<Character> OnCharacterSensed;

    private void Start()
    {
        collisionDetector = GetComponent<BoxCollider>();
        collisionDetector.isTrigger = true;
    }

    private void OnTriggerStay(Collider other)
    {
        if (!enabled)
            return;



        foreach (string tag in tagsToSearchFor)
        {
            if (other.CompareTag(tag))
            {
                float distance = Vector3.Distance(other.transform.position, this.transform.position);
                if (distance < Mathf.RoundToInt(Mathf.Sqrt(collisionDetector.size.x + collisionDetector.size.z)))
                {
                    Character sensedCharacter = other.GetComponent<Character>();
                    if (sensedCharacter != null)
                    {
                        OnCharacterSensed(sensedCharacter);
                    }
                }
            }
        }
    }
}
