using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TeleportTrigger : MonoBehaviour
{
    [Header("Settings")]
    public Transform spawnPoint;
    public float respawnDelay;
    [Header("Options")]
    public bool freezePlayerOnContact = true;
    public bool isActiveOnStart = true;
    [Header("Events")]
    public bool triggerEventOnce = false;
    public UnityEvent OnActivate;

    private bool isActive;
    private bool hasTriggered = false;

    private void Start()
    {
        if(isActiveOnStart)
        {
            isActive = true;
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        if (isActive)
        {
            if (col.gameObject.layer == 9) //Player
            {
                RagdollController playerRagDoll = col.transform.root.GetComponentInChildren<RagdollController>();
                
                if(playerRagDoll)
                {
                    isActive = false;

                    if(freezePlayerOnContact)
                    {
                        playerRagDoll.playerRigidbody.isKinematic = true;
                    }

                    StartCoroutine(ResetPlayer(playerRagDoll));
                }
            }
            else if (col.gameObject.layer == 10) //Player ragdoll
            {
                RagdollController playerRagDoll = col.transform.root.GetComponentInChildren<RagdollController>();

                if (playerRagDoll)
                {
                    isActive = false;

                    if (freezePlayerOnContact)
                    {
                        playerRagDoll.ragdollChest.isKinematic = true;
                    }

                    StartCoroutine(ResetPlayer(playerRagDoll));
                }
            }
        }
    }

    private IEnumerator ResetPlayer(RagdollController playerRagDoll)
    {
        yield return new WaitForSeconds(respawnDelay);
        playerRagDoll.TeleportTo(spawnPoint);

        if ((triggerEventOnce && !hasTriggered) || !triggerEventOnce)
        {
            hasTriggered = true;
            if (OnActivate != null)
            {
                OnActivate.Invoke();
            }
        }
        Invoke("MakeActive", 1f);
    }

    public void MakeActive()
    {
        ToggleActive(true);
    }

    public void ToggleActive(bool toggle)
    {
        isActive = toggle;
    }
}
