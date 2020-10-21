using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalTrack : MonoBehaviour
{
    public Animator animator;
    public float movementVelocityCutoff = 0.1f;
    public float runSpeed = 7f;
    public float idleTimerMovementCutoff = 2f;
    public float idleTimerMax = 5f;
    public float maxWalkSpeed = 12;

    private float idleTimer;
    private Transform playerCamera;
    private Vector3 movement;
    private Rigidbody playerRigidbody;
    private bool hasJumped;

    // Start is called before the first frame update
    void Start()
    {
        playerCamera = PlayerController.instance.playerCamera.transform;
        playerRigidbody = PlayerController.instance.rigidBody;
    }

    // Update is called once per frame
    void Update()
    {
        if (!PlayerController.instance.startedLaunch)
        {
            transform.rotation = Quaternion.Euler(0, playerCamera.transform.rotation.eulerAngles.y, playerCamera.transform.rotation.eulerAngles.z);
        }

        if (PlayerController.instance.isGrounded)
        {
            if (hasJumped)
            {
                hasJumped = false;
                animator.SetBool("isJumping", false);
                animator.SetBool("isRunning", false);
            }

            Vector3 walkingVelocity = new Vector3(playerRigidbody.velocity.x, 0, playerRigidbody.velocity.z);

            if (walkingVelocity.magnitude > movementVelocityCutoff)
            {
                animator.SetBool("isIdling", false);
                idleTimer = 0;

                if (walkingVelocity.magnitude >= runSpeed)
                {
                    animator.SetFloat("WalkSpeed", Mathf.Clamp(walkingVelocity.magnitude, 0, maxWalkSpeed) / 1.4f);
                    animator.SetBool("isRunning", true);
                    animator.SetBool("isWalking", false);
                }
                else
                {
                    animator.SetFloat("WalkSpeed", Mathf.Clamp(walkingVelocity.magnitude, 0, maxWalkSpeed) / 2);
                    animator.SetBool("isWalking", true);
                    animator.SetBool("isRunning", false);
                }
            }
            else
            {
                if (idleTimer < idleTimerMax)
                {
                    idleTimer += Time.deltaTime;

                    if (idleTimer > idleTimerMovementCutoff)
                    {
                        animator.SetBool("isWalking", false);
                        animator.SetBool("isRunning", false);
                    }
                }
                else
                {
                    animator.SetBool("isIdling", true);
                }
            }
        }
        else
        {
            if (!hasJumped)
            {
                animator.SetTrigger("doJump");
                animator.SetFloat("WalkSpeed", 1.2f);

                animator.SetBool("isJumping", true);
                animator.SetBool("isRunning", true);

                animator.SetBool("isIdling", false);
                animator.SetBool("isWalking", false);

                hasJumped = true;
            }

            idleTimer = 0;
        }
    }
}
