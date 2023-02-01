using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectScrolling : MonoBehaviour
{
    private float speed = default;
    public float defSpeed = default;

    private PlayerController PC = default;

    private float Acceleration = default;
    // Start is called before the first frame update
    void Start()
    {
        speed = defSpeed;
        Acceleration = 0.0f;
        // pc 찾기
        PC = GFunc.GetRootObj("GameObjs").FindChildObj("PlayerCharacter").GetComponentMust<PlayerController>();

    }

    // Update is called once per frame
    void Update()
    {
        SetAcceleration();
        Vector3 defMoveSpeed = Vector2.left * speed * Time.deltaTime;
        Vector3 addMoveSpeed = Vector2.left * Time.deltaTime * Acceleration;
        transform.Translate(defMoveSpeed + addMoveSpeed);
    }

    private void SetAcceleration()
    {
        GFunc.Assert(PC);
        if(PC.AxisX >= 1f)
        {
            Acceleration = 2f;
        }
        else if(PC.AxisX <= -0.5f)
        {
            Acceleration = -2f;
        }
        else
        {
            Acceleration = 0f;
        }
    }
}
