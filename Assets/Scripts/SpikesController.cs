using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpikesController : MonoBehaviour
{
    [SerializeField] private List<GameObject> spikeContainers;
    [SerializeField] private float timeToNextSpike;

    private Dictionary<string, List<Transform>> spikeWalls;
    private float spikeOffset = 0.8f;

    void Start()
    {
        GameEvents.Instance.OnGameStarts += RunCoroutine;
        GameEvents.Instance.OnBankTriggerEntered += ResetAllSpikes;

        ResetAllSpikes();
    }

    void RunCoroutine()
    {
        StartCoroutine(StartTimer(timeToNextSpike));
    }

    IEnumerator StartTimer(float time)
    {
        while(spikeWalls.SelectMany(x => x.Value).ToList().Count != 0)
        {
            yield return new WaitForSeconds(time);
            DrawSpike();
        }
    }

    void DrawSpike()
    {
        var spikeWallNames = spikeWalls.Keys.ToList();
        var randomName = spikeWallNames[UnityEngine.Random.Range(0, spikeWalls.Count)];
        var spikes = spikeWalls[randomName];
        if (spikes.Count == 0) return;
        var concreteSpike = spikes[UnityEngine.Random.Range(0, spikes.Count)];
        concreteSpike.transform.localPosition += new Vector3(randomName == "Left" ? spikeOffset : -spikeOffset, 0);
        spikeWalls[randomName] = spikeWalls[randomName].Where(x => x != concreteSpike).ToList();
    }

    private void ResetAllSpikes()
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
}
