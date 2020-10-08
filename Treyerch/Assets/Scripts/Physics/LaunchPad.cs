using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class LaunchPad : MonoBehaviour
{
    [Header("Settings")]
    public Vector3 launchAngle = new Vector3(0, 0, 1);
    public float launchPower = 80;
    public float launcherResetTimer = 3f;

    [Header("Tweening")]
    public SequentialTweener sequentialTweener;
    [ShowIf("@sequentialTweener != null")]
    public List<int> activeTweens;

    private bool hasLaunched;
    private bool isActive;

    private void Start()
    {
        if(sequentialTweener != null)
        {
            if (activeTweens != null && activeTweens.Count > 0)
            {
                isActive = false;
            }
            else
            {
                isActive = true;
            }
        }
        else
        {
            isActive = true;
        }
    }

    private void Update()
    {
        if (sequentialTweener != null)
        {
            if (activeTweens != null && activeTweens.Count > 0)
            {
                if (sequentialTweener.isActive)
                {
                    if (activeTweens.Contains(sequentialTweener.sequenceIndex))
                    {
                        isActive = true;
                    }
                    else
                    {
                        isActive = false;
                    }
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + ((transform.forward + launchAngle).normalized * launchPower)/15);
    }

    private void OnTriggerEnter(Collider col)
    {
        if(isActive)
        {
            if (col.gameObject.layer == 10) //Player Ragdoll
            {
                if (!hasLaunched)
                {
                    RagdollController playerRagDoll = col.transform.root.GetComponentInChildren<RagdollController>();
                    LaunchRagdoll(playerRagDoll);
                }
            }
        }
    }

    private void LaunchRagdoll(RagdollController playerRagDoll)
    {
        if (playerRagDoll != null)
        {
            hasLaunched = true;
            if (!playerRagDoll.isRagdoll)
            {
                playerRagDoll.TurnOnRagdoll();
            }

            playerRagDoll.ragdollChest.velocity = (transform.forward + launchAngle).normalized * launchPower;

            Invoke("ResetLauncher", launcherResetTimer);
        }
    }

    private void ResetLauncher()
    {
        hasLaunched = false;
    }
}
