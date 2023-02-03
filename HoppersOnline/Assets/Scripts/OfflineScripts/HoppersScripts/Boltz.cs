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

    private void Start()
    {
        reusableRay = Instantiate(rayPrefab, new Vector3(0, -20, 0), Quaternion.identity);
        reusableRay.SetActive(false);
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
        }
    }
}
