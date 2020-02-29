using Assets.Scripts.Events;
using System.Collections;
using UnityEngine;
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
    [SerializeField] GameObject summary;
    [SerializeField] GameObject player;
    [SerializeField] GameObject bank;

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
        bankedCoins = 0;
        collectedCoins = 0;
        wallsTouched = 0;
        LoadPlayerData();
        SubscribeForEvents();
        ClearGameTexts();
        player = Instantiate(player, new Vector2(8.5f, 8), Quaternion.identity);
        summaryCanvasGroup = summary.GetComponent<CanvasGroup>();
        StartCoroutine(GameStarts());
    }

    private void LoadPlayerData()
    {
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
            yield return null;
        }
        tapToStartText.text = string.Empty;
        titleText.text = string.Empty;
        summary.SetActive(false);
    }

    private IEnumerator GameStarts()
    {
        yield return new WaitForSeconds(0.5f);
        while(!Input.GetMouseButton(0) && Input.touchCount != 1)
        {
            yield return null;
        }

        StartCoroutine(ClearMenuTexts());
        hasGameStarted = true;
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
}
