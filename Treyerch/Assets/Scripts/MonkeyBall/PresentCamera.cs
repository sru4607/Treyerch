using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PresentCamera : MonoBehaviour
{
    public GameObject player;
    public Camera presentCamera;

    public void EnablePlayer()
    {
        presentCamera.gameObject.SetActive(false);
        player.gameObject.SetActive(true);
        gameObject.SetActive(false);
    }
}
