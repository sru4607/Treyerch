using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class UIController : MonoBehaviour
{
    [TabGroup("General")]
    [Header("References")]
    public LevelManager levelManager;
    [TabGroup("General")]
    public CanvasGroup activeTimer;

    [TabGroup("Bomb")]
    [Header("Bomb")]
    public Transform bomb;
    [TabGroup("Bomb")]
    public Image bombCrack;
    [TabGroup("Bomb")]
    public GameObject bombExplode;
    [TabGroup("Bomb")]
    public Animator bombExplodeAnim;

    [TabGroup("Bomb")]
    [Header("Fuse")]
    public Animator explosionTrack;
    [TabGroup("Bomb")]
    public Animator explosionRotation;
    [TabGroup("Bomb")]
    public Image fuse;
    [TabGroup("Bomb")]
    public Color startingFuse;
    [TabGroup("Bomb")]
    public Color endingFuse;

    [TabGroup("UI Elements")]
    [Header("Timer")]
    public Text mainTimer;
    [TabGroup("UI Elements")]
    public Text secondaryTimer;

    [TabGroup("UI Elements")]
    [Header("Counter")]
    public Text hundreds;
    [TabGroup("UI Elements")]
    public Animator hundredsAnim;
    [TabGroup("UI Elements")]
    public Text tens;
    [TabGroup("UI Elements")]
    public Animator tensAnim;
    [TabGroup("UI Elements")]
    public Text ones;
    [TabGroup("UI Elements")]
    public Animator onesAnim;

    [TabGroup("UI Elements")]
    [Header("Score")]
    public Text scoreCount;
    [TabGroup("UI Elements")]
    public Text totalInLevelBananas;
    [TabGroup("UI Elements")]
    public int scoreIncrement = 10;
    [TabGroup("UI Elements")]
    public Transform bananaTravelPoint;

    private bool hasStarted = false;
    private bool crackAppeared = false;
    private bool doExplode = false;
    private bool didExplode = false;
    private bool startedFlashing = false;
    private bool didPop = false;

    private int lastSecondSaved = 9999;
    private int bananasTotal = 0;
    private int playerScore = 0;
    private int currentBananasOnes = 0;
    private int currentBananasTens = 0;
    private int currentBananasHundreds = 0;

    private int currentVisibleScore = 0;
    private int totalScoreDifference = 0;

    private BananaCollectable[] allBananas;

    [HideInInspector]
    public static UIController instance; //Singleton

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        lastSecondSaved = levelManager.totalStageSeconds;
        explosionTrack.speed = 0.0f;

        //Reset Score
        scoreCount.text = "0";

        int foundBananas = 0;
        allBananas = GameObject.FindObjectsOfType<BananaCollectable>();
        foreach(BananaCollectable banana in allBananas)
        {
            foundBananas += banana.bananaCount;
        }

        totalInLevelBananas.text = "             / " + foundBananas.ToString("000");
    }

    // Update is called once per frame
    void Update()
    {
        if (currentVisibleScore != playerScore)
        {
            totalScoreDifference = playerScore - currentVisibleScore;

            if (totalScoreDifference > 0) //Positive
            {
                int moneyToAdd = scoreIncrement;

                if (totalScoreDifference < scoreIncrement)
                {
                    moneyToAdd = totalScoreDifference;
                }

                currentVisibleScore += moneyToAdd;
            }
            else if (totalScoreDifference < 0) //negative
            {
                int moneyToAdd = scoreIncrement;

                if (Mathf.Abs(totalScoreDifference) < scoreIncrement)
                {
                    moneyToAdd = totalScoreDifference;
                }

                currentVisibleScore -= moneyToAdd;
            }

            scoreCount.text = (currentVisibleScore.ToString());
        }



        if (!hasStarted)
        {
            if (levelManager.allLevelTimers[0].mainTimer.text != "" + 999)
            {
                mainTimer.text = levelManager.allLevelTimers[0].mainTimer.text;
                secondaryTimer.text = ":" + levelManager.allLevelTimers[0].secondaryTimer.text;
            }

            if (levelManager.timeLeft != levelManager.totalStageSeconds)
            {
                hasStarted = true;
                explosionRotation.enabled = true;
            }
        }

        if (PlayerController.instance)
        {
            if (!PlayerController.instance.isMovable)
            {
                explosionRotation.enabled = false;
            }
            else
            {
                if (explosionRotation.enabled == false)
                {
                    explosionRotation.enabled = true;
                }
            }

            if (levelManager.timeRanOut == true)
            {
                levelManager.timeRanOut = false;
                doExplode = true;
            }

            if (!doExplode && PlayerController.instance.isMovable)
            {
                mainTimer.text = levelManager.allLevelTimers[0].mainTimer.text;
                secondaryTimer.text = ":" + levelManager.allLevelTimers[0].secondaryTimer.text;

                float timeLeftRatio = levelManager.timeLeft / levelManager.totalStageSeconds;
                fuse.fillAmount = timeLeftRatio;
                fuse.color = Color.Lerp(startingFuse, endingFuse, 1.0f - timeLeftRatio);
                explosionTrack.Play("Path", 0, 1.0f - timeLeftRatio);

                int currentSecond = Mathf.CeilToInt(levelManager.timeLeft);
                if (currentSecond < lastSecondSaved)
                {
                    lastSecondSaved = currentSecond;

                    if (levelManager.timeLeft <= 10)
                    {
                        if (levelManager.timeLeft > 1)
                        {
                            StartCoroutine(DoBombPump());
                        }
                        else if (!didPop)
                        {
                            didPop = true;
                            StartCoroutine(DoBombPop());
                        }
                    }

                    if (!startedFlashing && levelManager.timeLeft <= 3)
                    {
                        startedFlashing = true;
                        StartCoroutine(DoCrackColor());
                    }
                }

                if (!crackAppeared && levelManager.timeLeft <= 8)
                {
                    crackAppeared = true;
                    MakeCrackAppear();
                }
            }
            else if (doExplode)
            {
                if (!didExplode)
                {
                    didExplode = true;

                    mainTimer.text = "000";
                    secondaryTimer.text = ":" + "00";

                    Sequence fadeIn = DOTween.Sequence();
                    fadeIn.Append(activeTimer.DOFade(0.0f, 0.1f));
                    bombExplode.SetActive(true);
                    bombExplodeAnim.Play("Burst");

                    Invoke("ResetTimer", 2.5f);
                }
            }
        }
    }

    public void ResetScore()
    {
        playerScore = 0;
        bananasTotal = 0;

        ones.text = "0";
        tens.text = "0";
        hundreds.text = "0";

        currentBananasOnes = 0;
        currentBananasTens = 0;
        currentBananasHundreds = 0;

        currentVisibleScore = 0;
        totalScoreDifference = 0;

        scoreCount.text = "0";

        foreach (BananaCollectable banana in allBananas)
        {
            banana.gameObject.SetActive(true);
            banana.ResetBanana();
        }

        foreach (BananaCollectable banana in allBananas)
        {
            banana.animator.Play("Spin");
        }
    }

    public void ResetTimer()
    {
        doExplode = false;
        didExplode = false;

        startedFlashing = false;
        crackAppeared = false;
        didPop = false;

        bomb.localScale = Vector3.one;
        Color defaultBombColor = Color.white;
        defaultBombColor.a = 0.0f;

        bombCrack.color = defaultBombColor;

        mainTimer.text = levelManager.allLevelTimers[0].mainTimer.text;
        secondaryTimer.text = ":" + levelManager.allLevelTimers[0].secondaryTimer.text;

        fuse.fillAmount = 1.0f;
        fuse.color = startingFuse;
        explosionTrack.Play("Path", 0, 0.0f);

        bombExplode.SetActive(false);
        Sequence fadeIn = DOTween.Sequence();
        fadeIn.Append(activeTimer.DOFade(1.0f, 1f));
    }

    public IEnumerator DoCrackColor()
    {
        float flashLength = levelManager.timeLeft / 3.0f;

        if(flashLength < 0.1f)
        {
            flashLength = 0.1f;
        }

        Sequence color = DOTween.Sequence();
        color.Append(bombCrack.DOColor(Color.red, flashLength/2));
        yield return new WaitUntil(() => (bombCrack.color == Color.red));
        color.Append(bombCrack.DOColor(Color.white, flashLength/2));
        yield return new WaitUntil(() => (bombCrack.color == Color.white));

        if (!doExplode)
        {
            StartCoroutine(DoCrackColor());
        }
    }

    public IEnumerator DoBombPump()
    {
        Sequence scaleUp = DOTween.Sequence();
        scaleUp.Append(bomb.DOScale(1.2f, 0.4f));
        yield return new WaitUntil(() => (bomb.localScale.x == 1.2f));
        scaleUp.Append(bomb.DOScale(1.0f, 0.5f));
    }

    public IEnumerator DoBombPop()
    {
        Sequence scaleUp = DOTween.Sequence();
        scaleUp.Append(bomb.DOScale(1.2f, 0.8f));
        yield return new WaitUntil(() => (bomb.localScale.x == 1.2f));
        scaleUp.Append(bomb.DOScale(1.0f, 0.1f));
        yield return new WaitUntil(() => (bomb.localScale.x == 1.0f));
        scaleUp.Append(bomb.DOScale(1.5f, 0.2f).SetEase(Ease.Linear));
    }

    public void MakeCrackAppear()
    {
        Sequence fadeIn = DOTween.Sequence();
        fadeIn.Append(bombCrack.DOFade(1.0f, 5f));
    }

    public void AddBanana(int amount)
    {
        bananasTotal += amount;

        currentBananasOnes+=amount;        

        if(currentBananasOnes > 9)
        {
            currentBananasTens += Mathf.FloorToInt(currentBananasOnes / 10);
            currentBananasOnes = currentBananasOnes%10;
            tensAnim.SetTrigger("Bounce");
        }
        else
        {
            onesAnim.SetTrigger("Bounce");
        }

        if (currentBananasTens > 9)
        {
            currentBananasHundreds += Mathf.FloorToInt(currentBananasHundreds / 100);
            currentBananasTens = currentBananasTens % 100;
            hundredsAnim.SetTrigger("Bounce");
        }

        if (currentBananasHundreds > 9)
        {
            currentBananasOnes = 9;
            currentBananasTens = 9;
            currentBananasHundreds = 9;

            onesAnim.SetTrigger("Bounce");
            tensAnim.SetTrigger("Bounce");
            hundredsAnim.SetTrigger("Bounce");
        }

        ones.text = currentBananasOnes.ToString("0");
        tens.text = currentBananasTens.ToString("0");
        hundreds.text = currentBananasHundreds.ToString("0");
    }

    public void AddToScore(int toAdd)
    {
        if (playerScore + toAdd > 99999999)
        {
            playerScore = 99999999;
        }
        else
        {
            playerScore += toAdd;
        }
    }
}
