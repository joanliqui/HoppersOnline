using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class NetBubbleGum : NetBaseHopper
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
            //StartCoroutine(ReturnGravity());

            OnUltimatePerformed?.Invoke();

            canUlt = false;
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
        Debug.Log("EndUltimate");
        canUlt = true;
    }
}
