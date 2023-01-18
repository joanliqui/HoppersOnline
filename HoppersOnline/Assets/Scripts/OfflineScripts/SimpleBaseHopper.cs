using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SimpleBaseHopper : MonoBehaviour
{
    //InputVariables
    protected float hDir;
    private bool jumpBtn;

    [Header("Movement Settings")]
    [SerializeField] protected float movSpeed = 1000;
    [Tooltip("A multiplier to reduce the mov Speed on air")]
    [SerializeField] float airMovementMultiplier = 0.8f;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] float groundCheckDistance = 1;
    protected bool isFacingRight = true;
    private bool isGrounded;
    private Vector2 posLeftRay;
    private Vector2 posRightRay;

    [Header("Jump Attributes")]
    [Tooltip("The force with which the player will jump")]
    [SerializeField] float jumpForce = 10;
    protected bool isJumping = false;
    protected bool startJumping = false;

    [Header("Gravity Attributes")]
    [SerializeField] protected float fastFallGravity;
    [SerializeField] protected float lowJumpGravity;

    #region ComponentReferences
    private Controls _inputs;
    private Rigidbody2D rb;
    private Collider2D col;
    #endregion


    private void OnEnable()
    {
        _inputs.Enable();
    }
    private void OnDisable()
    {
        _inputs.Disable();
    }

    void Awake()
    {
        _inputs = new Controls();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();

        _inputs.Player.Move.performed += ReadMovement;
        _inputs.Player.Move.canceled += ReadMovement;
        _inputs.Player.Jump.performed += ReadJump;
        _inputs.Player.Jump.canceled += ReadJump;
    }

    protected virtual void Update()
    {
        isGrounded = IsGrounded();
        if (isGrounded)
        {
            if (!startJumping)
            {
                isJumping = false;
            }
            Movement();
        }


        if (isFacingRight && hDir < 0)
        {
            Flip();
        }
        else if (!isFacingRight && hDir > 0)
        {
            Flip();
        }
    }

    private void FixedUpdate()
    {

        Gravity();

        if (jumpBtn && isGrounded) // clickas a saltar y estas tocando el suelo
        {
            if (rb.velocity.y > -0.1f && rb.velocity.y < 0.1f)//Si no estas cayendo(arregla el bug del doble salto)
            {
                Jump();
            }
        }
    }

    protected virtual void Movement()
    {
        rb.velocity = new Vector2(hDir * Time.deltaTime * movSpeed, rb.velocity.y);
    }
    void Flip()
    {
        isFacingRight = !isFacingRight;
        transform.Rotate(0.0f, 180f, 0.0f);
    }
    private void Jump()
    {
        startJumping = true;

        StartCoroutine(StopJumping());
        isJumping = true;
        rb.AddForce(Vector2.up * jumpForce * Time.deltaTime, ForceMode2D.Impulse);
    }

    IEnumerator StopJumping()
    {
        yield return new WaitForSeconds(0.001f);
        startJumping = false;
    }

    private void Gravity()
    {
        if (rb.velocity.y < -0.001)
        {
            rb.gravityScale = fastFallGravity;
        }
        else if (rb.velocity.y > 0.001 && !jumpBtn)
        {
            rb.gravityScale = lowJumpGravity;
        }
        else
        {
            rb.gravityScale = 1f;
        }
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
    public virtual void Abilitie()
    {

    }
    public virtual void EndAbilitie()
    {
       
    }

    #region ReadInput
    private void ReadMovement(InputAction.CallbackContext ctx)
    {
        hDir = ctx.ReadValue<float>();
    }

    private void ReadJump(InputAction.CallbackContext ctx)
    {
        jumpBtn = ctx.ReadValueAsButton();
    }
    #endregion
}
