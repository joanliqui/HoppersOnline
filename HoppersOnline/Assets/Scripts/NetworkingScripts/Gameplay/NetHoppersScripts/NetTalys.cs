using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;

public class NetTalys : NetBaseHopper
{
    [Header("Talys Attributes")]
    [SerializeField] int nMeteors = 4;
    [SerializeField] NetPool pool;
    Camera cam;
    float offsetPos = 7;
    float heightCam;
    float widthCam;

    private void Start()
    {
        cam = Camera.main;
        if (pool == null)
            pool = GetComponent<NetPool>();
    }
    protected override void Abilitie(InputAction.CallbackContext ctx)
    {
        if (canUlt)
        {
            isUlting = true;
            StartCoroutine(InvokeMeteors());

            canUlt = false;
            cntUltTime = 0.0f;
        }
    }

    IEnumerator InvokeMeteors()
    {
        heightCam = 2f * cam.orthographicSize;
        widthCam = heightCam * cam.aspect;

        for (int i = 0; i < nMeteors; i++)
        {
            Vector2 meteoPos = new Vector2(Random.Range(cam.gameObject.transform.position.x - widthCam / 2 + offsetPos, cam.gameObject.transform.position.x + widthCam / 2 - offsetPos),
                                                        cam.gameObject.transform.position.y + heightCam / 2 + 3f + Random.Range(0f, 2f));
            yield return new WaitForSeconds(0.2f);

            GameObject newMeteor = pool.Get();
            newMeteor.transform.position = meteoPos;
            PhotonView v = newMeteor.GetComponent<PhotonView>();
            view.RPC("ToogleObject", RpcTarget.All, v.ViewID, true);
        }
    }

    [PunRPC]
    private void ToogleObject(int id, bool active)
    {
        GameObject obj = PhotonNetwork.GetPhotonView(id).gameObject;
        obj.SetActive(active);
    }

    public override void EndUltimate()
    {
        isUlting = false;
        //canUlt = true;
    }

    public override void Damaged(Vector2 dir, float impulseForce)
    {
        if (!isUlting)
        {
            base.Damaged(dir, impulseForce);
        }
    }
}
