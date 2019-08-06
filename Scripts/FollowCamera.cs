using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [SerializeField] private Transform followPlayer;
    [SerializeField] private Vector3 positionOffset;
    [SerializeField] private Vector3 lookOffset;

    private Character player;
    private bool isTrackingPlayer;

    private void Start()
    {
        if (followPlayer != null)
            isTrackingPlayer = true;

        player = followPlayer.GetComponent<Character>();
        if(player != null)
        {
            //player.OnCharacterDeath += ((x) => isTrackingPlayer = false);
        }
    }

    private void LateUpdate()
    {
        if (isTrackingPlayer)
        {
            transform.position = followPlayer.position + positionOffset;
            transform.LookAt(followPlayer.position + lookOffset);
        }
    }
    public void RetrackPlayer(Transform transform)
    {
        followPlayer = transform;
        if (followPlayer != null)
            isTrackingPlayer = true;
        isTrackingPlayer = true;
        player = followPlayer.GetComponent<Character>();
    }
}
