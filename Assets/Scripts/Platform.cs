using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts;

public class Platform : MonoBehaviour
{
    [SerializeField] GameObject coin;
    [SerializeField] MinMaxPosition coinPosition = default;
    
    GameObject coinInstance;
    bool firstStart = true;

    void Awake()
    {
        coinInstance = Instantiate(coin);
        coinInstance.transform.parent = transform;
    }

    void OnEnable()
    {
        coinInstance.SetActive(false);
      
        if (!firstStart && Random.Range(0, 2) == 1)
        {
            coinInstance.transform.localPosition = new Vector2(Random.Range(coinPosition.minX, coinPosition.maxX), Random.Range(coinPosition.minY, coinPosition.maxY));
             coinInstance.SetActive(true);
        }
        firstStart = false;
    }
}
