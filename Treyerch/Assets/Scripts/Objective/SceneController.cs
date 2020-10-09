using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void NextScene()
    {
        Scene current = SceneManager.GetActiveScene();
        int toLoad = current.buildIndex + 1; 
        if(toLoad >= SceneManager.sceneCountInBuildSettings){
            LoadMenu();
        }
        else
        {
            SceneManager.LoadScene(toLoad);
        }
    }

    public void PreviousScene(){
        Scene current = SceneManager.GetActiveScene();
        int toLoad = current.buildIndex - 1; 
        if(toLoad <= 0){
            LoadMenu();
        }
        else
        {
            SceneManager.LoadScene(toLoad);
        }
    }

    public void LoadMenu(){
        SceneManager.LoadScene(0);
    }
}
