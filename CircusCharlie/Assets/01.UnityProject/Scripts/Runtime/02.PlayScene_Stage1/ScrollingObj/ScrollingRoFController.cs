using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollingRoFController : ScrollingObjController
{
    public Transform spawnTransform = default;
    public string prefab2Name = default;
    public string prefab3Name = default;

    private GameObject objPrefab2 = default;
    private GameObject objPrefab3 = default;
    private Vector2 objPrefab2Size = default;
    private Vector2 objPrefab3Size = default;

    // 현재 맵에 나와있는 불고리 수
    private int cntCurrentInCamRoF = default;
    // 맵에 나올 불고리 수 최댓값
    public int cntMaxInCamRof = default;
    // 불고리 스폰 시간
    public float spawnRate = default;
    // 불고리 스폰 시간을 잴 변수
    private float spawnTimer = default;

    //Delegate
    public delegate void OnObjectOoC();
    public OnObjectOoC objectOoCHandle;
    //

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();

        objPrefab2 = gameObject.FindChildObj(prefab2Name);
        objPrefab2Size = objPrefab2.GetRectSizeDelta();

        objPrefab3 = gameObject.FindChildObj(prefab3Name);
        objPrefab3Size = objPrefab3.GetRectSizeDelta();

        GFunc.Assert(objPrefab2 != null || objPrefab2 != default);
        GFunc.Assert(objPrefab3 != null || objPrefab3 != default);

        // { 스크롤링 풀을 생성해서 주어진 수 만큼 초기화 하는 로직
        GameObject tempObj = default;
        if(scrollingPool.Count <= 2)
        {
            for(int i = 0; i < scrollingObjCount; i++)
            {
                tempObj = Instantiate(objPrefab2, objPrefab2.transform.position, objPrefab2.transform.rotation, transform);
                scrollingPool.Add(tempObj);
                tempObj = default;

                tempObj = Instantiate(objPrefab3, objPrefab3.transform.position, objPrefab3.transform.rotation, transform);
                scrollingPool.Add(tempObj);
                tempObj = default;
            }
        }   // if : scrolling pool 초기화
        objPrefab2.SetActive(false);
        objPrefab3.SetActive(false);

        InitRoF();

        objectOoCHandle = () => cntCurrentInCamRoF--;

        cntCurrentInCamRoF = 0;

        StartCoroutine(SpawnRingofFire());
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

    private void InitRoF()
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

    IEnumerator SpawnRingofFire()
    {
        yield return new WaitForSeconds(0.5f);

        if(spawnTimer >= spawnRate && cntCurrentInCamRoF < cntMaxInCamRof)
        {
            RespawnRoF();
            spawnTimer = 0f;
        }
        StartCoroutine(SpawnRingofFire());
    }
    private void RespawnRoF()
    {
        List<GameObject> spawnList = new List<GameObject>();
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
        cntCurrentInCamRoF++;

    }

}
