using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Invector.vCharacterController;

public class PlayerAnimator : MonoBehaviour
{
    public Animator animator;
    public RagdollController ragdollController;
    public vThirdPersonController thirdPersonController;

    public float movementVelocityCutoff = 0.1f;
    public float runSpeed = 7f;
    public float idleTimerMovementCutoff = 2f;
    public float idleTimerMax = 5f;

    private float idleTimer;
    private bool hasJumped;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!ragdollController.isRagdoll)
        {
            if (thirdPersonController.isGrounded)
            {
                if (hasJumped)
                {
                    hasJumped = false;
                    animator.SetBool("isJumping", false);
                    animator.SetBool("isRunning", false);
                }

                Vector3 walkingVelocity = new Vector3(ragdollController.playerRigidbody.velocity.x, 0, ragdollController.playerRigidbody.velocity.z);

                if (walkingVelocity.magnitude > movementVelocityCutoff)
                {
                    animator.SetBool("isIdling", false);
                    idleTimer = 0;

                    animator.SetFloat("WalkSpeed", walkingVelocity.magnitude);
                    if (walkingVelocity.magnitude >= runSpeed)
                    {
                        animator.SetBool("isRunning", true);
                        animator.SetBool("isWalking", false);
                    }
                    else
                    {
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
                animator.SetFloat("WalkSpeed", 1.2f);
                if (animator.GetBool("isRunning") == false)
                {     
                    animator.SetBool("isJumping", true);
                    animator.SetBool("isIdling", false);
                    idleTimer = 0;
                    animator.SetBool("isRunning", true);
                    animator.SetBool("isWalking", false);

                    hasJumped = true;
                }
                else
                {
                    animator.SetBool("isJumping", false);
                }            
            }
        }
        else
        {
            animator.SetBool("isWalking", false);
            animator.SetBool("isRunning", false);
        }
    }
}
