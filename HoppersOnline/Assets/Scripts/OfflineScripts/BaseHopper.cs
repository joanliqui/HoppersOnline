using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BaseHopper : MonoBehaviour, IDamageable
{
    //Input Variables
    protected float hDir = 0;
    protected bool isJumpPressed;
    protected bool pausePressed;

    [Header("Movement Settings")]
    [SerializeField] protected float movSpeed = 1000;
    [Tooltip("A multiplier to reduce the mov Speed on air")]
    [SerializeField] protected float airMovementMultiplier = 0.8f;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] float groundCheckDistance = 0.02f;

    protected bool _isFacingRight = true;
    protected bool _isGrounded;
    protected Vector2 appliedMovement;
    protected Vector2 currentMovement;
    private Vector2 posLeftRay;
    private Vector2 posRightRay;

    [Header("JumpVariables")] 
    [SerializeField] float maxJumpTime = 0.5f;
    [SerializeField] float initialJumpVelocity = 7000f;
    [Range(0.0f, 1.0f)]
    [SerializeField] float jumpCutMomentum = 0.2f;
    private float cntTimeJumping;
    protected bool isJumping;
    private bool isJumpCanceled = false;

    //gravity variables
    [SerializeField] float lowGravity = -20f;
    [SerializeField] float hardGravity = -100f;

    //Abilitie Variables;
    protected bool canUlt = true;
    protected bool isUlting = false;

    //Animator
    private Animator anim;
    private int hVelocityHashAnim;
    private int isGroundedHashAnim;
    private int vVelocityHashAnim;
    private int isUltingHashAnim;

    #region ComponentReferences
    private Controls _inputs;
    private Rigidbody2D rb;
    private Collider2D col;
    #endregion

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

        hVelocityHashAnim = Animator.StringToHash("hVelocity");
        isGroundedHashAnim = Animator.StringToHash("isGrounded");
        vVelocityHashAnim = Animator.StringToHash("vVelocity");
        isUltingHashAnim = Animator.StringToHash("isUlting");

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

        _inputs.Player.Ability.started += Abilitie;

    }

    private void Update()
    {
        _isGrounded = IsGrounded();

        HorizontalMovement();
        
        Gravity();
        Jump();

        rb.velocity = appliedMovement * Time.deltaTime;
        
        if (hDir > 0 && !_isFacingRight) Flip();
        else if (hDir < 0 && _isFacingRight) Flip();

        UpdateAnimations();
    }

    protected virtual void HorizontalMovement()
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

    protected virtual void Gravity()
    {
        if(!isJumping && _isGrounded)
        {
            currentMovement.y = -0.1f;
            appliedMovement.y = -0.1f;
        }
        else if(isJumping && !_isGrounded)
        {
            if(appliedMovement.y > 0)
            {
                appliedMovement.y += lowGravity * Time.deltaTime;
            }
            else //Apex
            {
                appliedMovement.y += hardGravity;
            }
        }
        else if(!isJumping && !_isGrounded) //Cayendo
        {
            appliedMovement.y += hardGravity;
        }
        else if(isJumping && !_isGrounded)
        {
            Debug.Log("Pa tocar los huevos");
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

    private void Jump()
    {
        if (isJumpPressed)
        {
            if(!isJumping && _isGrounded)
            {
                cntTimeJumping = 0;
                appliedMovement.y = initialJumpVelocity;
                isJumping = true;
            }
            else if (isJumping)
            {   
                if(cntTimeJumping < maxJumpTime)
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
        //if (isJumpCanceled && isJumping)
        //{
        //    CutJumpOnCancelOrApex();
        //    isJumpCanceled = false;
        //}
    }

    private void CutJumpOnCancelOrApex()
    {
        appliedMovement.y = appliedMovement.y * jumpCutMomentum;
        isJumping = false;
        Debug.Log("Cuted");
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

    #region Interfaces
    public void Damaged()
    {
        throw new System.NotImplementedException();
    }

    public void Damaged(Vector2 dir, float impulseForce)
    {
        throw new System.NotImplementedException();
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
}
