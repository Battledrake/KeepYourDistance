using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction
{
    left,
    right,
    forward,
    backward,
}

public class TeleportPortal : MonoBehaviour
{
    [SerializeField] public ParticleSystem onUsageEffect;
    public Direction direction;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void OnTeleporting()
    {
        onUsageEffect.Play();
    }
    public void ChooseDirection(Direction direction)
    {
        var rotation = Quaternion.identity;
        if (direction == Direction.forward || direction == Direction.backward)
        {
            rotation = Quaternion.Euler(0, 0, 0);

        }
        else if (direction == Direction.left || direction == Direction.right)
        {
            rotation = Quaternion.Euler(0, 90, 0);

        }
        transform.rotation = rotation;

    }
    public void ClosePortal()
    {
        var particleSystems = GetComponentsInChildren<ParticleSystem>();
        foreach (var ps in particleSystems)
        {
            if (ps.name != onUsageEffect.name)
            {
                var main = ps.main;
                //main.loop = false;
                main.simulationSpeed = 2.38f;
                ps.Stop();
            }
        }
    }
    public void OpenPortalPrewarmed()
    {
        var particleSystems = GetComponentsInChildren<ParticleSystem>();
        foreach (var ps in particleSystems)
        {
            if (ps.name != onUsageEffect.name)
            {
                ps.Stop();
                ps.Clear();
                var main = ps.main;
                main.prewarm = true;

                ps.Play();
            }
        }
    }
}
