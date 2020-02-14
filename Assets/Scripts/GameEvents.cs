using Assets.Scripts.Events;
using System;
using System.Collections;
using System.Collections.Generic;
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
            DontDestroyOnLoad(gameObject);
        }
    }

    public event Action<PlayerWallEntered> OnWallTriggerEntered;

    public void HandleWallTriggerEntered(PlayerWallEntered eventData)
    {
        OnWallTriggerEntered?.Invoke(eventData);
    }
}
