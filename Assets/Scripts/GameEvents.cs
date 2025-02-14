﻿using Assets.Scripts.Events;
using System;
using UnityEngine;

public class GameEvents : MonoBehaviour
{
    public static GameEvents Instance { get; private set; }

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

    public event Action<PlayerWallEntered> OnWallTriggerEntered;
    public event Action OnBankTriggerEntered;
    public event Action OnCoinTriggerEntered;
    public event Action OnSpikeTriggerEntered;
    public event Action OnGameStarts;
    public event Action<ItemInShopBought> OnItemInShopBought;
    public event Action OnLevelIncreased;

    public void HandleWallTriggerEntered(PlayerWallEntered eventData)
    {
        OnWallTriggerEntered?.Invoke(eventData);
    }

    public void HandleBankTriggerEntered()
    {
        OnBankTriggerEntered?.Invoke();
    }

    public void HandleCoinTriggerEntered()
    {
        OnCoinTriggerEntered?.Invoke();
    }

    public void HandleSpikeTriggerEntered()
    {
        OnSpikeTriggerEntered?.Invoke();
    }

    public void HandleGameStart()
    {
        OnGameStarts?.Invoke();
    }

    public void HandleItemInShopBought(ItemInShopBought itemData)
    {
        OnItemInShopBought?.Invoke(itemData);
    }

    public void HandleIncreasedDifficulty()
    {
        OnLevelIncreased?.Invoke();
    }
}
