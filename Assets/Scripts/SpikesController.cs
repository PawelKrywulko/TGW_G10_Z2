using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpikesController : MonoBehaviour
{
    [SerializeField] private List<GameObject> spikeContainers;
    [SerializeField] private float timeToNextSpike;
    [SerializeField] private float animationTime = 4f;
    [SerializeField] private float spikeOffset = 0.4f;

    private Dictionary<string, List<Transform>> spikeWalls;

    void Start()
    {
        GameEvents.Instance.OnGameStarts += ResetAllSpikes;
        GameEvents.Instance.OnBankTriggerEntered += ResetAllSpikes;
    }

    private void ResetAllSpikes()
    {
        StopAllCoroutines();
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

        StartCoroutine(StartTimer(timeToNextSpike));
    }

    IEnumerator StartTimer(float time)
    {
        while(spikeWalls.SelectMany(x => x.Value).ToList().Count != 0)
        {
            yield return new WaitForSeconds(time);
            yield return DrawSpike();
        }
    }

    IEnumerator DrawSpike()
    {
        var spikeWallNames = spikeWalls.Where(x => x.Value.Count != 0).Select(x => x.Key).ToList();
        var randomName = spikeWallNames[UnityEngine.Random.Range(0, spikeWalls.Count)];
        var spikes = spikeWalls[randomName];
        var concreteSpike = spikes[UnityEngine.Random.Range(0, spikes.Count)];
        
        var originPosition = concreteSpike.localPosition;
        var newXPos = concreteSpike.localPosition.x + (randomName == "Left" ? spikeOffset : -spikeOffset);
        var newPosition = new Vector3(newXPos, concreteSpike.localPosition.y);

        spikeWalls[randomName] = spikeWalls[randomName].Where(x => x != concreteSpike).ToList();
        
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
