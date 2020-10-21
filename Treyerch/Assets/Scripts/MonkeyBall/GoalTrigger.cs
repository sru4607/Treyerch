using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalTrigger : MonoBehaviour
{
	public Animator animator;
	private PlayerController player;

	private void OnTriggerEnter(Collider c)
	{
		if (c.gameObject.layer == 9) //Player
		{
			if (player == null)
			{
				player = PlayerController.instance;
			}

			animator.SetFloat("Speed", 1);

			player.goalReached = true;
			player.isMovable = false;
			player.gameObject.tag = "Untagged";
			player.playerCamera.SetTarget(null);
			player.playerCamera.player = player.transform;
		}
	}
}
