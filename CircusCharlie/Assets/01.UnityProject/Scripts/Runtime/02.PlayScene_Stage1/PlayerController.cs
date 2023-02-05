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

    // { controller
    private float leftMoveValue = default;
    private float rightMoveValue = default;
    // controller delegate 
    public delegate void OnButtonPressHandle();
    public OnButtonPressHandle onButtonDownLeftBtn = default;
    public OnButtonPressHandle onButtonUpLeftBtn = default;
    public OnButtonPressHandle onButtonDownRightBtn = default;
    public OnButtonPressHandle onButtonUpRightBtn = default;
    public OnButtonPressHandle onButtonDownJumpBtn = default;

    // } controller
    public float AxisX
    {
        get
        {
            if(playerMoveType == EPlayerMoveType.BgMove)
                return moveAxisX;
            else
                return 0f;
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
    public float Distance
    {
        get 
        {
            return currentDistance;
        }
        set
        {
            currentDistance = value;
        }
    }


    


#region  Player's Components
    private Rigidbody2D playerRigid = default;
    private Animator CharlieAnim = default;
    private Animator LionAnim = default;
    private AudioSource audioSource = default;
    public AudioClip jumpSound = default;
    public AudioClip deathSound = default;
#endregion // player components

    
    public delegate void OnCalDistanceHandle(float dist);
    public OnCalDistanceHandle onCalDistHandle;
    public delegate void OnChangePlayerMoveType();
    public OnChangePlayerMoveType onChangedPlayerMoveHandle;

    // Trigger delegate
    public delegate void OnDistancePrc();
    public OnDistancePrc onOver80MHandle;
    public OnDistancePrc onUnder80MHandle;
    private bool isInUnder80M;
    public OnDistancePrc onOver94MHandle;
    public OnDistancePrc onUnder94MHandle;
    private bool isInUnder94M;
    //
    void Awake()
    {
        playerRigid = gameObject.GetComponentMust<Rigidbody2D>();
        CharlieAnim = gameObject.FindChildObj("Charlie").GetComponentMust<Animator>();
        LionAnim = gameObject.FindChildObj("Lion").GetComponentMust<Animator>();
        audioSource = gameObject.GetComponentMust<AudioSource>();
    }
    // Start is called before the first frame update
    void Start()
    {
        GFunc.LogWarning("Before Init Player");
        InitPlayer();
        GFunc.LogWarning("After Init Player");
        // onCalDistHandle = new OnCalDistanceHandle(CalculateDistance);

        // trigger
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
        //
        // controller
        leftMoveValue = 0f;
        rightMoveValue = 0f;
        onButtonDownJumpBtn = new OnButtonPressHandle(Jump);
        onButtonDownLeftBtn = () => leftMoveValue = 1f;
        onButtonUpLeftBtn = () => leftMoveValue = 0f;
        onButtonDownRightBtn = () => rightMoveValue = 1f;
        onButtonUpRightBtn = () => rightMoveValue = 0f;
        //

    }
    void InitPlayer()
    {
        isGrounded = true;
        isDead = false;
        playerMoveType = EPlayerMoveType.BgMove;
        currentDistance = 0f;
        hasPoint = 0;
        audioSource.clip = jumpSound;
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
        UpdateAnimationProperty();

    }

    private void Move()
    {
        Vector3 moveVector = Vector3.zero;
        switch(playerMoveType)
        {
            case EPlayerMoveType.BgMove:
                moveAxisX = rightMoveValue - leftMoveValue;
            break;
            case EPlayerMoveType.CharacterMove:
                Vector3 leftMoveVector = Vector2.left * speed * Time.deltaTime * leftMoveValue;
                Vector3 rightMoveVector = Vector2.right * speed * Time.deltaTime * rightMoveValue;
                moveVector = leftMoveVector + rightMoveVector;
                transform.Translate(moveVector);
                moveAxisX = rightMoveValue - leftMoveValue;
            break;
            case EPlayerMoveType.Finished:
                moveVector = epCenter.transform.position - transform.position;
                transform.position = Vector3.Lerp(transform.position, epCenter.transform.position, 5.0f);
            break;
            default:
            break;
        }
        // MoveForward();
        // MoveBackward();
        CalculateDistance();
        // GFunc.Log($"distance : {currentDistance}");
    }


    // 쉬운 버전, 좌 우 이동 구현 * Lagacy Code
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
        playerRigid.AddForce(new Vector2(0f, jumpForce));
        audioSource.PlayOneShot(jumpSound);
    }

    private void Die()
    {
        CharlieAnim.SetTrigger("Dead");
        LionAnim.SetTrigger("Dead");
        StartCoroutine(DeadPrc());
        StartCoroutine(StartDead());
        SoundManager.instance_.StopMainBgm();
        audioSource.PlayOneShot(deathSound);
    }
    IEnumerator StartDead()
    {
        yield return new WaitForSeconds(0.1f);
        Time.timeScale = 0;
    }
    IEnumerator DeadPrc()
    {
        yield return new WaitForSecondsRealtime(3f);
        GameManager gm = GFunc.GetRootObj("ManagerObjs").FindChildObj("GameManager").GetComponentMust<GameManager>();
        gm.EnterLoading();
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
            CharlieAnim.SetTrigger("Finish");
            GameManager.instance_.EnterFin();
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
