using Assets.Scripts;
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
    Player player;
    float factor = 1;

    void Start()
    {
        player = FindObjectOfType<Player>();
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

    public void SetUpPlatforms(int count)
    {
        factor *= -1f;
        platforms.ForEach(platform => platform.SetActive(false));
        float xPos = player.GetCurrentPosition().x + Random.Range(firstPlatformSpawnLocation.minX, firstPlatformSpawnLocation.maxX) * factor;
        float yPos = player.GetCurrentPosition().y - Random.Range(firstPlatformSpawnLocation.minY, firstPlatformSpawnLocation.maxY);

        for (int i = 0; i < count; i++)
        {
            var platform = platforms[i];
            platform.transform.position = new Vector3(xPos, yPos);

            if (xPos > 1 && xPos < 16)
            {
                platform.SetActive(true);
            }

            xPos += Random.Range(SpaceBetweenPlatforms.minX, SpaceBetweenPlatforms.maxX) * factor;
            yPos += Random.Range(SpaceBetweenPlatforms.minY, SpaceBetweenPlatforms.maxY);
            yPos = Mathf.Clamp(yPos, 2f, 7f);
        }
    }
}
