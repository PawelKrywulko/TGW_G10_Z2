using Assets.Scripts.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    [SerializeField] List<GameObject> tipTexts;

    public static Tutorial Instance { get; private set; }
    private Dictionary<GameObject, bool> tipsDict = new Dictionary<GameObject, bool>();
    private bool tutorialNotFinished;
    private Player player;
    private float currentPlayerVelocity;
    private bool canContinue = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        tutorialNotFinished = PlayerPrefs.GetInt("TutorialNotFinished", 1) == 1 ? true : false;
        if (tutorialNotFinished)
        {
            tipTexts.ForEach(txtObj => tipsDict.Add(txtObj, false));
            GameEvents.Instance.OnGameStarts += RunTutorial;
            GameEvents.Instance.OnWallTriggerEntered += RunTipTwo;
        }
    }

    private void RunTipTwo(PlayerWallEntered obj)
    {
        if(tipsDict[tipTexts[1]] == false)
            StartCoroutine(TipTwo());
    }

    private void RunTutorial()
    {
        player = FindObjectOfType<Player>();
        StartCoroutine(StartTutorial());
    }

    private IEnumerator StartTutorial()
    {
        yield return new WaitForSeconds(0.7f);
        if(tipsDict[tipTexts[0]] == false)
            StartCoroutine(TipOne());
    }

    IEnumerator TipOne()
    {
        var tipOneObj = tipTexts[0];
        PausePlayer();
        //bool singleTap = false;
        bool doubleTap = false;
        tipOneObj.SetActive(true);
        //yield return new WaitForSeconds(0.5f);
        while (doubleTap != true)
        {
            doubleTap = IsDoubleTap();
            //if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began)
            //{
            //    singleTap = true;
            //}
            yield return null;
        }
        tipOneObj.SetActive(false);
        tipsDict[tipOneObj] = true;
        ResumePlayer();
    }

    IEnumerator TipTwo()
    {
        var tipObj = tipTexts[1];
        var tmp = Time.fixedDeltaTime;
        Time.timeScale = 0.01f;
        Time.fixedDeltaTime = tmp * Time.timeScale;
        player.SwitchJumpFunction();
        tipObj.SetActive(true);

        while(canContinue == false)
        {
            yield return null;
        }

        tipObj.SetActive(false);
        player.SwitchJumpFunction();
        Time.timeScale = 1f;
        Time.fixedDeltaTime = tmp * Time.timeScale;
        canContinue = false;
        tipsDict[tipObj] = true;

        StartCoroutine(TipThree());
    }

    private IEnumerator TipThree()
    {
        if (tipsDict[tipTexts[2]] == false)
        {
            var tipObj = tipTexts[2];
            var tmp = Time.fixedDeltaTime;
            Time.timeScale = 0.01f;
            Time.fixedDeltaTime = tmp * Time.timeScale;
            player.SwitchJumpFunction();
            tipObj.SetActive(true);

            while (canContinue == false)
            {
                yield return null;
            }

            tipObj.SetActive(false);
            player.SwitchJumpFunction();
            Time.timeScale = 1f;
            Time.fixedDeltaTime = tmp * Time.timeScale;
            canContinue = false;
            tipsDict[tipObj] = true;
        }
        
        StartCoroutine(TipFour());
    }

    private IEnumerator TipFour()
    {
        if (tipsDict[tipTexts[3]] == false)
        {
            var tipObj = tipTexts[3];
            var tmp = Time.fixedDeltaTime;
            Time.timeScale = 0.01f;
            Time.fixedDeltaTime = tmp * Time.timeScale;
            player.SwitchJumpFunction();
            tipObj.SetActive(true);

            while (canContinue == false)
            {
                yield return null;
            }

            tipObj.SetActive(false);
            player.SwitchJumpFunction();
            Time.timeScale = 1f;
            Time.fixedDeltaTime = tmp * Time.timeScale;
            canContinue = false;
            tipsDict[tipObj] = true;
        }

        StartCoroutine(TipFive());
    }

    private IEnumerator TipFive()
    {
        if (tipsDict[tipTexts[4]] == false)
        {
            var tipObj = tipTexts[4];
            var tmp = Time.fixedDeltaTime;
            Time.timeScale = 0.01f;
            Time.fixedDeltaTime = tmp * Time.timeScale;
            player.SwitchJumpFunction();
            tipObj.SetActive(true);

            while (canContinue == false)
            {
                yield return null;
            }

            tipObj.SetActive(false);
            player.SwitchJumpFunction();
            Time.timeScale = 1f;
            Time.fixedDeltaTime = tmp * Time.timeScale;
            canContinue = false;
            tipsDict[tipObj] = true;
        }
    }

    private void ResumePlayer()
    {
        player.SetMoveVelocity(currentPlayerVelocity);
    }

    private void PausePlayer()
    {
        currentPlayerVelocity = player.GetMoveVelocity();
        player.SetMoveVelocity(0);
    }

    public bool IsDoubleTap()
    {
        bool result = false;
        float MaxTimeWait = 0.8f;

        if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            float DeltaTime = Input.GetTouch(0).deltaTime;

            if (DeltaTime > 0 && DeltaTime < MaxTimeWait)
                result = true;
        }
        return result;
    }

    public void ContinueTutorial()
    {
        canContinue = true;
    }

    public void FinishTutorial()
    {
        canContinue = true;
        tutorialNotFinished = false;
        PlayerPrefs.SetInt("TutorialNotFinished", 0);
    }
}
