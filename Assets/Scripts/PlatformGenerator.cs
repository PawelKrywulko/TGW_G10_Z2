using Assets.Scripts;
using Assets.Scripts.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlatformGenerator : MonoBehaviour
{
    [SerializeField] GameObject platform = null;
    [SerializeField] MinMaxPosition firstPlatformSpawnLocation = default;
    [SerializeField] MinMaxPosition SpaceBetweenPlatforms = default;

    List<GameObject> platforms = new List<GameObject>();
    float factor = 1;
    float xPos;
    float yPos;

    void Start()
    {
        GameEvents.Instance.OnWallTriggerEntered += SetUpPlatforms;
        GameEvents.Instance.OnGameStarts += SetUpPlatformsAfterStart;
        SetupPlatformsBeforeStart(8.5f);
    }

    private void SetUpPlatformsAfterStart()
    {
        var activePlatform = platforms.First(platform => platform.activeSelf == true);
        var remainingPlatforms = platforms.Where(platform => platform.activeSelf == false).ToList();

        var startPosition = activePlatform.transform.position;
        for (int i = 0; i < 5; i++)
        {
            var platform = remainingPlatforms[i];
            platform.transform.position = new Vector3(startPosition.x + i + 1, startPosition.y);
            platform.SetActive(true);
        }
    }

    private void SetupPlatformsBeforeStart(float firstPlatformXPos)
    {
        Enumerable.Range(0, 10).ToList().ForEach(index =>
        {
            var newPlatform = Instantiate(platform, new Vector3(firstPlatformXPos, 5), Quaternion.identity);
            if(index != 0)
                newPlatform.SetActive(false);
            platforms.Add(newPlatform);
        });
    }

    private void SetUpPlatforms(PlayerWallEntered player)
    {
        factor *= -1f;
        platforms.ForEach(platform => platform.SetActive(false));
        xPos = player.PlayerPosition.x + UnityEngine.Random.Range(firstPlatformSpawnLocation.minX, firstPlatformSpawnLocation.maxX) * factor;
        yPos = player.PlayerPosition.y - UnityEngine.Random.Range(firstPlatformSpawnLocation.minY, firstPlatformSpawnLocation.maxY);
        yPos = Mathf.Clamp(yPos, 2f, 7f);

        var firstPlatform = platforms[0];
        firstPlatform.transform.position = new Vector3(xPos, yPos);
        firstPlatform.SetActive(true);

        foreach (var platform in platforms.Where(p => p.activeSelf == false))
        {
            xPos += UnityEngine.Random.Range(SpaceBetweenPlatforms.minX, SpaceBetweenPlatforms.maxX) * factor;
            yPos += UnityEngine.Random.Range(SpaceBetweenPlatforms.minY, SpaceBetweenPlatforms.maxY);

            yPos = Mathf.Clamp(yPos, 2f, 7f);

            if (xPos < 1 || xPos > 16)
                break;

            platform.transform.position = new Vector3(xPos, yPos);
            platform.SetActive(true);
            platform.GetComponent<Platform>().GenerateCoinRandomly();
        }
    }
}
