using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;

public class NetBaseHopper : MonoBehaviour, IDamageable
{
    //Input Variables
    private float hDir = 0;
    private bool isJumpPressed;

    [Header("Movement Settings")]
    [SerializeField] protected float movSpeed = 4000;
    [Tooltip("A multiplier to reduce the mov Speed on air")]
    [SerializeField] float airMovementMultiplier = 0.8f;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] float groundCheckDistance = 0.02f;

    private bool _isFacingRight = true;
    private bool _isGrounded;
    private Vector2 appliedMovement;
    private Vector2 currentMovement;
    private Vector2 posLeftRay;
    private Vector2 posRightRay;

    [Header("JumpVariables")]
    [SerializeField] float maxJumpTime = 0.5f;
    [SerializeField] float initialJumpVelocity = 7000;
    [Range(0.0f, 1.0f)]
    [SerializeField] float jumpCutMomentum = 0.2f;
    private float cntTimeJumping;
    private bool isJumping;
    private bool isJumpCanceled = false;

    //gravity variables
    [SerializeField] float lowGravity = -9.8f;
    [SerializeField] float hardGravity;

    //Animator
    private Animator anim;
    private int hVelocityHashAnim;
    private int isGroundedHashAnim;
    private int vVelocityHashAnim;

    #region ComponentReferences
    private PhotonView view;
    private Controls _inputs;
    private Rigidbody2D rb;
    private Collider2D col;
    #endregion

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
        
        if (view.IsMine)
        {
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
                ReadJump(ctx);
            };
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

    private void Gravity()
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
                    isJumpPressed = false;
                }
            }
        }
        if (isJumpCanceled && isJumping)
        {
            CutJumpOnCancelOrApex();
            isJumpCanceled = false;
        }
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
    }
}
