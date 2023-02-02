using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private const float PLAYER_STEP_ON_Y_POS_MIN = 0.7f;
    private bool isDead = default;
    private bool isGrounded = default;
    public float jumpForce = default;

    public float speed = default;

    private float moveAxisX = default;
    public float AxisX
    {
        get
        {
            return moveAxisX;
        }
    }
    public enum EPlayerMoveType
    {
        NONE = -1,
        BgMove, CharacterMove
    }

    private EPlayerMoveType playerMoveType = default;

    public EPlayerMoveType PlayerMoveType
    {
        get
        {
            return playerMoveType;
        }
        set
        {
            playerMoveType = value;
            moveAxisX = 0f;
            onChangedPlayerMoveHandle();
        }
    }

    private float currentDistance = default;


    


#region  Player's Components
    private Rigidbody2D playerRigid = default;
    private Animator CharlieAnim = default;
    private Animator LionAnim = default;
#endregion // player components

    // delegate
    public delegate void OnCalDistanceHandle(float dist);
    public OnCalDistanceHandle onCalDistHandle;
    public delegate void OnChangePlayerMoveType();
    public OnChangePlayerMoveType onChangedPlayerMoveHandle;
    //

    // Start is called before the first frame update
    void Start()
    {
        playerRigid = gameObject.GetComponentMust<Rigidbody2D>();
        CharlieAnim = gameObject.FindChildObj("Charlie").GetComponentMust<Animator>();
        LionAnim = gameObject.FindChildObj("Lion").GetComponentMust<Animator>();
        isGrounded = true;
        isDead = false;
        playerMoveType = EPlayerMoveType.BgMove;
        currentDistance = 0f;
        onCalDistHandle = new OnCalDistanceHandle(CalculateDistance);
    }

    // Update is called once per frame
    void Update()
    {
        if(isDead)
        {
            return;
        }
        Move();
        ChangeMoveType();
        Jump();
        UpdateAnimationProperty();
        GFunc.LogWarning($"Cal Distance : {currentDistance}");

    }

    private void Move()
    {
        MoveForward();
        MoveBackward();
    }


    // 쉬운 버전, 좌 우 이동 구현
    private void MoveForward() // * MoveRight
    {
        if(playerMoveType == EPlayerMoveType.BgMove)
        {
            // 임시로 키 입력 받아서 실행
            if(Input.GetKeyDown(KeyCode.RightArrow))
            {
                moveAxisX = 1f;
            }
            if(Input.GetKeyUp(KeyCode.RightArrow))
            {
                moveAxisX = 0f;
            }
        }
        else
        {
            // 임시로 키 입력 받아서 실행
            if(Input.GetKey(KeyCode.RightArrow))
            {
                Vector3 moveVector = Vector2.right * speed * Time.deltaTime;
                transform.Translate(moveVector);
            }
            if(Input.GetKeyUp(KeyCode.RightArrow))
            {
                moveAxisX = 0f;
            }
        }
        
    }

    private void MoveBackward() // * MoveLeft
    {
        if(playerMoveType == EPlayerMoveType.BgMove)
        {
            // 임시로 키 입력 받아서 실행
            if(Input.GetKeyDown(KeyCode.LeftArrow))
            {
                moveAxisX = -1f;
            }
            if(Input.GetKeyUp(KeyCode.LeftArrow))
            {
                moveAxisX = 0f;
            }
        }
        else
        {
            // 임시로 키 입력 받아서 실행
            if(Input.GetKey(KeyCode.LeftArrow))
            {
                Vector3 moveVector = Vector2.left * speed * Time.deltaTime;
                transform.Translate(moveVector);
            }
            if(Input.GetKeyUp(KeyCode.LeftArrow))
            {
                moveAxisX = 0f;
            }
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
        
        CharlieAnim.SetFloat("AxisX", moveAxisX);
        CharlieAnim.SetBool("Grounded", isGrounded);

        LionAnim.SetBool("Grounded", isGrounded);
        LionAnim.SetFloat("AxisX", moveAxisX);
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

    private void CalculateDistance(float dist)
    {
        if(playerMoveType == EPlayerMoveType.CharacterMove)
            return;
        currentDistance = dist * 0.57f;
        if(currentDistance >= 92)
        {
            playerMoveType = EPlayerMoveType.CharacterMove;
        }
        else
        {
            playerMoveType = EPlayerMoveType.BgMove;
        }
    }
    private void ChangeMoveType()
    {
        if(Input.GetKeyDown(KeyCode.A))
        {
            GFunc.Log("Change Move Type");
            if(playerMoveType == EPlayerMoveType.BgMove)
            {
                playerMoveType = EPlayerMoveType.CharacterMove;
            }
            else
            {
                playerMoveType = EPlayerMoveType.BgMove;
            }
        }
    }
}
