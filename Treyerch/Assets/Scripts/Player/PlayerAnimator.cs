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
    public float slapTimerMax = 1.5f;

    private float slapTimer;
    private float idleTimer;
    private bool hasJumped;
    private bool hasSlapped;

    private bool leftSlap;

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
                if (ragdollController.characterInput.isSlapping)
                {
                    idleTimer = 0;
                    slapTimer = 0;
                    if (!hasSlapped)
                    {
                        animator.SetBool("isJumping", false);
                        animator.SetBool("isIdling", false);
                        animator.SetBool("isRunning", false);
                        animator.SetBool("isWalking", false);

                        hasSlapped = true;

                        if (leftSlap)
                        {
                            animator.SetBool("leftSlap", true);
                            leftSlap = false;
                        }
                        else
                        {
                            animator.SetBool("leftSlap", false);
                            leftSlap = true;
                        }
                        animator.SetTrigger("doSlap");
                        ragdollController.playerRigidbody.isKinematic = true;
                        animator.SetBool("isSlapping", true);
                    }
                }
                else if (ragdollController.characterInput.finishedSlap == false)
                {
                    slapTimer += Time.deltaTime;

                    animator.SetBool("isSlapping", false);
                    ragdollController.playerRigidbody.velocity = Vector3.zero;
                    hasSlapped = false;

                    if (slapTimer >= slapTimerMax)
                    {
                        ragdollController.playerRigidbody.isKinematic = false;
                        ragdollController.characterInput.finishedSlap = true;
                        ragdollController.characterInput.cc.inputSmooth = Vector3.zero;
                        ragdollController.characterInput.cc.input = Vector3.zero;
                    }
                }

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
        else
        {
            animator.SetBool("isWalking", false);
            animator.SetBool("isRunning", false);
        }
    }
}
