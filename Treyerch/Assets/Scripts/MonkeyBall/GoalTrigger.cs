using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoalTrigger : MonoBehaviour
{
	[Header("TimerUI")]
	public Text mainTimer;
	public Text secondaryTimer;

	[Header("Trigger")]
	public Animator animator;
	public Transform particleSpawnPoint;
	public GameObject particleToSpawn;
	public Transform Ribbon;
	public Vector3 forwardRotation = new Vector3(0, 0, 0);
	public Vector3 backwardRotation = new Vector3(0, -180, -120);

	private PlayerController player;
	private bool hasTriggered = false;

	private void Update()
    {
		if (player)
		{
			if (!hasTriggered)
			{
				float angle = Vector3.Angle(transform.up, player.transform.position - transform.position);
				if (Mathf.Abs(angle) < 90)
				{
					Ribbon.transform.localRotation = Quaternion.Euler(forwardRotation);
				}
				else
				{
					Ribbon.transform.localRotation = Quaternion.Euler(backwardRotation);
				}
			}
		}
		else
		{
			if (PlayerController.instance)
			{
				player = PlayerController.instance;
			}
		}
    }

    private void OnDrawGizmos()
    {
		Gizmos.DrawLine(transform.position, transform.position + (transform.up * 2));
    }

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
				hasTriggered = true;
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
