using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using DG.Tweening;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private LayerMask platformLayerMask;
    public RaycastHit2D rayCastHit1;
    public BoxCollider2D groundChecker;

    #region GameEndVariable
    private bool isDead;
    private bool isGameEnd;

    [SerializeField] private float fdt;
    private float gameEndFdt = 2.5f;

    #endregion

    [Header("Reflection")]
    public bool isReflection;
    public Vector2 groundCheckDirection;
    public GameObject playerClone;
    public CameraController cameraController;

    private Rigidbody2D rigid;

    #region PlayerMoveVariable
    private float horInput;
    private Vector3 playerScale;
    [SerializeField] private float playerSpeed;
    private float playerXScale;
    private Animator animator;
    #endregion

    #region PlayerJumpVariable
    private bool isGround;
    [SerializeField] private bool isJump;

    [SerializeField] private float jumpHeight;
    [SerializeField] private int maxJump;
    private float jumpForce;
    [SerializeField] private int jumpCount;
    #endregion

    [Header("FrontScan")]
    public Vector2 detectionBoxSize = new Vector2(1f, 2f);
    public LayerMask detectionLayer;
    public Vector3 DetectDirection;

    #region MonoBehaviour Method

    void Start()
    {
        jumpCount = 0;
        rigid = GetComponent<Rigidbody2D>();
        playerXScale = transform.localScale.x;
        animator = GetComponent<Animator>();
        isReflection = false;
        isDead = false;
        groundCheckDirection = Vector2.down;
    }

    void Update()
    {        
        isGround = isGrounded();

        if (!isGround) fdt += Time.deltaTime;
        
        CheckGameOver();
      
        if (!isDead) PlayerAct();

    }

    private void FixedUpdate()
    {
        if (rigid != null)
        {
            if (rigid.velocity.y < -15)
            {
                rigid.velocity = new Vector3(rigid.velocity.x, -15,0f);
            }
        }
    }
    #endregion

    void PlayerAct()
    {   // check Player Act     
        PlayerJump();
        PlayerMove();
    }

    void PlayerJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && jumpCount < maxJump && fdt<=0.1f)
        {
            if (playerClone.activeSelf) playerClone.GetComponent<ClonePlayer>().PlayerJump(true);
            ColorManager.instance.AutoSwitchMainColoring();
            rigid.velocity = Vector2.zero;
            jumpForce = Mathf.Sqrt(jumpHeight * -2 * (Physics2D.gravity.y * rigid.gravityScale));
            Debug.Log(this.name+" "+ jumpForce);
            
            isJump = true;
            animator.Play("Boing");

            rigid.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
            //playerClone.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
        }

    }

    void PlayerMove()
    {
        horInput = Input.GetAxisRaw("Horizontal");
        Vector3 playerCloneScale = playerClone.transform.localScale;
        Vector3 detectDirection = new Vector3(0.5f, -1f,0f);
        Vector3 detectDirectionClone = new Vector3(0.5f, 1f, 0f);
        
        //좌우 구분
        playerScale = transform.localScale;
        if (horInput > 0)
        {
            playerScale.x = playerXScale;
            playerCloneScale.x = playerXScale;
        }
        else if(horInput < 0)
        {
            detectDirection = new Vector3(-detectDirection.x, detectDirection.y, detectDirection.z);
            detectDirectionClone = new Vector3(-detectDirectionClone.x, detectDirectionClone.y, detectDirectionClone.z);
            playerScale.x = -playerXScale;
            playerCloneScale.x = -playerXScale;
        }
        transform.localScale = playerScale;
        playerClone.transform.localScale = playerCloneScale;
        DetectDirection = detectDirection;


        //애니메이션
        if(horInput == 0)
        {
            animator.Play("Idle");
        }
        else
        {
            animator.Play("Boing");
        }

        //이동
        if (!playerClone.GetComponent<ClonePlayer>().isDetectObjectsAhead(detectDirectionClone) && !isDetectObjectsAhead())
        {
            transform.Translate(Vector2.right * Time.deltaTime * playerSpeed * horInput);
            if (playerClone.activeSelf) playerClone.GetComponent<ClonePlayer>().PlayerMove(horInput);
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    { // check to player Fall, use OnCollisionExit
        if (collision.gameObject.CompareTag("Ground"))
        {
            isJump = true;
            jumpCount++;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Item"))
        {
            if (!isGround)
            {
                fdt = 0f;
                isJump = false;
                jumpCount = 0;
            }
            other.gameObject.SetActive(false);

            StartCoroutine(ResetItem());

            IEnumerator ResetItem()
            { // item regenerate after 3 sec
                other.gameObject.SetActive(false);
                yield return new WaitForSeconds(3);
                other.gameObject.SetActive(true);
            }
        }

        if (other.gameObject.CompareTag("Spike"))
        {
            ColoredObject _co = other.gameObject.GetComponent<ColoredObject>();
            if (_co.isSpikeActive)
            {
                if (isDead == false) StartCoroutine(DeathCoroutine());
            }
        }

    }

    bool isGrounded()
    {
        float extraHeightText = 0.2f;
        // RaycastHit2D rayCastHit = Physics2D.Raycast(groundChecker.bounds.center, Vector2.down, groundChecker.bounds.extents.y + extraHeightText, platformLayerMask);
//        RaycastHit2D rayCastHit = Physics2D.BoxCast(groundChecker.bounds.center, groundChecker.bounds.size, 0f, Vector2.down, extraHeightText, platformLayerMask);
        rayCastHit1 = Physics2D.BoxCast(groundChecker.bounds.center, groundChecker.bounds.size, 0f, groundCheckDirection, extraHeightText, platformLayerMask);

        bool value;

        if (rayCastHit1.collider != null && !rayCastHit1.collider.isTrigger)
        {
            value = true;
            fdt = 0f;
            jumpCount = 0;
        }
        else
        {
            value = false;
            if (isJump)
            {
                jumpCount++;
                isJump = false;
            }
        }

        return value;
    }

    void CheckGameOver()
    {
        if(fdt > gameEndFdt)
        {
            GameOver();
        }
    }

    public IEnumerator DeathCoroutine()
    {
        isDead = true;
        DeathEffect();
        if (isReflection) playerClone.GetComponent<ClonePlayer>().StartCoroutine("DeathCoroutine");
        yield return new WaitForSeconds(1f);
        GameOver();
    }

    void DeathEffect()
    {
        float _minPower = 3f;
        float _maxPower = 9f;
        float _torquePower = 9f;

        for (int i = 0; i < transform.childCount; i++)
        {
            Rigidbody2D _rb2d = transform.GetChild(i).AddComponent<Rigidbody2D>();

            float _angle = Random.Range(0f, 360f);
            Vector2 _dir = new Vector2(Mathf.Cos(Mathf.Deg2Rad * _angle), Mathf.Sin(Mathf.Deg2Rad * _angle));
            float _power = Random.Range(_minPower, _maxPower);
            _rb2d.AddForce(_dir * _power, ForceMode2D.Impulse);

            int _rnd = Random.Range(0, 2);
            _rb2d.AddTorque((_rnd * 2f - 1f) * _torquePower,ForceMode2D.Impulse);

            if (transform.GetChild(i).GetComponent<Collider2D>() != null) transform.GetChild(i).GetComponent<Collider2D>().enabled = false;
        }
        transform.DetachChildren();
    }

    void GameOver()
    {
        if (isGameEnd == true) return;

        isGameEnd = true;
        UIManager.instance._isGameEnd = true;
    }

    public IEnumerator Reflection()
    {
        isReflection = true;
        cameraController.isReflection = true;
        //cameraController.GetComponent<Camera>().DOOrthoSize(10f, 0.5f);

        GetComponent<JellyShooter>().UpdateHeadColor();

        //클론 생성
        playerClone.transform.position = this.transform.position + new Vector3(0,0.4f,0);
        playerClone.transform.localScale = Vector3.zero; 
        playerClone.SetActive(true);
        //x,y 스케일 -1 곱하기
        Vector3 reflectScale = new Vector3(transform.localScale.x*1, transform.localScale.y * -1, transform.localScale.z);
        playerClone.transform.DOScale(reflectScale, 0.5f).SetEase(Ease.OutSine);

        yield return null;
    }

    public IEnumerator ReflectionOff()
    {
        // 머리 반납
        if (GetComponent<JellyShooter>().canRetrieve) GetComponent<JellyShooter>().RetriveJelly();

        // playerClone의 크기를 0으로 줄이는 트윈 애니메이션
        playerClone.transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.OutSine);
        // playerClone의 위치를 원래 오브젝트의 위치로 이동하는 트윈 애니메이션
        Tween tween = playerClone.transform.DOMove(transform.position, 0.5f);
        //cameraController.GetComponent<Camera>().DOOrthoSize(8f, 0.5f);
        tween.OnComplete(() =>
        {
            playerClone.SetActive(false);
            cameraController.isReflection = false;
            isReflection = false;
        });

        yield return null;
    }

    private bool isDetectObjectsAhead()
    {
        // 플레이어 바로 앞의 박스 캐스트 수행
        Vector2 detectionOrigin = transform.position + DetectDirection * detectionBoxSize.x * 0.5f; // 박스 캐스트 시작 위치 계산
        Collider2D[] hitColliders = Physics2D.OverlapBoxAll(detectionOrigin, detectionBoxSize, 0f);
        //Debug.Log("DetectObject ");

        // 충돌한 오브젝트 처리
        foreach (Collider2D collider in hitColliders)
        {
            if (!collider.isTrigger && collider.gameObject != this.gameObject && collider.gameObject.name != "Player_Reflection" && collider.gameObject.name != "GroundCheck")
            {
                Debug.Log("STOP: " + collider.gameObject.name);
                return true;
            }
        }
        return false;
    }

    private void OnDrawGizmos()
    {
        // 디버그용으로 검출 박스를 그리는 코드 (Scene 뷰에서만 보임)
        Vector3 detectionOrigin = transform.position + DetectDirection * detectionBoxSize.x * 0.5f;
        Gizmos.DrawWireCube(detectionOrigin, detectionBoxSize);
    }
}