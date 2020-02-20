using Assets.Scripts;
using Assets.Scripts.Events;
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
        SetupPlatformsBeforeStart(10, 5);
    }

    private void SetupPlatformsBeforeStart(int count, int startX)
    {
        for (int i = 0; i < count; i++)
        {
            var newPlatform = Instantiate(platform, new Vector3(startX++, 5), Quaternion.identity);
            platforms.Add(newPlatform);
        }
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

        foreach (var platform in platforms.Where(p => p.activeSelf == false))
        {
            xPos += Random.Range(SpaceBetweenPlatforms.minX, SpaceBetweenPlatforms.maxX) * factor;
            yPos += Random.Range(SpaceBetweenPlatforms.minY, SpaceBetweenPlatforms.maxY);

            yPos = Mathf.Clamp(yPos, 2f, 7f);

            if (xPos < 1 || xPos > 16)
                break;


            platform.transform.position = new Vector3(xPos, yPos);
            platform.SetActive(true);
        }
    }
}
