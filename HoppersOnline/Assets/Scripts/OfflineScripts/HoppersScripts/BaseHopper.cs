using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class BaseHopper : MonoBehaviour, IDamageable
{
    [SerializeField] CharacterSO hopperdCard;

    //Input Variables
    protected float hDir = 0;
    protected bool isJumpPressed;
    protected bool pausePressed;
    private bool pausedPressed;

    [Space(20)]
    [Header("Movement Settings")]
    [SerializeField] protected float movSpeed = 900;
    [Tooltip("A multiplier to reduce the mov Speed on air")]
    [Range(0, 1)]
    [SerializeField] protected float airMovementMultiplier = 0.8f;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] float groundCheckDistance = 0.02f;

    protected bool _isFacingRight = true;
    protected bool _isGrounded;
    protected Vector2 appliedMovement;
    protected Vector2 currentMovement;
    private Vector2 posLeftRay;
    private Vector2 posRightRay;

    //Roof Checker
    [SerializeField] float roofCheckDistance = 0.1f;
    protected Vector2 posRoofLeft;
    protected Vector2 posRoofRight;
    bool roofTouch;

    [Header("JumpVariables")] 
    [SerializeField] float maxJumpTime = 0.15f;
    [SerializeField] float initialJumpVelocity = 3000;
    [Range(0.0f, 1.0f)]
    [SerializeField] float jumpCutMomentum = 0.2f;
    [SerializeField] float coyoteTime = 0.1f;
    [SerializeField] float jumpBufferTime = 0.15f;

    protected bool isJumping;
    private bool bufferJump;
    private float cntTimeJumping;
    private bool isJumpCanceled = false;
    private float cntCoyoteTime;
    private float cntJumpBufferTime;

    //gravity variables
    [SerializeField] protected float lowGravity = -20f;
    [SerializeField] protected float hardGravity = -90f;

    //Ultimate Variables;
    [Header("UltimateVariables")]
    [SerializeField] protected float cooldown = 3f;
    [SerializeField] UltBarController ultBarPrefab;
    protected float cntUltTime = 0;
    protected bool canUlt = false;
    protected bool isUlting = false;
    private bool startCld = false;
    protected UnityEvent onUltPerformed = new UnityEvent();

    public delegate void UltCharging(float progres);
    public event UltCharging OnUltCharging;

    //Damaged
    [SerializeField] Color damagedColor;
    private SpriteRenderer sr;
    private bool isDamaged = false;

    //Animator
    private Animator anim;
    private int hVelocityHashAnim;
    private int isGroundedHashAnim;
    private int vVelocityHashAnim;
    private int isUltingHashAnim;
    private int isDamagedHashAnim;

    //Pause
    private bool isPaused = false;
    public delegate void PausePerformed();
    public event PausePerformed OnPausePerformed;

    #region ComponentReferences
    private Controls _inputs;
    private Rigidbody2D rb;
    private Collider2D col;

    #endregion

    public float Cooldown { get => cooldown; set => cooldown = value; }
    public CharacterSO HopperdCard { get => hopperdCard; set => hopperdCard = value; }


    #region Enable/Disable
    private void OnEnable()
    {
        _inputs.Enable();
    }

    private void OnDisable()
    {
        _inputs.Disable();
    }
    #endregion

    private void Awake()
    {
        _inputs = new Controls();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        anim = GetComponentInChildren<Animator>();
        sr = GetComponentInChildren<SpriteRenderer>();

        hVelocityHashAnim = Animator.StringToHash("hVelocity");
        isGroundedHashAnim = Animator.StringToHash("isGrounded");
        vVelocityHashAnim = Animator.StringToHash("vVelocity");
        isUltingHashAnim = Animator.StringToHash("isUlting");
        isDamagedHashAnim = Animator.StringToHash("isDamaged");

        cntJumpBufferTime = jumpBufferTime;


        _inputs.Player.Move.performed += ReadMovement;
        _inputs.Player.Move.canceled += ReadMovement;
        _inputs.Player.Jump.started += ctx =>
        {
            isJumpCanceled = false;
            if (!_isGrounded)
            {
                cntJumpBufferTime = 0;
                bufferJump = false;
            }
            ReadJump(ctx);
        };
        _inputs.Player.Jump.canceled += ctx => 
        {
            isJumpCanceled = true;
            if (isJumping)
            {
                CutJumpOnCancelOrApex();
            }

            ReadJump(ctx);
        };

        //Pause
        _inputs.Player.Pause.started += ctx =>
        {
            pausedPressed = true;
            OnPausePerformed?.Invoke();
        };
        _inputs.Player.Pause.canceled += ctx =>
        {
            pausedPressed = false;
        };
        OnPausePerformed += ToggleInputMap;

        //Ability
        _inputs.Player.Ability.started += Abilitie;

        SpawnUltBar();
    }



    private void Update()
    {
        _isGrounded = IsGrounded();
        roofTouch = IsTouchingRoof();

        CoyoteTimeHandler();

        if (cntJumpBufferTime < jumpBufferTime)
        {
            cntJumpBufferTime += Time.deltaTime;
            if (_isGrounded && !isJumping)
            {
                bufferJump = true;
            }
        }

        HorizontalMovement();
        
        if (roofTouch && appliedMovement.y > 0)
        {
            appliedMovement.y = 0;
            currentMovement.y = 0;
        }

        Gravity();
        Jump();
        
        if (hDir > 0 && !_isFacingRight) Flip();
        else if (hDir < 0 && _isFacingRight) Flip();

        if(startCld)
            CooldownUltimate();

        UpdateAnimations();
    }

    private void FixedUpdate()
    {
        rb.velocity = appliedMovement * Time.deltaTime;
    }

    protected virtual void HorizontalMovement()
    {
        if (!isDamaged)
        {
            if (_isGrounded)
            {
                appliedMovement.x = hDir * movSpeed;
            }
            else
            {
                appliedMovement.x = hDir * movSpeed * airMovementMultiplier;
            }
        }
    }
    private void Flip()
    {
        _isFacingRight = !_isFacingRight;
        transform.Rotate(0.0f, 180f, 0.0f);
    }

    private bool IsGrounded()
    {
        float xLeft = col.bounds.min.x;
        float y = col.bounds.min.y;
        float xRight = col.bounds.max.x;

        posLeftRay = new Vector2(xLeft, y);
        posRightRay = new Vector2(xRight, y);

        bool groundLeft = Physics2D.Raycast(posLeftRay, Vector2.down, groundCheckDistance, groundLayer);
        bool groundRight = Physics2D.Raycast(posRightRay, Vector2.down, groundCheckDistance, groundLayer);

        if (groundLeft || groundRight)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool IsTouchingRoof()
    {
        float xLeft = col.bounds.min.x;
        float y = col.bounds.max.y;
        float xRight = col.bounds.max.x;

        posRoofLeft = new Vector2(xLeft, y); 
        posRoofRight = new Vector2(xRight, y);

        bool roofLeft = Physics2D.Raycast(posRoofLeft, Vector2.up, roofCheckDistance, groundLayer);
        bool roofRight = Physics2D.Raycast(posRoofRight, Vector2.up, roofCheckDistance, groundLayer);

        if(roofLeft || roofRight)
        {
            return true;
        }
        else
        {
            return false;
        }

    }

    private void CoyoteTimeHandler()
    {
        if (_isGrounded)
        {
            cntCoyoteTime = 0;
        }
        else
        {
            if (cntCoyoteTime < coyoteTime)
            {
                cntCoyoteTime += Time.deltaTime;
            }
        }
    }

    protected virtual void Gravity()
    {
        if (!isDamaged)
        {
            if (!isJumping && _isGrounded)
            {

                currentMovement.y = -0.1f;
                appliedMovement.y = -0.1f;

            }
            else if (isJumping && !_isGrounded)
            {
                if (appliedMovement.y > 0)
                {
                    appliedMovement.y += lowGravity * Time.deltaTime;
                }
                else //Apex
                {
                    appliedMovement.y += hardGravity * Time.deltaTime;
                }
            }
            else if (!isJumping && !_isGrounded) //Cayendo
            {
                appliedMovement.y += hardGravity * Time.deltaTime * 100;
            }
            else if (isJumping && !_isGrounded)
            {
                Debug.Log("Pa tocar los huevos");
            }
        }
    }

    private void Jump()
    {
        if (isJumpPressed || bufferJump)
        {
            if (!isJumping)
            {
                if (_isGrounded || cntCoyoteTime < coyoteTime)
                {
                    cntTimeJumping = 0;
                    cntJumpBufferTime = jumpBufferTime;
                    appliedMovement.y = initialJumpVelocity;

                    isJumping = true;
                }
            }
            else if (isJumping)
            {
                if (cntTimeJumping < maxJumpTime)
                {
                    cntTimeJumping += Time.deltaTime;
                    appliedMovement.y = initialJumpVelocity;
                }
                else
                {
                    CutJumpOnCancelOrApex();
                    isJumpPressed = false;
                }
            }
        }
    }

    private void CutJumpOnCancelOrApex()
    {
        appliedMovement.y = appliedMovement.y * jumpCutMomentum;
        isJumping = false;
        bufferJump = false;
    }
    protected void CooldownUltimate()
    {
        if (!canUlt)
        {
            if (cntUltTime < cooldown)
            {
                cntUltTime += Time.deltaTime;
            }
            else
            {
                canUlt = true;
                cntUltTime = cooldown;
            }
            OnUltCharging?.Invoke(cntUltTime);
        }
    }


    #region ReadInput
    private void ReadMovement(InputAction.CallbackContext ctx)
    {
        hDir = ctx.ReadValue<float>();
    }

    private void ReadJump(InputAction.CallbackContext ctx)
    {
        isJumpPressed = ctx.ReadValueAsButton();
    }

    #endregion

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(posLeftRay, new Vector2(posLeftRay.x, posLeftRay.y - groundCheckDistance));
        Gizmos.DrawLine(posRightRay, new Vector2(posRightRay.x, posRightRay.y - groundCheckDistance));

        Gizmos.DrawLine(posRoofLeft, new Vector2(posRoofLeft.x, posRoofLeft.y + roofCheckDistance));
        Gizmos.DrawLine(posRoofRight, new Vector2(posRoofRight.x, posRoofRight.y + roofCheckDistance));
    }

    private void UpdateAnimations()
    {
        anim.SetBool(isGroundedHashAnim, _isGrounded);
        anim.SetFloat(hVelocityHashAnim, Mathf.Abs(rb.velocity.x));
        anim.SetFloat(vVelocityHashAnim, rb.velocity.y);
        anim.SetBool(isUltingHashAnim, isUlting);
        anim.SetBool(isDamagedHashAnim, isDamaged);
    }

    protected void ToggleInputMap()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            _inputs.Player.Move.Disable();
            _inputs.Player.Jump.Disable();
            _inputs.Player.Ability.Disable();
        }
        else
        {
            _inputs.Player.Move.Enable();
            _inputs.Player.Jump.Enable();
            _inputs.Player.Ability.Enable();
        }
        
    }

    protected virtual void Abilitie(InputAction.CallbackContext ctx)
    {
        cntUltTime = 0;
    }

    protected IEnumerator DisableInputCoroutine(float sec)
    {
        _inputs.Player.Disable();
        yield return new WaitForSeconds(sec);
        _inputs.Player.Enable();
    }

    public virtual void EndUltimate()
    {
        isUlting = false;
    }

    protected void SpawnUltBar()
    {
        Transform p = GameObject.FindGameObjectWithTag("UltBarContainer").transform;
        UltBarController ultBar = Instantiate(ultBarPrefab, p);
        ultBar.SetHopper(this);
    }

    public void DisableAllInput()
    {
        _inputs.Player.Disable();
    }

    public void StartCooldown()
    {
        startCld = true;
    }

    #region DAMAGE REGION
    public void Damaged()
    {
        throw new System.NotImplementedException();
    }

    public void Damaged(Vector2 dir, float impulseForce)
    {
        isDamaged = true;
        appliedMovement = dir * impulseForce;

        StartCoroutine(DisableInputCoroutine(0.1f));
        StartCoroutine(StopDamage());

        StartCoroutine(DamagedColor());
    }

    public void Damaged(float x, float y, float impulseForce, int id)
    {
      
        Vector2 dir = new Vector2(x, y);
        isDamaged = true;
        appliedMovement = dir * impulseForce;

        StartCoroutine(DisableInputCoroutine(0.1f));
        StartCoroutine(StopDamage());

        StartCoroutine(DamagedColor());
    }
    private IEnumerator StopDamage()
    {
        yield return new WaitForSeconds(0.15f);
        isDamaged = false;
    }

    private IEnumerator DamagedColor()
    {
        sr.color = damagedColor;
        yield return new WaitForSeconds(0.1f);
        sr.color = Color.white;
        yield return new WaitForSeconds(0.1f);
        sr.color = damagedColor;
        yield return new WaitForSeconds(0.1f);
        sr.color = Color.white;

    }
    #endregion


}
