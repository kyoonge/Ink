using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using DG.Tweening;

public class ClonePlayer : MonoBehaviour
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

    private Rigidbody2D rigid;

    #region PlayerMoveVariable
    private float horInput;
    private Vector3 playerScale;
    [SerializeField] private float playerSpeed;
    public float playerXScale;
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

    #region MonoBehaviour Method

    void Start()
    {
        jumpCount = 0;
        rigid = GetComponent<Rigidbody2D>();
        playerXScale = 0.9f;
        animator = GetComponent<Animator>();
        isReflection = false;
        isDead = false;
        groundCheckDirection = Vector2.up;
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
                rigid.velocity = new Vector3(rigid.velocity.x, -15, 0f);
            }
        }
    }
    #endregion

    void PlayerAct()
    {   // check Player Act     
        //PlayerJump();
        //PlayerMove();
    }

    public void PlayerJump(bool keySpace)
    {

        //ColorManager.instance.AutoSwitchMainColoring();
        rigid.velocity = Vector2.zero;
        jumpForce = Mathf.Sqrt(jumpHeight * -2 * (Physics2D.gravity.y * rigid.gravityScale*-1));
        Debug.Log(this.name + " " + Mathf.Sqrt(jumpHeight * -2 * (Physics2D.gravity.y * rigid.gravityScale*-1)));

        isJump = true;
        animator.Play("Boing");

        rigid.AddForce(new Vector2(0, -jumpForce), ForceMode2D.Impulse);
        //playerClone.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
       
    }

    public void PlayerMove(float horInput)
    {
        //horInput = Input.GetAxisRaw("Horizontal");

        playerScale = transform.localScale;
        if (horInput > 0)
        {
            playerScale.x = playerXScale;
        }
        else if (horInput < 0)
        {
            playerScale.x = -playerXScale;
        }
        transform.localScale = playerScale;
        transform.Translate(Vector2.right * Time.deltaTime * playerSpeed * horInput);

        if (horInput == 0)
        {
            animator.Play("Idle");
        }
        else
        {
            animator.Play("Boing");
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

        if (rayCastHit1.collider != null && !rayCastHit1.collider.isTrigger)
        {
            fdt = 0f;
            jumpCount = 0;
        }
        else
        {
            if (isJump)
            {
                jumpCount++;
                isJump = false;
            }
        }

        return rayCastHit1.collider != null;
    }

    void CheckGameOver()
    {
        if (fdt > gameEndFdt)
        {
            GameOver();
        }
    }

    public IEnumerator DeathCoroutine()
    {
        isDead = true;
        DeathEffect();
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
            _rb2d.AddTorque((_rnd * 2f - 1f) * _torquePower, ForceMode2D.Impulse);

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

    public void Reflection()
    {
        isReflection = false;

        //Ŭ�� ����
        playerClone.transform.position = this.transform.position + new Vector3(0, 0.4f, 0);
        playerClone.transform.localScale = Vector3.zero;
        playerClone.SetActive(true);
        //x,y ������ -1 ���ϱ�
        Vector3 reflectScale = new Vector3(transform.localScale.x * 1, transform.localScale.y * -1, transform.localScale.z);
        playerClone.transform.DOScale(reflectScale, 1f).SetEase(Ease.InQuad);
        //playerClone.transform.DOMoveX(transform.position.x, 0.5f);


        Debug.Log(playerClone.GetComponent<PlayerController>().groundCheckDirection);
        //�ٴڽ����� up����
        playerClone.GetComponent<PlayerController>().groundCheckDirection = Vector2.up;
        //���� ����
        playerClone.GetComponent<PlayerController>().isReflection = true;
        Debug.Log(playerClone.GetComponent<PlayerController>().groundCheckDirection);
    }
}
