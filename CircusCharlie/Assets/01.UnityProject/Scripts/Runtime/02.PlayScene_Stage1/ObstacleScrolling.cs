using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleScrolling : MonoBehaviour
{
    private float speed = default;
    public float defSpeed = default;
    private float pcSpeed = default;

    private float sizeX = default;

    private PlayerController PC = default;

    private RectTransform rectTransform = default;

    private ScrollingObjController scrollController = default;

    private float Acceleration = default;
    // Start is called before the first frame update
    void Start()
    {
        speed = defSpeed;
        Acceleration = 0.0f;
        // pc 찾기
        PC = GFunc.GetRootObj("GameObjs").FindChildObj("PlayerCharacter").GetComponentMust<PlayerController>();
        pcSpeed = PC.speed;
        rectTransform = gameObject.GetComponentMust<RectTransform>();
        scrollController = transform.parent.gameObject.GetComponentMust<ScrollingObjController>();
        sizeX = gameObject.GetRectSizeDelta().x;
        GFunc.Log($"Size Delta X : {sizeX}");
    }

    // Update is called once per frame
    void Update()
    {
        SetAcceleration();
        Vector3 defMoveSpeed = Vector2.left * speed * Time.deltaTime;
        Vector3 addMoveSpeed = Vector2.left * Time.deltaTime * Acceleration;
        Vector3 finalMoveSpeed = defMoveSpeed + addMoveSpeed;
        //transform.Translate(defMoveSpeed + addMoveSpeed);
        // gameObject.AddLocalPos(finalMoveSpeed.x, 0f, 0f);
        rectTransform.Translate(defMoveSpeed + addMoveSpeed);
        DisableCheck();
    }

    private void SetAcceleration()
    {
        GFunc.Assert(PC);
        if(PC.AxisX >= 1f)
        {
            Acceleration = pcSpeed;
        }
        else if(PC.AxisX <= -0.5f)
        {
            Acceleration = -pcSpeed;
        }
        else
        {
            Acceleration = 0f;
        }
    }
    private void DisableCheck()
    {
        if(transform.localPosition.x <= (-640 - gameObject.GetRectSizeDelta().x))
        {
            gameObject.SetActive(false);
            ScrollingRoFController rofCtrl = scrollController as ScrollingRoFController;
            if(rofCtrl != null && rofCtrl != default)
            {
                rofCtrl.objectOoCHandle();
                return;
            }
            ScrollingJarController jarCtrl = scrollController as ScrollingJarController;
            if(jarCtrl != null && jarCtrl != default)
            {
                jarCtrl.objectOoCHandle();
                return;
            }
            
        }
    }
}
