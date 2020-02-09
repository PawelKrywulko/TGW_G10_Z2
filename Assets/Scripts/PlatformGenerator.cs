
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlatformGenerator : MonoBehaviour
{
    [SerializeField] GameObject platform;
    [SerializeField] Transform platformsParent;

    List<GameObject> platforms = new List<GameObject>();
    Player player;
    float factor = 1;

    void Awake()
    {
        InitializePlatformsList(10);
    }

    void Start()
    {
        player = FindObjectOfType<Player>();
        SetupPlatformsBeforeStart(5);
    }

    private void SetupPlatformsBeforeStart(int startX)
    {
        platforms.ForEach(platform =>
        {
            platform.transform.position = new Vector3(startX++, 5);
            platform.SetActive(true);
        });
    }

    void InitializePlatformsList(int count)
    {
        for (int i = 0; i < count; i++)
        {
            var newPlatform = Instantiate(platform, Vector3.zero, Quaternion.identity);
            newPlatform.SetActive(false);
            platforms.Add(newPlatform);
        }
    }

    public void SetUpPlatforms(int count)
    {
        factor *= -1f;
        platforms.ForEach(platform => platform.SetActive(false));
        float xPos = player.GetCurrentPosition().x + Random.Range(1f, 2f) * factor;
        float yPos = player.GetCurrentPosition().y - Random.Range(1.5f, 2.5f);

        for (int i = 0; i < count; i++)
        {
            var platform = platforms[i];
            platform.transform.position = new Vector3(xPos, yPos);

            if (xPos > 1 && xPos < 16)
            {
                platform.SetActive(true);
            }

            xPos += Random.Range(2f, 3f) * factor;
            yPos += Random.Range(-1f, 1f);
            yPos = Mathf.Clamp(yPos, 2f, 7f);
        }
    }
}
