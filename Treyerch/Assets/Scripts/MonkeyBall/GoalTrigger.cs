using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalTrigger : MonoBehaviour
{
	public Animator animator;
	public Transform particleSpawnPoint;
	public GameObject particleToSpawn;
	private PlayerController player;

	private void OnTriggerEnter(Collider c)
	{
		if (c.gameObject.layer == 9) //Player
		{
			if (player == null)
			{
				player = PlayerController.instance;
			}

			if (player.isMovable)
			{
				animator.SetFloat("Speed", 1);
				GameObject newParticles = Instantiate(particleToSpawn, particleSpawnPoint.position, particleSpawnPoint.rotation);
				newParticles.transform.localScale = particleSpawnPoint.localScale;
				newParticles.transform.parent = particleSpawnPoint.parent.parent;

				player.levelManager.DoDOFFadeOut();
				player.goalReached = true;
				player.isMovable = false;
				player.gameObject.tag = "Untagged";
				player.playerCamera.SetTarget(null);
				player.playerCamera.player = player.transform;
			}
		}
	}
}
