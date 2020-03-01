﻿using Assets.Scripts.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] Text titleText;
    [SerializeField] Text collectedCoinsText;
    [SerializeField] Text wallsTouchedText;
    [SerializeField] Text tapToStartText;
    [SerializeField] Text allCoinsText;
    [SerializeField] Text bestCoinsText;
    [SerializeField] Text bestWallsText;
    [SerializeField] Button musicButton;
    [SerializeField] Button sfxButton;
    [SerializeField] GameObject summary;
    [SerializeField] GameObject soundButtons;
    [SerializeField] GameObject player;
    [SerializeField] GameObject bank;
    [SerializeField] List<Sprite> musicSprites;
    [SerializeField] List<Sprite> sfxSprites;

    string collectedCoinsStr = "COINS COLLECED:";
    string wallsTouchedStr = "WALLS TOUCHED:";
    string bestCoinsStr = "COINS BANKED:";
    string bestWallsStr = "WALLS TOUCHED:";

    public static int collectedCoins;
    int bankedCoins;
    int wallsTouched;
    int bestCoins;
    int bestWalls;
    int allCoins;

    bool hasGameStarted = false;
    Text bankText;
    CanvasGroup summaryCanvasGroup;
    CanvasGroup musicButtonsCanvasGroup;

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

    void Update()
    {
        if (hasGameStarted)
            return;
        CheckForUIInput();
    }

    void Start()
    {
        bankedCoins = 0;
        collectedCoins = 0;
        wallsTouched = 0;
        LoadPlayerData();
        SubscribeForEvents();
        ClearGameTexts();
        player = Instantiate(player, new Vector2(8.5f, 8), Quaternion.identity);
        summaryCanvasGroup = summary.GetComponent<CanvasGroup>();
        musicButtonsCanvasGroup = soundButtons.GetComponent<CanvasGroup>();
        StartCoroutine(GameStarts());
    }

    private void LoadPlayerData()
    {
        musicButton.image.sprite = musicSprites[PlayerPrefs.GetInt("MusicEnabled")];
        sfxButton.image.sprite = sfxSprites[PlayerPrefs.GetInt("SfxEnabled")];
        bestCoins = PlayerPrefs.GetInt("BestCoins", 0);
        bestWalls = PlayerPrefs.GetInt("BestWalls", 0);
        allCoins = PlayerPrefs.GetInt("AllCoins", 0);
        allCoinsText.text = allCoins.ToString();
        bestCoinsText.text = $"{bestCoinsStr} {bestCoins}";
        bestWallsText.text = $"{bestWallsStr} {bestWalls}";
    }

    private void ClearGameTexts()
    {
        collectedCoinsText.text = string.Empty;
        wallsTouchedText.text = string.Empty;
    }

    private IEnumerator ClearMenuTexts()
    {
        float transparency = 1f;
        while (transparency > 0f)
        {
            transparency -= Time.deltaTime;
            titleText.color = new Color(titleText.color.r, titleText.color.g, titleText.color.b, transparency);
            tapToStartText.color = new Color(titleText.color.r, tapToStartText.color.g, tapToStartText.color.b, transparency);
            summaryCanvasGroup.alpha = transparency;
            musicButtonsCanvasGroup.alpha = transparency;
            yield return null;
        }
        tapToStartText.text = string.Empty;
        titleText.text = string.Empty;
        summary.SetActive(false);
        soundButtons.SetActive(false);
    }

    private IEnumerator GameStarts()
    {
        yield return new WaitForSeconds(0.5f);

        while (!hasGameStarted)
        {
            yield return null;
        }

        StartCoroutine(ClearMenuTexts());
        bank = Instantiate(bank, new Vector2(8.5f, 5.5f), Quaternion.identity);
        bankText = bank.transform.Find("Canvas").transform.Find("CoinsInBankText").GetComponent<Text>();
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
        AudioManager.Instance.Play("Bank", "SfxEnabled");
        bankedCoins += collectedCoins;
        collectedCoins = 0;
        RefreshGUI();        
    }

    public void UpdateCollectedCoins()
    {
        AudioManager.Instance.Play("Coin", "SfxEnabled");
        collectedCoins++;
        RefreshGUI();
    }

    public void ManagePlayersDeath()
    {
        AudioManager.Instance.Play("Death", "SfxEnabled");

        if (bankedCoins > bestCoins)
        {
            bestCoins = bankedCoins;
            PlayerPrefs.SetInt("BestCoins", bestCoins);
        }
        if (wallsTouched > bestWalls)
        {
            bestWalls = wallsTouched;
            PlayerPrefs.SetInt("BestWalls", bestWalls);
        }

        allCoins += bankedCoins;
        PlayerPrefs.SetInt("AllCoins", allCoins);
        StartCoroutine(RestartGame());
    }

    private void UpdateWallsTouched(PlayerWallEntered obj)
    {
        AudioManager.Instance.Play("Wall", "SfxEnabled");
        wallsTouched++;
        RefreshGUI();
    }

    private void RefreshGUI()
    {
        if (hasGameStarted)
        {
            collectedCoinsText.text = $"{collectedCoinsStr} <color=#FFFF00>{collectedCoins}</color>";
            wallsTouchedText.text = $"{wallsTouchedStr} <color=#ffa500ff>{wallsTouched}</color>";
            bankText.text = bankedCoins.ToString();
        }
    }

    private IEnumerator RestartGame()
    {
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(0);
    }

    public void SwitchMusic()
    {
        PlayerPrefs.SetInt("MusicEnabled", PlayerPrefs.GetInt("MusicEnabled", 1) == 1 ? 0 : 1);
        AudioManager.Instance.Play("Theme", "MusicEnabled");
        musicButton.image.sprite = musicSprites[PlayerPrefs.GetInt("MusicEnabled")];
    }

    public void SwitchSfx()
    {
        PlayerPrefs.SetInt("SfxEnabled", PlayerPrefs.GetInt("SfxEnabled", 1) == 1 ? 0 : 1);
        sfxButton.image.sprite = sfxSprites[PlayerPrefs.GetInt("SfxEnabled")];
    }

    private void CheckForUIInput()
    {
        #region PC Input
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        if (Input.GetMouseButtonDown(0))
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                hasGameStarted = true;
            }
        }
#endif
        #endregion

        #region MobileInput
#if UNITY_ANDROID && !UNITY_EDITOR
        if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            if (!EventSystem.current.IsPointerOverGameObject(Input.touches[0].fingerId))
            {
                hasGameStarted = true;
            }
        }
#endif
        #endregion
    }
}
