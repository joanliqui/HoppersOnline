using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class LettersAnimation : MonoBehaviour
{
    [SerializeField] RectTransform[] letters;
    private float[] initPos = new float[7];
    private float[] finalPos = new float[7];
    private bool startJump = false;
    private bool swapLetter = false;
    private bool canJump = true;
    private int actualLetter;
    private int lastLetter;

    [SerializeField] float jumpHeight = 20f;
    private void Awake()
    {
        for (int i = 0; i < letters.Length; i++)
        {
            initPos[i] = letters[i].localPosition.y;
            finalPos[i] = initPos[i] + jumpHeight;
        }

        letters[0].gameObject.SetActive(false);
    }
    //La animación inicial de las letras apareciendo se hace a traves del Timeline, por algun motivo solo funciona con una animacion Recorded en timeline, no
    //puedo usar una animación grabada en un animation clip
    void Start()
    {
        startJump = false;
        swapLetter = false;
        canJump = true;
        lastLetter = -10;

        actualLetter = RandomLetter();
    }

    private void Update()
    {
        if (startJump)
        {
            if (!swapLetter)
            {
                if (canJump)
                {
                    BounceLetter();
                    canJump = false;
                }
            }
            else
            {
                actualLetter = RandomLetter();
                swapLetter = false;
                canJump = true;
            }
        }
    }
    private int RandomLetter()
    {
        lastLetter = actualLetter;
        int n;
        do
        {
            n = Random.Range(0, letters.Length);

        } while (lastLetter == n);

        return n;
    }
    private void BounceLetter()
    {
        Sequence mySeq = DOTween.Sequence();

        mySeq.Append(letters[actualLetter].DOLocalMoveY(finalPos[actualLetter], 0.3f).SetEase(Ease.OutQuad));
        mySeq.Append(letters[actualLetter].DOLocalMoveY(initPos[actualLetter], 0.3f).SetEase(Ease.InQuad));
        mySeq.OnComplete(FunctionWaitTime);

    }
    private void FunctionWaitTime()
    {
        StartCoroutine(WaitTheTime());
    }
    private IEnumerator WaitTheTime()
    {
        yield return new WaitForSeconds(Random.Range(0.2f, 1f));
        swapLetter = true;
    }
    public void StartBounceLetters()
    {
        startJump = true;
    }
    public void KillAllSequences()
    {
        DOTween.KillAll();
    }
}
