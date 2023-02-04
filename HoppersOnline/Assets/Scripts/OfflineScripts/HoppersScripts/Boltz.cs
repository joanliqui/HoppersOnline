using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Boltz : BaseHopper
{
    [Space(20)]
    [SerializeField] GameObject rayPrefab;
    GameObject reusableRay;
    [SerializeField] Transform socket;

    [SerializeField] AudioClip ultimateClip;
    [SerializeField] AudioSource source;

    private void Start()
    {
        reusableRay = Instantiate(rayPrefab, new Vector3(0, -20, 0), Quaternion.identity);
        reusableRay.SetActive(false);

        onUltPerformed.AddListener(UltimateSound);

    }


    protected override void Abilitie(InputAction.CallbackContext ctx)
    {
        if (canUlt)
        {
            isUlting = true;
            canUlt = false;
            reusableRay.transform.position = socket.position;
            reusableRay.SetActive(true);
            cntUltTime = 0.0f;

            onUltPerformed?.Invoke();

        }
    }

    protected override void Gravity()
    {
        if (!isUlting)
        {
            base.Gravity();
        }
        else
        {
            if (!_isGrounded)
            {
                appliedMovement.y += Time.deltaTime * lowGravity;
            }
        }
    }

    private void UltimateSound()
    {
        source.clip = ultimateClip;
        source.pitch = 1;
        source.volume = 0.55f;
        source.Play();
    }
}
