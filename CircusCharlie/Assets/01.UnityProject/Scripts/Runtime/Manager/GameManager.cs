using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance_ = default;

    public bool isGameOver = false;

    private const string SCORE_TEXT_OBJ = "PlayerScore";
    private const string HISCORE_TEXT_OBJ = "HiScore";
    private const string BONUSSCORE_TEXT_OBJ = "BonusScore";
    private const string STAGE_TEXT_OBJ = "Stage";
    private const string LOADING_IMAGE_OBJ = "LoadingImage";
    private const string LOADING_TEXT_OBJ = "LoadingText";

    private GameObject scoreTObj = default;
    private GameObject hiScoreTObj = default;
    private GameObject bonusScoreTObj = default;
    private GameObject stageTObj = default;
    private GameObject loadingIObj = default;
    private GameObject loadingTObj = default;

    private int score = default;
    private int hiScore = default;
    private int bonusScore = default;
    private int stageNumber = default;
    private int lifeCount = default;

    private bool isFirstLoad = true;

    // scrollingController Components.
    List<ScrollingObjController> ScrollringManagers = default;

    public delegate void OnAddScoreHandle(int value);
    public OnAddScoreHandle onAddScoreHandle;

    void Awake()
    {
        if(instance_ == null)
        {
            GFunc.LogWarning("Manager Awake s Check");
            instance_ = this;
            isGameOver = false;
            isFirstLoad = true;
            score = 0;
            hiScore = 20000;
            bonusScore = 5000;
            stageNumber = 1;
            lifeCount = 3;
            
           
            GameObject uiObj = GFunc.GetRootObj("UiObjs");
            GameObject uispace = uiObj.FindChildObj("UiSpace");

            if(uispace == null || uispace == default)
            {
                GFunc.LogWarning("uispace : null");
            }
            bonusScoreTObj = uispace.FindChildObj(BONUSSCORE_TEXT_OBJ);
            if(bonusScoreTObj == null || bonusScoreTObj == default)
            {
                GFunc.LogWarning("bonusScoreTObj : null");
            }
            scoreTObj = uispace.FindChildObj(SCORE_TEXT_OBJ);
            if(scoreTObj == null || scoreTObj == default)
            {
                GFunc.LogWarning("scoreTObj : null");
            }

            hiScoreTObj = uispace.FindChildObj(HISCORE_TEXT_OBJ);

            stageTObj = uispace.FindChildObj(STAGE_TEXT_OBJ);

            loadingIObj = uiObj.FindChildObj(LOADING_IMAGE_OBJ);
            if(loadingIObj == null || loadingIObj == default)
            {
                GFunc.LogWarning("loadingIObj : null");
            }

            loadingTObj = uiObj.FindChildObj(LOADING_TEXT_OBJ);

            
            hiScoreTObj.SetTmpText($"HI - {hiScore.ToString("D6")}");
            loadingTObj.SetTmpText($"STAGE {stageNumber.ToString("D2")}");
            bonusScoreTObj.SetTmpText($"BONUS - {bonusScore}");


            GameObject gameObjs = GFunc.GetRootObj("GameObjs");
            ScrollringManagers = new List<ScrollingObjController>();
            ScrollingBgController bgScroller = gameObjs.FindChildObj("BgObjs").GetComponentMust<ScrollingBgController>();
            ScrollingMeterSignController metersignScroller = gameObjs.FindChildObj("MeterSigns").GetComponentMust<ScrollingMeterSignController>();
            ScrollingRoFController rofScroller = gameObjs.FindChildObj("Obstacles_RoF").GetComponentMust<ScrollingRoFController>();
            ScrollingJarController jarScroller = gameObjs.FindChildObj("Obstacles_Jar").GetComponentMust<ScrollingJarController>();
            ScrollringManagers.Add(bgScroller);
            ScrollringManagers.Add(metersignScroller);
            ScrollringManagers.Add(rofScroller);
            ScrollringManagers.Add(jarScroller);

            onAddScoreHandle = new OnAddScoreHandle(AddScore);
            GFunc.LogWarning("Manager Awake e Check");
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        GFunc.LogWarning("Manager Start Check");
        StartCoroutine(StageTimer());
        EnterLoading();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void StageLoadPrc()
    {
        if(lifeCount <= 0)
            return;
        StartCoroutine(EndLoading());
        if(isFirstLoad)
            isFirstLoad = false;
        else
            StartCoroutine(DecreaseLife());

        foreach(var scroller in ScrollringManagers)
        {
            scroller.InitObjs();
        }
        stageTObj.SetTmpText($"STAGE - {stageNumber.ToString("D2")}");
    }
    public void EnterLoading()
    {
        loadingIObj.SetActive(true);
        GFunc.Pause();
        GFunc.LogWarning("Pause");
        GameOverPrc();
        StageLoadPrc();
    }
    public void EnterFin()
    {
        StartCoroutine(GameFinish());
    }
    void GameOverPrc()
    {
        if(lifeCount >= 1)
            return;
        StartCoroutine(GameOver());
    }
    IEnumerator DecreaseLife()
    {
        loadingTObj.SetTmpText($"STAGE {stageNumber.ToString("D2")}");
        yield return new WaitForSecondsRealtime(1f);
        loadingTObj.SetTmpText($"LIFE - {lifeCount}");
        yield return new WaitForSecondsRealtime(1f);
        lifeCount--;
        loadingTObj.SetTmpText($"LIFE - {lifeCount}");
    }
    IEnumerator GameOver()
    {
        loadingTObj.SetTmpText($"LIFE - {lifeCount}");
        yield return new WaitForSecondsRealtime(1.5f);
        loadingTObj.SetTmpText("GAME OVER");
        yield return new WaitForSecondsRealtime(3f);
        GFunc.LoadScene(GData.SCENE_NAME_TITLE);
    }
    IEnumerator GameFinish()
    {
        loadingTObj.SetTmpText("Thanks for Playing!");
        yield return new WaitForSecondsRealtime(3f);
        loadingIObj.SetActive(true);
        yield return new WaitForSecondsRealtime(3f);
        GFunc.LoadScene(GData.SCENE_NAME_TITLE);
    }
    IEnumerator EndLoading()
    {
        GFunc.LogWarning("End Loading to after 3Sec");
        yield return new WaitForSecondsRealtime(3f);
        // 로딩 검은화면 가림 해제 및 퍼즈 해제
        GFunc.LogWarning("End Loading");
        loadingIObj.SetActive(false);
        SoundManager.instance_.PlayMainBgm();
        GFunc.ReleasePause();
    }

    IEnumerator StageTimer()
    {
        while(true)
        {
            yield return new WaitForSeconds(0.25f);
            bonusScore -= 10;
            string strBS = bonusScore.ToString("D4");
            bonusScoreTObj.SetTmpText($"BONUS - {strBS}");
        }
    }
    void AddScore(int value)
    {
        score += value;
        GFunc.Log($"AddScore : {value}");
        scoreTObj.SetTmpText($"1P - {score.ToString("D6")}");
    }
}
