using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class BubbleGum : BaseHopper
{
    [Space(10)]
    [Header("BubbleGumVariables")]
    [SerializeField] float initialUltimateVelocity;
    [SerializeField] UnityEvent OnUltimatePerformed;

    protected override void Abilitie(InputAction.CallbackContext ctx)
    {
        if (canUlt)
        {
            isUlting = true;
            isJumping = false;

            ImpulseUp();
            StartCoroutine(DisableInputCoroutine(0.1f));

            OnUltimatePerformed?.Invoke();

            canUlt = false;
            cntUltTime = 0.0f;
        }
    }

    private void ImpulseUp()
    {
        appliedMovement.y = initialUltimateVelocity; 
    }

    private IEnumerator ReturnGravity()
    {
        yield return new WaitForSeconds(0.08f);
        isUlting = false;
    }

    protected override void Gravity()
    {
        if (!isUlting)
        {
            base.Gravity();
        }
    }

    public override void EndUltimate()
    {
        isUlting = false;
        //canUlt = true;
    }
}
