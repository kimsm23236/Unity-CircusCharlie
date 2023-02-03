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
    private const string STAGE_TEXT_OBJ = "";

    private GameObject scoreTObj = default;
    private GameObject hiScoreTObj = default;
    private GameObject bonusScoreTObj = default;
    private GameObject stageTObj = default;

    private int score = default;
    private int hiScore = default;
    private int bonusScore = default;
    private int stageNumber = default;

    public delegate void OnAddScoreHandle(int value);
    public OnAddScoreHandle onAddScoreHandle;

    void Awake()
    {
        if(instance_ == null)
        {
            instance_ = this;
            isGameOver = false;
            score = 0;
            hiScore = 20000;
            bonusScore = 5000;
            stageNumber = 0;
            GameObject uiObj = GFunc.GetRootObj("UiObjs");
            bonusScoreTObj = uiObj.FindChildObj(BONUSSCORE_TEXT_OBJ);
            scoreTObj = uiObj.FindChildObj(SCORE_TEXT_OBJ);

            hiScoreTObj = uiObj.FindChildObj(HISCORE_TEXT_OBJ);
            hiScoreTObj.SetTmpText($"HI - {hiScore.ToString("D6")}");

            onAddScoreHandle = new OnAddScoreHandle(AddScore);
            
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(StageTimer());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator StageTimer()
    {
        while(true)
        {
            bonusScore -= 10;
            string strBS = bonusScore.ToString("D4");
            bonusScoreTObj.SetTmpText($"BONUS - {strBS}");
            yield return new WaitForSeconds(0.25f);
        }
    }
    void AddScore(int value)
    {
        score += value;
        GFunc.Log($"AddScore : {value}");
        scoreTObj.SetTmpText($"1P - {score.ToString("D6")}");
    }
}
