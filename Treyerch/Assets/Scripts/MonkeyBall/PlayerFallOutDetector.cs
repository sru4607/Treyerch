using UnityEngine;
using System.Collections;
using DG.Tweening;

public class PlayerFallOutDetector : MonoBehaviour 
{
	public LevelManager levelTilter;
	public Color falloutColor = Color.red;
	public bool drawFallOut;

	private PlayerController player;

	private void OnDrawGizmos()
	{
		if (drawFallOut)
		{
			Gizmos.color = falloutColor;
			Gizmos.DrawCube(transform.position, transform.localScale);
		}
	}

    void OnTriggerEnter(Collider c) 
	{
		if (c.gameObject.layer == 9) //Player
		{
			TriggerFallout();
		}
	}

	public void TriggerFallout()
    {
		if (player == null)
		{
			player = PlayerController.instance;
		}

		if (player.isMovable)
		{
			UIController.instance.minimapAnim.SetFloat("Speed", 50);
			UIController.instance.minimapAnim.SetTrigger("SpinOut");
			player.isMovable = false;
			player.gameObject.tag = "Untagged";
			player.playerCamera.SetTarget(null);

			player.playerCamera.player = player.transform;
			player.playerCamera.EnableFollowMode();

			Sequence scaleUp = DOTween.Sequence();
			scaleUp.Append(player.transform.DOScale(0.0f, 1.8f).SetEase(player.scaleUpEase));

			Invoke("ResetPlayer", 2f);
		}
	}

	private void ResetPlayer()
    {
		player.rigidBody.isKinematic = true;
		player.transform.localPosition = Vector3.zero;
		player.transform.localRotation = Quaternion.Euler(Vector3.zero);
		player.gameObject.tag = "Player";
		player.playerCamera.SetTarget(player.transform);
		player.playerCamera.ResetCamera();
		player.playerCamera.DisableFollowMode();
		levelTilter.ResetWorldTilt();
		if(UIController.instance)
        {
			UIController.instance.ResetScore();
		}
	}
}
