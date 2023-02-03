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

    private int hasPoint = default;

    private GameObject epCenter = default;
    public float AxisX
    {
        get
        {
            if(playerMoveType == EPlayerMoveType.CharacterMove)
                return 0;
            else
                return moveAxisX;
        }
    }
    public enum EPlayerMoveType
    {
        NONE = -1,
        BgMove, CharacterMove,Finished
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
    public delegate void OnDistancePrc();
    public OnDistancePrc onOver80MHandle;
    public OnDistancePrc onUnder80MHandle;
    private bool isInUnder80M;
    public OnDistancePrc onOver94MHandle;
    public OnDistancePrc onUnder94MHandle;
    private bool isInUnder94M;
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
        hasPoint = 0;
        // onCalDistHandle = new OnCalDistanceHandle(CalculateDistance);
        onOver94MHandle += () => 
        {
            playerMoveType = EPlayerMoveType.CharacterMove;
            GFunc.LogWarning($"playerMoveType : {playerMoveType}");
        };
        onUnder94MHandle += () => 
        {
            playerMoveType = EPlayerMoveType.BgMove;
            GFunc.LogWarning($"playerMoveType : {playerMoveType}");
        };
        isInUnder80M = true;
        isInUnder94M = true;
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

    }

    private void Move()
    {
        MoveForward();
        // MoveBackward();
        CalculateDistance();
        // GFunc.Log($"distance : {currentDistance}");
    }


    // 쉬운 버전, 좌 우 이동 구현
    private void MoveForward() // * MoveRight
    {
        switch(playerMoveType)
        {
            case EPlayerMoveType.BgMove:
            {
                // 임시로 키 입력 받아서 실행
                if(Input.GetKey(KeyCode.RightArrow) && currentDistance < 100)
                {
                    moveAxisX = 1f;
                }
                else if(Input.GetKey(KeyCode.LeftArrow) && currentDistance > 0)
                {
                    moveAxisX = -1f;
                }
                else // if(Input.GetKeyUp(KeyCode.RightArrow))
                {
                    moveAxisX = 0f;
                }
            }
            break;
            case EPlayerMoveType.CharacterMove:
            {
                if(Input.GetKey(KeyCode.RightArrow))
                {
                    Vector3 moveVector = Vector2.right * speed * Time.deltaTime;
                    transform.Translate(moveVector);
                    moveAxisX = 1f;
                }
                else if(Input.GetKey(KeyCode.LeftArrow))
                {
                    Vector3 moveVector = Vector2.left * speed * Time.deltaTime;
                    transform.Translate(moveVector);
                    moveAxisX = -1f;
                }
                else //if(Input.GetKeyUp(KeyCode.RightArrow))
                {
                    moveAxisX = 0f;
                }
            }
            break;
            case EPlayerMoveType.Finished:
            {
                Vector3 moveVector = epCenter.transform.position - transform.position;
                transform.position = Vector3.Lerp(transform.position, epCenter.transform.position, 5.0f);
            }
            break;
        }
    }

    private void MoveBackward() // * MoveLeft
    {
        if(playerMoveType == EPlayerMoveType.BgMove)
        {
            // 임시로 키 입력 받아서 실행
            if(Input.GetKey(KeyCode.LeftArrow))
            {
                moveAxisX = -1f;
            }
            else //if(Input.GetKeyUp(KeyCode.LeftArrow))
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

    private void Die()
    {
        CharlieAnim.SetTrigger("Dead");
        LionAnim.SetTrigger("Dead");
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
            GameManager.instance_.onAddScoreHandle(hasPoint);
            hasPoint = 0;
        }

        if(collision.gameObject.tag == "EndPlatform")
        {
            playerMoveType = EPlayerMoveType.Finished;
            epCenter = collision.gameObject.FindChildObj("Center");
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        GFunc.Log("Grounded false");
        isGrounded = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Deadzone")
        {
            Die();
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.tag == "Obstacle")
        {
            ObstacleInfo collObsComp = collision.gameObject.GetComponentMust<ObstacleInfo>();
            hasPoint += collObsComp.Score;
        }
    }

    private void CalculateDistance()
    {
        Vector3 moveSpeed = Vector3.zero;
        float moveDistance = default;
        moveSpeed = (Vector3.right * moveAxisX) * speed * Time.deltaTime;

        // if(playerMoveType == EPlayerMoveType.BgMove)
        // {
        //     moveSpeed = (Vector3.right * moveAxisX) * speed * Time.deltaTime;
        // }
        // else if(playerMoveType == EPlayerMoveType.CharacterMove)
        // {
        //     moveSpeed = (Vector3.right * moveAxisX) * speed * Time.deltaTime;
        // }
        moveDistance = moveSpeed.magnitude * 0.57f;
        currentDistance += moveDistance * moveAxisX;
        GFunc.LogWarning($"currentDist : {currentDistance}");

        if(currentDistance >= 80 && isInUnder80M)
        {
            onOver80MHandle();
            isInUnder80M = false;
        }
        else if(currentDistance < 80 && !isInUnder80M)
        {
            onUnder80MHandle();
            isInUnder80M = true;
        }

        if(currentDistance >= 94 && isInUnder94M)
        {
            onOver94MHandle();
            isInUnder94M = false;
        }
        else if(currentDistance < 94 && !isInUnder94M)
        {
            onUnder94MHandle();
            isInUnder94M = true;
        }
        // if(playerMoveType == EPlayerMoveType.CharacterMove)
        //     return;
        // currentDistance = dist * 0.57f;
        // if(currentDistance >= 92)
        // {
        //     playerMoveType = EPlayerMoveType.CharacterMove;
        // }
        // else
        // {
        //     playerMoveType = EPlayerMoveType.BgMove;
        // }
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
