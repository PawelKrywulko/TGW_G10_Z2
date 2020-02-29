using Assets.Scripts.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpikesController : MonoBehaviour
{
    [SerializeField] private List<GameObject> spikeContainers;
    [SerializeField] private float animationTime = 4f;
    [SerializeField] [Range(0.2f, 0.8f)] private float spikeOffset = 0.4f;
    [SerializeField] [Range(1, 2)] private int hidingMethod = 1;

    private Dictionary<string, List<Transform>> spikeWalls;

    void Start()
    {
        SetupAllSpikes();
        GameEvents.Instance.OnBankTriggerEntered += ManageSpikes;
        GameEvents.Instance.OnWallTriggerEntered += OnPlayerWallEntered;
    }

    private void ManageSpikes()
    {
        StartCoroutine(HideAllSpikes());
    }

    private void SetupAllSpikes()
    {
        spikeWalls = new Dictionary<string, List<Transform>>();
        foreach (var container in spikeContainers)
        {
            var spikesList = container.GetComponentsInChildren<Transform>().Skip(1).ToList();
            foreach (var spike in spikesList)
            {
                spike.localPosition = new Vector3(0, spike.localPosition.y);
            }
            spikeWalls.Add(container.name, spikesList);
        }
    }

    IEnumerator HideAllSpikes()
    {
        if (hidingMethod == 1)
        {
            foreach (var spikeWall in spikeWalls)
            {
                foreach (var spike in spikeWall.Value.Where(spike => spike.localPosition.x != 0))
                {
                    var originPosition = spike.localPosition;
                    var newPosition = new Vector3(0, spike.localPosition.y);
                    float journey = 0;
                    while (journey <= animationTime)
                    {
                        journey += Time.deltaTime;
                        var percent = Mathf.Clamp01(journey / animationTime);
                        spike.localPosition = Vector3.Lerp(originPosition, newPosition, percent);
                        yield return null;
                    }
                }
            }
        }
        if (hidingMethod == 2)
        {
            var unhiddenSpikes = spikeWalls.SelectMany(x => x.Value).Where(spike => spike.localPosition.x != 0).ToList();
            float time = 0;
            while (unhiddenSpikes.Count != 0)
            {
                foreach (var spike in unhiddenSpikes)
                {
                    time += Time.deltaTime;
                    spike.localPosition = Vector3.Lerp(spike.localPosition, new Vector3(0, spike.localPosition.y), time / animationTime);
                    unhiddenSpikes = spikeWalls.SelectMany(x => x.Value).Where(x => x.localPosition.x != 0).ToList();

                }
                yield return null;
            }
        }
    }

    void OnPlayerWallEntered(PlayerWallEntered player)
    {
        StartCoroutine(DrawAndAnimateSpike(player.EnteredWallName));
    }

    IEnumerator DrawAndAnimateSpike(string wallName)
    {
        wallName = wallName.Contains("Left") ? "Right" : "Left";
        var spikes = spikeWalls[wallName].Where(spike => spike.localPosition.x == 0).ToList();
        
        var concreteSpike = spikes[UnityEngine.Random.Range(0, spikes.Count)];
        var originPosition = concreteSpike.localPosition;
        var newXPos = concreteSpike.localPosition.x + (wallName == "Left" ? spikeOffset : -spikeOffset);
        var newPosition = new Vector3(newXPos, concreteSpike.localPosition.y);

        float journey = 0;
        while (journey <= animationTime)
        {
            journey += Time.deltaTime;
            var percent = Mathf.Clamp01(journey / animationTime);
            concreteSpike.localPosition = Vector3.Lerp(originPosition, newPosition, percent);
            yield return null;
        }
    }
}
