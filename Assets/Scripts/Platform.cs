using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts;

public class Platform : MonoBehaviour
{
    [SerializeField] GameObject coin = default;
    [SerializeField] MinMaxPosition coinPosition = default;
    
    GameObject coinInstance;

    void Awake()
    {
        coinInstance = Instantiate(coin);
        coinInstance.SetActive(false);
        coinInstance.transform.parent = transform;
    }

    public void GenerateCoinRandomly()
    {
        if (Random.Range(0, 2) == 1)
        {
            coinInstance.transform.localPosition = new Vector2(Random.Range(coinPosition.minX, coinPosition.maxX), Random.Range(coinPosition.minY, coinPosition.maxY));
            coinInstance.SetActive(true);
        }
    }
}
