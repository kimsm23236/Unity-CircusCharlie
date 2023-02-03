using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectScrolling : MonoBehaviour
{
    private float speed = default;
    public float defSpeed = default;

    private float dist = default;
    public float Distance
    {
        get 
        {
            return dist;
        }
    }

    private float pcSpeed = default;

    private PlayerController PC = default;

    private float Acceleration = default;
    // Start is called before the first frame update
    void Start()
    {
        speed = defSpeed;
        Acceleration = 0.0f;
        // pc 찾기
        PC = GFunc.GetRootObj("GameObjs").FindChildObj("PlayerCharacter").GetComponentMust<PlayerController>();
        pcSpeed = PC.speed;
        dist = 0;
    }

    // Update is called once per frame
    void Update()
    {
        SetAcceleration();
        Vector3 defMoveSpeed = Vector2.left * speed * Time.deltaTime;
        Vector3 addMoveSpeed = Vector2.left * Acceleration * Time.deltaTime ;
        Vector3 finalMoveSpeed = defMoveSpeed + addMoveSpeed;
        // GFunc.Log($"FMS : {finalMoveSpeed.x}, {finalMoveSpeed.y}, {finalMoveSpeed.z}");
        if(PC.PlayerMoveType == PlayerController.EPlayerMoveType.BgMove)
        {
            transform.Translate(finalMoveSpeed);
        }
        float mag = default;
        if(finalMoveSpeed.x < 0)
        {
            mag = finalMoveSpeed.magnitude;
        }
        else
        {
            mag = -finalMoveSpeed.magnitude;
        }

        dist += mag;
        // PC.onCalDistHandle(dist);
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
}
