using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BananaCollectable : MonoBehaviour
{
    [Header("Values")]
    public int bananaCount = 1;
    public int bananaScore = 100;

    [Header("Animation")]
    public Transform pointToReturnTo;
    public Animator animator;
    public float collectAnimationLength = 2f;
    public float maxSpinSpeed = 3f;

    private bool beenCollected = false;
    private Vector3 initialScale;

    // Start is called before the first frame update
    void Start()
    {
        initialScale = transform.localScale;
        pointToReturnTo.parent = transform.parent;
    }

    public void ResetBanana()
    {
        animator.SetFloat("Speed", 1);
        transform.parent = pointToReturnTo.parent;
        transform.localPosition = pointToReturnTo.localPosition;

        Sequence scaleUp = DOTween.Sequence();
        scaleUp.Append(transform.DOScale(initialScale, 1f).SetEase(Ease.OutBack));

        beenCollected = false;
    }

    private void OnTriggerEnter(Collider col)
    {
        if (!beenCollected)
        {
            if (col.gameObject.layer == 9 && PlayerController.instance.isMovable) //Player
            {
                beenCollected = true;

                transform.SetParent(PlayerController.instance.playerCamera.myCamera.transform);
                Vector3 forward = transform.localPosition - Vector3.zero;

                transform.localPosition = forward / 3;
                transform.localScale = transform.localScale / 3;
                StartCoroutine(BananaCollectAnimation());

                UIController.instance.AddBanana(bananaCount);
                UIController.instance.AddToScore(bananaScore);
            }
        }
    }

    private IEnumerator BananaCollectAnimation()
    {
        Sequence positionScale = DOTween.Sequence();
        positionScale.Append(transform.DOLocalMove(new Vector3(4, 3, 6.5f), collectAnimationLength).SetEase(Ease.InQuart));
        positionScale.Join(transform.DOScale(0.0f, collectAnimationLength).SetEase(Ease.InQuart).OnComplete(HideBanana));

        float time = 0;
        while (time < 1)
        {
            time += Time.deltaTime / (collectAnimationLength/2);
            animator.SetFloat("Speed", Mathf.Lerp(animator.GetFloat("Speed"), maxSpinSpeed, time));
            yield return null;
        }
    }

    private void HideBanana()
    {
        animator.SetFloat("Speed", 1);
        gameObject.SetActive(false);
    }
}
