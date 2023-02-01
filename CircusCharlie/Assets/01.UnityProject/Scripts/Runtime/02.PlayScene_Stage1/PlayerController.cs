using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private const float PLAYER_STEP_ON_Y_POS_MIN = 0.7f;
    private bool isDead = default;
    private bool isGrounded = default;
    public float jumpForce = default;

    private float MoveAxisX = default;


#region  Player's Components
    private Rigidbody2D playerRigid = default;
    private Animator CharlieAnim = default;
    private Animator LionAnim = default;
#endregion // player components

    // Start is called before the first frame update
    void Start()
    {
        playerRigid = gameObject.GetComponentMust<Rigidbody2D>();
        CharlieAnim = gameObject.FindChildObj("Charlie").GetComponentMust<Animator>();
        LionAnim = gameObject.FindChildObj("Lion").GetComponentMust<Animator>();
        isGrounded = true;
        isDead = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(isDead)
        {
            return;
        }
        Move();
        Jump();
        UpdateAnimationProperty();
    }

    private void Move()
    {
        MoveForward();
        MoveBackward();
    }

    // 쉬운 버전, 좌 우 이동 구현
    private void MoveForward() // * MoveRight
    {
        // 임시로 키 입력 받아서 실행
        if(Input.GetKeyDown(KeyCode.RightArrow))
        {
            // GFunc.Log("Forward Move");
            MoveAxisX = 1f;
        }
        if(Input.GetKeyUp(KeyCode.RightArrow))
        {
            MoveAxisX = 0f;
        }
    }

    private void MoveBackward() // * MoveLeft
    {
        // 임시로 키 입력 받아서 실행
        if(Input.GetKeyDown(KeyCode.LeftArrow))
        {
            // GFunc.Log("Forward Move");
            MoveAxisX = -0.5f;
        }
        if(Input.GetKeyUp(KeyCode.LeftArrow))
        {
            MoveAxisX = 0f;
        }
    }

    private void Jump()
    {
        if(!isGrounded)
        {
            return;
        }
        // 임시로 키 입력 받아서 실행
        if(Input.GetMouseButtonDown(0) && isGrounded)
        {
            playerRigid.AddForce(new Vector2(0f, jumpForce));
        }
    }
    private void UpdateAnimationProperty()
    {
        
        CharlieAnim.SetFloat("AxisX", MoveAxisX);
        CharlieAnim.SetBool("Grounded", isGrounded);

        LionAnim.SetBool("Grounded", isGrounded);
        LionAnim.SetFloat("AxisX", MoveAxisX);
    }

    // 바닥에 닿았는지 체크하는 함수
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.contacts[0].normal.y > PLAYER_STEP_ON_Y_POS_MIN)
        {
            GFunc.Log("Grounded true;");
            isGrounded = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        GFunc.Log("Grounded false");
        isGrounded = false;
    }
}
