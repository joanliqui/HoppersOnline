using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class CountdownManager : MonoBehaviour
{
    [SerializeField] int cdwTime = 3;
    [SerializeField] TextMeshProUGUI cdwText;
    RectTransform rectCdwTime;
    [SerializeField] string Go = "Hopp!";

    private void Start()
    {
        cdwText.text = cdwTime.ToString();
        rectCdwTime = cdwText.GetComponent<RectTransform>();

        StartCountdown();
    }

    public void StartCountdown()
    {
        StartCoroutine(CountDown());
    }

    IEnumerator CountDown()
    {
        while (cdwTime > 0)
        {
            if (cdwText)
            {
                cdwText.text = cdwTime.ToString();
            }

            rectCdwTime.localScale = Vector3.zero; //Pone la scale a 0

            Tweener scalingTweener = rectCdwTime.DOScale(Vector2.one * 2, 1f); //Anima la Scale
            scalingTweener.SetEase(Ease.OutBack);

            yield return new WaitForSeconds(1f);
            cdwTime--;
        }

        cdwText.text = Go;
        yield return new WaitForSeconds(1f);
        SoloGameManager.Instance.onGameStarted?.Invoke();
        rectCdwTime.gameObject.SetActive(false);
    }
}
