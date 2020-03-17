using Assets.Scripts;
using Assets.Scripts.Events;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlatformGenerator : MonoBehaviour
{
    [SerializeField] GameObject platform = null;
    [SerializeField] MinMaxPosition firstPlatformSpawnLocation = default;
    [SerializeField] MinMaxPosition spaceBetweenPlatforms = default;
    [SerializeField] float spaceIncreaser = 0.05f;
    [SerializeField] bool spawnMultiplePlatforms = false;

    List<GameObject> platforms = new List<GameObject>();
    float factor = 1;
    float xPos;
    float yPos;

    void Start()
    {
        GameEvents.Instance.OnWallTriggerEntered += SetUpPlatforms;
        GameEvents.Instance.OnGameStarts += SetUpPlatformsAfterStart;
        GameEvents.Instance.OnLevelIncreased += IncreasePlatformSpawnValues;
        SetupPlatformsBeforeStart(8.5f);
    }

    private void SetUpPlatformsAfterStart()
    {
        var activePlatform = platforms.First(platform => platform.activeSelf == true);
        var remainingPlatforms = platforms.Where(platform => platform.activeSelf == false).ToList();

        var startPosition = activePlatform.transform.position;
        for (int i = 0; i < 6; i++)
        {
            var platform = remainingPlatforms[i];
            platform.transform.position = new Vector3(startPosition.x + i + 1, startPosition.y);
            platform.SetActive(true);
        }
    }

    private void SetupPlatformsBeforeStart(float firstPlatformXPos)
    {
        int platformsCount = spawnMultiplePlatforms ? 25 : 10;
        Enumerable.Range(0, platformsCount).ToList().ForEach(index =>
        {
            var newPlatform = Instantiate(platform, new Vector3(firstPlatformXPos, 5.4f), Quaternion.identity);
            if(index != 0)
                newPlatform.SetActive(false);
            platforms.Add(newPlatform);
        });
    }

    private void SetUpPlatforms(PlayerWallEntered player)
    {
        factor *= -1f;
        platforms.ForEach(platform => platform.SetActive(false));
        xPos = player.PlayerPosition.x + Random.Range(firstPlatformSpawnLocation.minX, firstPlatformSpawnLocation.maxX) * factor;
        yPos = player.PlayerPosition.y - Random.Range(firstPlatformSpawnLocation.minY, firstPlatformSpawnLocation.maxY);
        yPos = Mathf.Clamp(yPos, 2f, 7f);

        var firstPlatform = platforms[0];
        firstPlatform.transform.position = new Vector3(xPos, yPos);
        firstPlatform.SetActive(true);

        bool spawnAdditional = false;
        Vector3 lastlySpawnedPosition = Vector3.zero;
        foreach (var platform in platforms.Where(p => p.activeSelf == false))
        {
            if (!spawnAdditional)
            {
                xPos += Random.Range(spaceBetweenPlatforms.minX, spaceBetweenPlatforms.maxX) * factor;
                yPos += Random.Range(spaceBetweenPlatforms.minY, spaceBetweenPlatforms.maxY);
                

                yPos = Mathf.Clamp(yPos, 2f, 7f);

                if (xPos < 2 || xPos > 15)
                    break;

                lastlySpawnedPosition = new Vector3(xPos, yPos);
            } 
            if(spawnAdditional && lastlySpawnedPosition.y + 1.5f <= 8 && lastlySpawnedPosition.y - 1.5f >= 1.5f)
            {
                float additionalYPos = 0;
                if (UnityEngine.Random.Range(0,2) == 1)
                {
                    additionalYPos = Random.Range(lastlySpawnedPosition.y + 1.5f, 8f);
                } 
                else
                {
                    additionalYPos = Random.Range(1.5f, lastlySpawnedPosition.y - 1.5f);
                }
                lastlySpawnedPosition = new Vector3(lastlySpawnedPosition.x + Random.Range(-0.5f, 0.5f), additionalYPos);
            }

            platform.transform.position = lastlySpawnedPosition;
            platform.SetActive(true);
            platform.GetComponent<Platform>().GenerateCoinRandomly();
            if(spawnAdditional)
            {
                spawnAdditional = false;
                continue;
            }
            if(spawnMultiplePlatforms)
            {
                spawnAdditional = Random.Range(0, 2) == 1 ? true : false;
            }
        }
    }

    private void IncreasePlatformSpawnValues()
    {
        firstPlatformSpawnLocation.minX += spaceIncreaser;
        firstPlatformSpawnLocation.maxX += spaceIncreaser;
        firstPlatformSpawnLocation.minY += spaceIncreaser;
        firstPlatformSpawnLocation.maxY += spaceIncreaser;

        spaceBetweenPlatforms.minX += spaceIncreaser;
        spaceBetweenPlatforms.maxX += spaceIncreaser;
        spaceBetweenPlatforms.minY += spaceIncreaser;
        spaceBetweenPlatforms.maxY += spaceIncreaser;
    }
}
