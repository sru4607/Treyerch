using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PresentCamera : MonoBehaviour
{
    public GameObject player;
    public Animator animator;
    public Camera playerCamera;
    public Camera presentCamera;
    public LevelManager levelManager;


    public void EnablePlayer()
    {
        animator.enabled = false;
        player.gameObject.SetActive(true);
        levelManager.DoDOFFadeIn();
    }

    public void StartSwitch()
    {
        Sequence cameraFinal = DOTween.Sequence();
        cameraFinal.Append(presentCamera.transform.DOMove(playerCamera.transform.position, 0.2f).SetEase(Ease.Linear).OnComplete(DoSwitch));
    }

    private void DoSwitch()
    {
        gameObject.SetActive(false);
        playerCamera.gameObject.SetActive(true);
    }
}
