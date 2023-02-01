using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollingBgController : ScrollingObjController
{
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
    }
    protected override void InitObjsPosition()
    {
        base.InitObjsPosition();

        int scrollCntIndex = scrollingObjCount - 1;
        float horizonPos = objPrefabSize.x * (scrollingObjCount - 1) * (-1) * 0.5f;

        for(int i = 0; i < scrollingObjCount; i++)
        {
            scrollingPool[i].SetLocalPos(horizonPos, 0f, 0f);
            horizonPos = horizonPos + objPrefabSize.x;
        }   // 생성한 오브젝트를 가로로 왼쪽부터 차례대로 정렬하는 루프

        // 가장 마지막 오브젝트의 초기화 위치를 캐싱
        lastScrObjInitPosX = horizonPos;

    }
    protected override void RepositionFirstObj()
    {
        base.RepositionFirstObj();

        float lastScrollObjXPos = scrollingPool[scrollingObjCount - 1].transform.localPosition.x;
        if(lastScrollObjXPos <= objPrefabSize.x * 0.5f)
        {
            scrollingPool[0].SetLocalPos((objPrefabSize.x + objPrefabSize.x * 0.5f) -1f, 0f, 0f);
            scrollingPool.Add(scrollingPool[0]);
            scrollingPool.RemoveAt(0);
        }   // if : 스크롤링 오브젝트의 마지막 오브젝트가 화면 상의 절반정도 Draw 되는 때
    }
}
