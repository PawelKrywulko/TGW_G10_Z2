using Assets.Scripts.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] Text bankText = default;
    [SerializeField] Text collectedCoinsText = default;

    int bankedCoins = 0;
    int collectedCoins = 0;

    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        GameEvents.Instance.OnBankTriggerEntered += UpdateBankedCoins;
        GameEvents.Instance.OnCoinTriggerEntered += UpdateCollectedCoins;
        GameEvents.Instance.OnSpikeTriggerEntered += RemoveCollectedCoins;
    }

    private void UpdateBankedCoins()
    {
        bankedCoins += collectedCoins;
        collectedCoins = 0;
        RefreshGUI();        
    }

    public void UpdateCollectedCoins()
    {
        collectedCoins++;
        RefreshGUI();
    }

    public void RemoveCollectedCoins()
    {
        collectedCoins = 0;
        RefreshGUI();
    }

    private void RefreshGUI()
    {
        collectedCoinsText.text = collectedCoins.ToString();
        bankText.text = bankedCoins.ToString();
    }
}
