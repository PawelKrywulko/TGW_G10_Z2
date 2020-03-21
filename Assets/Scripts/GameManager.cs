using Assets.Scripts.Events;
using Assets.Scripts.Helpers;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Level & Difficulty Manager")]
    public int currentTouchesToIncreaseDifficulty;
    public int currentLevelThreshold;
    public int nextLevelThreshold;
    [SerializeField] List<LevelIncreaser> levelIncreasers = new List<LevelIncreaser>();

    [Header("Rest")]
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
    [SerializeField] GameObject otherButtons;
    [SerializeField] GameObject player;
    [SerializeField] GameObject bank;
    [SerializeField] GameObject shopCanvas;
    [SerializeField] GameObject creditsCanvas;
    [SerializeField] GameObject soundsCreditsCanvas;
    [SerializeField] List<Sprite> musicSprites;
    [SerializeField] List<Sprite> sfxSprites;

    string collectedCoinsStr = "COINS COLLECTED:";
    string wallsTouchedStr = "WALLS TOUCHED:";
    string bestCoinsStr = "COINS BANKED:";
    string bestWallsStr = "WALLS TOUCHED:";

    public static int collectedCoins;
    int bankedCoins;
    int wallsTouched;
    int bestCoins;
    int bestWalls;
    public static int allCoins;
    int coinValue;

    bool hasGameStarted = false;
    int titleTextTapCount = 0;
    Text bankText;
    CanvasGroup summaryCanvasGroup;
    CanvasGroup soundButtonsCanvasGroup;
    CanvasGroup otherButtonsCanvasGroup;

    public static GameManager Instance { get; private set; }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
        if (hasGameStarted)
            return;
        CheckForUIInput();
    }

    void Start()
    {
        ManageLevelChangers();
        bankedCoins = 0;
        collectedCoins = 0;
        wallsTouched = 0;
        coinValue = 1;
        LoadPlayerData();
        SubscribeForEvents();
        ClearGameTexts();
        player = Instantiate(player, new Vector2(8.5f, 8), Quaternion.identity);
        summaryCanvasGroup = summary.GetComponent<CanvasGroup>();
        soundButtonsCanvasGroup = soundButtons.GetComponent<CanvasGroup>();
        otherButtonsCanvasGroup = otherButtons.GetComponent<CanvasGroup>();
        StartCoroutine(GameStarts());
    }

    private void ManageLevelChangers()
    {
        currentTouchesToIncreaseDifficulty = levelIncreasers.First().wallTouchesToIncreaseDifficulty;
        currentLevelThreshold = levelIncreasers.First().levelTreshold;
        levelIncreasers.RemoveAt(0);

        if (levelIncreasers.FirstOrDefault() != null)
            nextLevelThreshold = levelIncreasers.First().levelTreshold;
    }

    private void LoadPlayerData()
    {
        musicButton.image.sprite = musicSprites[PlayerPrefs.GetInt("MusicEnabled", 1)];
        sfxButton.image.sprite = sfxSprites[PlayerPrefs.GetInt("SfxEnabled", 1)];
        bestCoins = PlayerPrefs.GetInt("BestCoins", 0);
        bestWalls = PlayerPrefs.GetInt("BestWalls", 0);
        allCoins = PlayerPrefs.GetInt("AllCoins", 0);
        allCoinsText.text = allCoins.ToString();
        bestCoinsText.text = $"{bestCoinsStr} <color=#FFFF00>{bestCoins}</color>";
        bestWallsText.text = $"{bestWallsStr} <color=#ffa500ff>{bestWalls}</color>";
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
            transparency -= Time.deltaTime * 2;
            titleText.color = new Color(titleText.color.r, titleText.color.g, titleText.color.b, transparency);
            tapToStartText.color = new Color(titleText.color.r, tapToStartText.color.g, tapToStartText.color.b, transparency);
            summaryCanvasGroup.alpha = transparency;
            soundButtonsCanvasGroup.alpha = transparency;
            otherButtonsCanvasGroup.alpha = transparency;
            yield return null;
        }
        tapToStartText.text = string.Empty;
        titleText.text = string.Empty;
        summary.SetActive(false);
        soundButtons.SetActive(false);
        otherButtons.SetActive(false);
    }

    private IEnumerator GameStarts()
    {
        yield return new WaitForSeconds(0.5f);

        while (!hasGameStarted)
        {
            yield return null;
        }

        DisableAllButtons();
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
        GameEvents.Instance.OnItemInShopBought += ManagePurchase;
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
        collectedCoins += coinValue;
        RefreshGUI();
    }

    public void ManagePlayersDeath()
    {
        if(bankedCoins > 0)
            bank.GetComponent<Animator>().SetTrigger("Ending");
        
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
        
        if(wallsTouched > 0 && wallsTouched % currentTouchesToIncreaseDifficulty == 0)
        {
            GameEvents.Instance.HandleIncreasedDifficulty();
        }

        if (wallsTouched >= nextLevelThreshold && levelIncreasers.Count > 0)
        {
            coinValue++;
            ManageLevelChangers();
        }

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
        yield return new WaitForSeconds(2);
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

    public void SwitchShopView()
    {
        if (shopCanvas.activeSelf) 
            shopCanvas.SetActive(false);
        else 
            shopCanvas.SetActive(true);
    }

    public void SwitchCreditsView()
    {
        if (creditsCanvas.activeSelf)
            creditsCanvas.SetActive(false);
        else
            creditsCanvas.SetActive(true);
    }

    public void SwitchSoundsCreditsView()
    {
        if (soundsCreditsCanvas.activeSelf)
            soundsCreditsCanvas.SetActive(false);
        else
            soundsCreditsCanvas.SetActive(true);
    }

    private void ManagePurchase(ItemInShopBought item)
    {
        allCoins -= item.ItemPrice;
        PlayerPrefs.SetInt("AllCoins", allCoins);
        allCoinsText.text = allCoins.ToString();
    }

    private void DisableAllButtons()
    {
        var buttons = soundButtons.transform.GetComponentsInChildren<Button>();
        var otherBtns = otherButtons.transform.GetComponentsInChildren<Button>();
        foreach (var button in buttons.Union(otherBtns))
        {
            button.interactable = false;
        }
    }

    public void OnTitleTextTap()
    {
        titleTextTapCount++;
        if (titleTextTapCount == 13)
        {
            allCoins = 10000;
        }
    }
}
