using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollingJarController : ScrollingObjController
{
    public Transform spawnTransform = default;

    // 현재 맵에 나와있는 항아리 수
    private int cntCurrentInCamJar = default;
    // 맵에 나올 불고리 수 최댓값
    public int cntMaxInCamJar = default;
    // 불고리 스폰 시간
    public float spawnRate = default;
    // 불고리 스폰 시간을 잴 변수
    private float spawnTimer = default;
    private bool isSpawnable = default;

    private GameObject LastJar = default;

    //Delegate
    public delegate void OnObjectOoC();
    public OnObjectOoC objectOoCHandle;
    //

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();

        InitJar();

        objectOoCHandle = () => 
        {
            cntCurrentInCamJar--;
        };

        cntCurrentInCamJar = 0;
        isSpawnable = true;

        // 플레이어 컨트롤러 델리게이트에 스폰 스위치 등록
        PlayerController pc =
        GFunc.GetRootObj("GameObjs").FindChildObj("PlayerCharacter").GetComponentMust<PlayerController>();

        pc.onOver80MHandle += () => isSpawnable = false;
        pc.onUnder80MHandle += () => isSpawnable = true;

        StartCoroutine(SpawnJar());
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();

        spawnTimer += Time.deltaTime;

        
    }
    protected override void InitObjsPosition()
    {
       
    }

    private void InitJar()
    {
        foreach(var obj in scrollingPool)
        {
            obj.SetActive(false);
        }
    }
    protected override void RepositionFirstObj()
    {
        base.RepositionFirstObj();
    }

    IEnumerator SpawnJar()
    {
        yield return new WaitForSeconds(1f);

        if(isSpawnable == true)
        {
            GFunc.LogWarning("Jar RespawnCheck");
            if(LastJar == null || LastJar == default)
            {
                RespawnJar();
            }
            else
            {
                if(LastJar.transform.localPosition.x <= -640)
                {
                    RespawnJar();
                }
            }
        }
        StartCoroutine(SpawnJar());
    }
    private void RespawnJar()
    {
        List<GameObject> spawnList = new List<GameObject>();
        GFunc.LogWarning("Jar RespawnCheck");
        foreach(var obj_ in scrollingPool)
        {
            if(obj_.activeSelf)
            {
                continue;
            }
            spawnList.Add(obj_);
        }

        int idx = Random.Range(0, spawnList.Count);
        GameObject spawnObj = spawnList[idx];
        
        spawnObj.transform.position = spawnTransform.position;
        spawnObj.SetActive(true);
        LastJar = spawnObj;
        cntCurrentInCamJar++;

    }
}
