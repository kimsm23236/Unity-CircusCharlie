using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollingMeterSignController : ScrollingObjController
{
    // * bg width size
    public float distanceMeterSigns = default;

    private GameObject endpoint = default;

    // Start is called before the first frame update
    void Awake()
    {
        base.Awake();
        endpoint = gameObject.FindChildObj("EndPoint");
        // endpoint.transform.parent = transform.parent;
    }
    public override void Start()
    {
        base.Start();
        // 텍스트 작업 추가
        int meter = 100;
        foreach(var obj_ in scrollingPool)
        {
            obj_.FindChildObj("Text (TMP)").SetTmpText($"{meter}");
            meter -= 10;
        }
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
    }
    protected override void InitObjsPosition()
    {
        base.InitObjsPosition();

        PlayerController pc = GFunc.GetRootObj("GameObjs").FindChildObj("PlayerCharacter").GetComponentMust<PlayerController>();
        float dist = Mathf.Floor(pc.Distance / 10);
        pc.Distance = dist * 10;

        int scrollCntIndex = scrollingObjCount - 1;
        // float horizonPos = objPrefab.transform.localPosition.x;
        float horizonPos = objPrefab.transform.localPosition.x - (distanceMeterSigns * dist);
        float verticalPos = objPrefab.transform.localPosition.y;

        for(int i = 0; i < scrollingObjCount; i++)
        {
            scrollingPool[i].SetLocalPos(horizonPos, verticalPos, 0f);
            horizonPos = horizonPos + distanceMeterSigns;
        }   // 생성한 오브젝트를 가로로 왼쪽부터 차례대로 정렬하는 루프
        
        SetEndPointTransform();
        // 가장 마지막 오브젝트의 초기화 위치를 캐싱
        lastScrObjInitPosX = horizonPos;

    }
    public void InitObjsPosition(float dist)
    {
        base.InitObjsPosition();

        // 플레이어 위치 변수에 따라 미터사인 위치 결정
        dist = Mathf.Floor(dist / 10);
        int scrollCntIndex = scrollingObjCount - 1;
        float horizonPos = objPrefab.transform.localPosition.x - (distanceMeterSigns * dist);
        float verticalPos = objPrefab.transform.localPosition.y;

        for(int i = 0; i < scrollingObjCount; i++)
        {
            scrollingPool[i].SetLocalPos(horizonPos, verticalPos, 0f);
            horizonPos = horizonPos + distanceMeterSigns;
        }   // 생성한 오브젝트를 가로로 왼쪽부터 차례대로 정렬하는 루프

        SetEndPointTransform();

        // 가장 마지막 오브젝트의 초기화 위치를 캐싱
        lastScrObjInitPosX = horizonPos;
    }
    protected override void RepositionFirstObj()
    {
        base.RepositionFirstObj();

        // float lastScrollObjXPos = scrollingPool[scrollingObjCount - 1].transform.localPosition.x;
        // if(lastScrollObjXPos <= objPrefabSize.x * 0.5f)
        // {
        //     scrollingPool[0].SetLocalPos((objPrefabSize.x + objPrefabSize.x * 0.5f) -1f, 0f, 0f);
        //     scrollingPool.Add(scrollingPool[0]);
        //     scrollingPool.RemoveAt(0);
        // }   // if : 스크롤링 오브젝트의 마지막 오브젝트가 화면 상의 절반정도 Draw 되는 때
    }
    public void SetEndPointTransform()
    {
        RectTransform lastMeterSignTransform = scrollingPool[scrollingObjCount - 1].GetRect();
        endpoint.GetRect().position = lastMeterSignTransform.position;
        endpoint.GetRect().rotation = lastMeterSignTransform.rotation;
        endpoint.AddLocalPos(0f, 100f, 0f);
    }
    public override void InitObjs()
    {
        base.InitObjs();
        InitObjsPosition();
    }
}
