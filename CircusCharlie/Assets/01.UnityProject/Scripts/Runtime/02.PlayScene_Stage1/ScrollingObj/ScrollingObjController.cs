using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollingObjController : MonoBehaviour
{
    public string prefabName = default;
    public float scrollingSpeed = 100.0f;
    public int scrollingObjCount = default;
    protected GameObject objPrefab = default;
    protected Vector2 objPrefabSize = default;

    protected List<GameObject> scrollingPool = default;

    protected float lastScrObjInitPosX = default;
    protected float prefabYPos = default;
    // Start is called before the first frame update
    public virtual void Start()
    {
        objPrefab = gameObject.FindChildObj(prefabName);
        objPrefabSize = objPrefab.GetRectSizeDelta();

        prefabYPos = objPrefab.transform.localPosition.y;

        scrollingPool = new List<GameObject>();

        GFunc.Assert(objPrefab != null || objPrefab != default);

        // { 스크롤링 풀을 생성해서 주어진 수 만큼 초기화 하는 로직
        GameObject tempObj = default;
        if(scrollingPool.Count <= 0)
        {
            for(int i = 0; i < scrollingObjCount; i++)
            {
                tempObj = Instantiate(objPrefab, objPrefab.transform.position, objPrefab.transform.rotation, transform);
                scrollingPool.Add(tempObj);
                tempObj = default;
            }
            
        }   // if : scrolling pool 초기화
        
        objPrefab.SetActive(false);
        // } 스크롤링 풀을 생성해서 주어진 수 만큼 초기화 하는 로직
        InitObjsPosition();
        
    }

    // Update is called once per frame
    public virtual void Update()
    {
        if(scrollingPool == default || scrollingPool.Count <= 0 )
        {
            return;
        } 

        // 스크롤링할 오브젝트가 존재하는 경우
        for(int i = 0 ; i < scrollingObjCount; i++)
        {
            scrollingPool[i].AddLocalPos(scrollingSpeed * Time.deltaTime * (-1), 0f, 0f);
        }   // 배경 왼쪽 이동

        RepositionFirstObj();
        
    }   // Update()

    protected virtual void InitObjsPosition()
    {
        
    }

    protected virtual void RepositionFirstObj()
    {
        

    }
}
