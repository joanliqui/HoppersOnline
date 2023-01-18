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
    private bool jumpBtn;

    [Header("Movement Settings")]
    [SerializeField] protected float movSpeed = 1000;
    [Tooltip("A multiplier to reduce the mov Speed on air")]
    [SerializeField] float airMovementMultiplier = 0.8f;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] float groundCheckDistance = 1;
    private bool isFacingRight = true;
    private Vector2 appliedMovement;
    private Vector2 currentMovement;
    private bool isGrounded;
    private Vector2 posLeftRay;
    private Vector2 posRightRay;

    [Header("Jump Settings")]
    [SerializeField] float maxJumpHeight = 1f;
    [SerializeField] float maxJumpTime = 0.5f;
    [SerializeField] float jumpDistance = 2f;
    private float initialJumpVelocity;
    bool isJumping = false;

    [Header("Gravity Attributes")]
    [SerializeField] protected float fastFallGravityMultiplier;
    private float groundedGravity = -0.5f;
    private float gravity = -9.8f;

    #region ComponentReferences
    private Controls _inputs;
    protected PhotonView view;
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
        view = GetComponent<PhotonView>();
        col = GetComponent<Collider2D>();

        _inputs.Player.Move.performed += ReadMovement;
        _inputs.Player.Move.canceled += ReadMovement;
        _inputs.Player.Jump.performed += ReadJump;
        _inputs.Player.Jump.canceled += ReadJump;
    }

    private void Update()
    {
        if (view.IsMine)
        {
            isGrounded = IsGrounded();

            if (isGrounded)
            {
                RunMovement();
            }

            Gravity();
            if (jumpBtn)
            {
                Jump();
            }

            if (hDir > 0 && !isFacingRight) Flip();
            else if (hDir < 0 && isFacingRight) Flip(); 
        }
    }

    private void FixedUpdate()
    {
        if (view.IsMine)
        {
            rb.velocity = appliedMovement * Time.deltaTime ;
        }
    }

    private void RunMovement()
    {
        appliedMovement.x = hDir * movSpeed;
    }


    private void Flip()
    {
        isFacingRight = !isFacingRight;
        transform.Rotate(0.0f, 180f, 0.0f);
    }

    protected void Gravity()
    {
        //EULER INTEGRATION: pues es la manera en la que he calculado la aceleracion toda la vida, pillas el valor, le sumas el de la aceleración, y lo metes en la
        //funcion de movimiento para que se mueva, y ya esta.
        //El frame rate puede hacer este salto inconsistente, y no saltar siempre de la misma manera, lo que puede llegar a ser frustrante

        //VELOCITY VERLET INTEGRATION: guardamos la velocida anterior, calculamos la siguiente velocidad con el Euler Integration, y los sumamos los dos
        //multiplicado por 0.5 y el Time Step
        bool isFalling = currentMovement.y <= -0.01f || !jumpBtn;
        if (isGrounded)
        {
            currentMovement.y = groundedGravity;
            appliedMovement.y = groundedGravity;
        }
        else if (isFalling) // Cuando esta cayendo
        {
            float previousYVelocity = currentMovement.y;
            currentMovement.y = currentMovement.y + (gravity * fastFallGravityMultiplier * Time.deltaTime);
            appliedMovement.y = Mathf.Max((previousYVelocity + currentMovement.y) * 0.5f, -20f);
        }
        else // Cuando acaba de saltar y esta subiendo
        {
            float previousYVelocity = currentMovement.y;
            currentMovement.y = currentMovement.y + (gravity * Time.deltaTime);
            appliedMovement.y = (previousYVelocity + currentMovement.y) * 0.5f; //(en este caso el Time.deltaTime no lo pongo porque ya estan en el RB.velocity)
        }
    }
    private void Jump()
    {
        if(!isJumping && isGrounded)
        {
            Debug.Log("Jump");
            isJumping = true;
            SetupJumpVariables();
            appliedMovement.y = initialJumpVelocity;
        }
    }
    private void SetupJumpVariables()
    {
        //En esta funcion calculamos la graviedad que ha de tener nuestro mundo y la velocidad inicial a la que ha de saltar el character para que
        //concuerde con el tiempo y la altura de salto que nosotors decidimos
        float distanceApex = jumpDistance / 2;
        gravity = (-2 * maxJumpHeight * Mathf.Pow(movSpeed, 2)) / Mathf.Pow(distanceApex, 2);

        initialJumpVelocity = (2 * maxJumpHeight * movSpeed) / distanceApex;
        Debug.Log("initialJumpVelocity:" + initialJumpVelocity);
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

    #region ReadInput
    private void ReadMovement(InputAction.CallbackContext ctx)
    {
        if (view.IsMine)
        {
            hDir = ctx.ReadValue<float>();
        }
    }

    private void ReadJump(InputAction.CallbackContext ctx)
    {
        jumpBtn = ctx.ReadValueAsButton();
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
}
