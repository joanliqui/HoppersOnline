using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;

public class NetBaseHopper : MonoBehaviour, IDamageable
{
    [SerializeField] CharacterSO hopperdCard;
    public int playerNumber;

    private bool isPaused = false;
    //Input Variables
    private float hDir = 0;
    private bool isJumpPressed;
    private bool pausedPressed;
    [Space(20)]
    [Header("Movement Settings")]
    [SerializeField] protected float movSpeed = 4000;
    [Tooltip("A multiplier to reduce the mov Speed on air")]
    [SerializeField] float airMovementMultiplier = 0.8f;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] float groundCheckDistance = 0.02f;

    private bool _isFacingRight = true;
    private bool _isGrounded;
    protected Vector2 appliedMovement;
    protected Vector2 currentMovement;
    private Vector2 posLeftRay;
    private Vector2 posRightRay;

    [Header("JumpVariables")]
    [SerializeField] float maxJumpTime = 0.5f;
    [SerializeField] float initialJumpVelocity = 7000;
    [Range(0.0f, 1.0f)]
    [SerializeField] float jumpCutMomentum = 0.2f;
    private float cntTimeJumping;
    protected bool isJumping;
    private bool isJumpCanceled = false;

    //gravity variables
    [SerializeField] float lowGravity = -20f;
    [SerializeField] float hardGravity = -100f;


    //Ultimate Variables;
    [Header("UltimateVariables")]
    [SerializeField] protected float cooldown = 3f;
    [SerializeField] NetUltBarController ultBarPrefab;
    protected float cntUltTime = 0;
    protected bool canUlt = false;
    protected bool isUlting = false;
    private bool startCld = false;

    public delegate void UltCharging(float progres);
    public event UltCharging OnUltCharging;

    //Animator
    private Animator anim;
    private int hVelocityHashAnim;
    private int isGroundedHashAnim;
    private int vVelocityHashAnim;
    private int isUltingHashAnim;

    //Pause
    public delegate void PausePerformed();
    public event PausePerformed OnPausePerformed;

    #region ComponentReferences
    protected PhotonView view;
    private Controls _inputs;
    private Rigidbody2D rb;
    private Collider2D col;
    #endregion

    public PhotonView View { get => view;}
    public float Cooldown { get => cooldown; set => cooldown = value; }
    public CharacterSO HopperdCard { get => hopperdCard; set => hopperdCard = value; }

    #region Enable/Disable
    private void OnEnable()
    {
        if (view.IsMine)
        {
            _inputs.Enable();
        }
    }

    private void OnDisable()
    {
        if (view.IsMine)
        {
            _inputs.Disable();
        }
    }
    #endregion

    private void Awake()
    {
        _inputs = new Controls();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        anim = GetComponentInChildren<Animator>();
        view = GetComponent<PhotonView>();

        hVelocityHashAnim = Animator.StringToHash("hVelocity");
        isGroundedHashAnim = Animator.StringToHash("isGrounded");
        vVelocityHashAnim = Animator.StringToHash("vVelocity");
        isUltingHashAnim = Animator.StringToHash("isUlting");

        if (view.IsMine)
        {
            OnPausePerformed += ToggleInputMap;

            _inputs.Player.Move.performed += ReadMovement;
            _inputs.Player.Move.canceled += ReadMovement;
            _inputs.Player.Jump.started += ctx =>
            {
                isJumpCanceled = false;
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
                pausedPressed = true;
            };

            //Ability
            _inputs.Player.Ability.started += Abilitie;

        }

    }

    private void Update()
    {
        if (view.IsMine)
        {
            _isGrounded = IsGrounded();

            HorizontalMovement();

            Gravity();
            Jump();

            rb.velocity = appliedMovement * Time.deltaTime;

            if (hDir > 0 && !_isFacingRight) Flip();
            else if (hDir < 0 && _isFacingRight) Flip();

            if(startCld)
                CooldownUltimate();

            UpdateAnimations();
        }
    }
    private void HorizontalMovement()
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

    protected virtual void Gravity()
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
                appliedMovement.y += hardGravity;
            }
        }
        else if (!isJumping && !_isGrounded) //Cayendo
        {
            appliedMovement.y += hardGravity;
        }
        else if (isJumping && !_isGrounded)
        {
            Debug.Log("Pa tocar los huevos");
        }
    }
    private void Jump()
    {
        if (isJumpPressed)
        {
            if (!isJumping && _isGrounded)
            {
                cntTimeJumping = 0;
                appliedMovement.y = initialJumpVelocity;
                isJumping = true;
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
                    isJumping = false;
                    isJumpPressed = false;
                }
            }
        }
    }
    protected virtual void CutJumpOnCancelOrApex()
    {
        appliedMovement.y = appliedMovement.y * jumpCutMomentum;
        isJumping = false;
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

    #region INTERFACES
    public void Damaged()
    {
        throw new NotImplementedException();
    }

    public void Damaged(Vector2 dir, float impulseForce)
    {
        throw new NotImplementedException();
    }
    #endregion

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(posLeftRay, new Vector2(posLeftRay.x, posLeftRay.y - groundCheckDistance));
        Gizmos.DrawLine(posRightRay, new Vector2(posRightRay.x, posRightRay.y - groundCheckDistance));
    }

    private void UpdateAnimations()
    {
        anim.SetBool(isGroundedHashAnim, _isGrounded);
        anim.SetFloat(hVelocityHashAnim, Mathf.Abs(rb.velocity.x));
        anim.SetFloat(vVelocityHashAnim, rb.velocity.y);
        anim.SetBool(isUltingHashAnim, isUlting);
    }

    public void ToggleInputMap()
    {
        view.RPC("ToggleInputMapRPC", RpcTarget.All);
    }

    [PunRPC]
    protected void ToggleInputMapRPC()
    {
        if (view.IsMine)
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
    }

    protected virtual void Abilitie(InputAction.CallbackContext ctx)
    {

    }

    protected IEnumerator DisableInputCoroutine(float sec)
    {
        _inputs.Player.Disable();
        yield return new WaitForSeconds(sec);
        _inputs.Player.Enable();
    }

    public virtual void EndUltimate()
    {

    }

    public void DisableAllInput()
    {
        if(view.IsMine)
            _inputs.Player.Disable();
    }

    public void StartCooldown()
    {
        startCld = true;
    }
}
