using Assets.Scripts.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] Text bankText;
    [SerializeField] Text collectedCoinsText;
    [SerializeField] Text wallsTouchedText;
    [SerializeField] Text tapToStartText;

    string collectedCoinsStr = "COINS COLLECED:";
    string wallsTouchedStr = "WALLS TOUCHED:";

    int bankedCoins = 0;
    int collectedCoins = 0;
    int wallsTouched = 0;

    bool hasGameStarted = false;

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

    void Start()
    {
        SubscribeForEvents();
        ClearTexts();
        StartCoroutine(GameStarts());
    }

    private void ClearTexts()
    {
        bankText.text = string.Empty;
        collectedCoinsText.text = string.Empty;
        wallsTouchedText.text = string.Empty;
    }

    private IEnumerator GameStarts()
    {
        while(!Input.GetMouseButton(0) && Input.touchCount != 1)
        {
            yield return null;
        }

        tapToStartText.text = string.Empty;
        hasGameStarted = true;
        RefreshGUI();
        GameEvents.Instance.HandleGameStart();
    }

    private void SubscribeForEvents()
    {
        GameEvents.Instance.OnBankTriggerEntered += UpdateBankedCoins;
        GameEvents.Instance.OnCoinTriggerEntered += UpdateCollectedCoins;
        GameEvents.Instance.OnSpikeTriggerEntered += ManagePlayersDeath;
        GameEvents.Instance.OnWallTriggerEntered += UpdateWallsTouched;
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

    public void ManagePlayersDeath()
    {
        collectedCoins = 0;
        wallsTouched = 0;
        RefreshGUI();
        StartCoroutine(RestartGame());
    }

    private void UpdateWallsTouched(PlayerWallEntered obj)
    {
        wallsTouched++;
        RefreshGUI();
    }

    private void RefreshGUI()
    {
        if (hasGameStarted)
        {
            collectedCoinsText.text = $"{collectedCoinsStr} {collectedCoins}";
            wallsTouchedText.text = $"{wallsTouchedStr} {wallsTouched}";
            bankText.text = bankedCoins.ToString();
        }
    }

    private IEnumerator RestartGame()
    {
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(0);
    }
}
