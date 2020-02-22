using Assets.Scripts.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] Text titleText;
    [SerializeField] Text bankText;
    [SerializeField] Text collectedCoinsText;
    [SerializeField] Text wallsTouchedText;
    [SerializeField] Text tapToStartText;
    [SerializeField] Text allCoinsText;
    [SerializeField] Text bestCoinsText;
    [SerializeField] Text bestWallsText;
    [SerializeField] GameObject player;
    [SerializeField] GameObject bank;
    [SerializeField] GameObject coins;

    string collectedCoinsStr = "COINS COLLECED:";
    string wallsTouchedStr = "WALLS TOUCHED:";
    string bestCoinsStr = "THE MOST COINS BANKED:";
    string bestWallsStr = "THE MOST WALLS TOUCHED:";

    int bankedCoins = 0;
    int collectedCoins = 0;
    int wallsTouched = 0;
    int bestCoins = 0;
    int bestWalls = 0;
    int allCoins = 0;

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
        ClearGameTexts();
        player = Instantiate(player, new Vector2(8.5f, 8), Quaternion.identity);
        StartCoroutine(GameStarts());
    }

    private void ClearGameTexts()
    {
        bankText.text = string.Empty;
        collectedCoinsText.text = string.Empty;
        wallsTouchedText.text = string.Empty;
    }

    private void ClearMenuTexts()
    {
        tapToStartText.text = string.Empty;
        titleText.text = string.Empty;
        coins.SetActive(false);
        bestCoinsText.text = string.Empty;
        bestWallsText.text = string.Empty;
    }

    private IEnumerator GameStarts()
    {
        while(!Input.GetMouseButton(0) && Input.touchCount != 1)
        {
            yield return null;
        }

        ClearMenuTexts();
        hasGameStarted = true;
        bank = Instantiate(bank, new Vector2(8.5f, 7.93f), Quaternion.identity);
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
        if (bankedCoins > bestCoins)
            bestCoins = bankedCoins;
        if (wallsTouched > bestWalls)
            bestWalls = wallsTouched;
        allCoins += bankedCoins;

        collectedCoins = 0;
        wallsTouched = 0;
        RefreshGUI();

        coins.SetActive(true);
        bestCoinsText.text = $"{bestCoinsStr} {bestCoins}";
        bestWallsText.text = $"{bestWallsStr} {bestWalls}";
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
            allCoinsText.text = allCoins.ToString();
            bankText.text = bankedCoins.ToString();
        }
    }

    private IEnumerator RestartGame()
    {
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(0);
    }
}
