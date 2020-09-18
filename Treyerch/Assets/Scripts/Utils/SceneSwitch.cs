using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitch : MonoBehaviour
{
    public string sceneName;
    public float switchDelay;

    public void SwitchScene()
    {
        Invoke("DoSwitch", switchDelay);
    }

    private void DoSwitch()
    {
        SceneManager.LoadScene(sceneName);
    }
}
