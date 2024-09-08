using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SwitchSceneScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnMouseDown()
    {
        Console.WriteLine("Changing scene");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void LoadMenuScene() 
    {
        SceneManager.LoadScene("MenuScene");
    }

    public void NextScene() 
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void NextScene(string sceneName) 
    {
        SceneManager.LoadScene(sceneName);
    }

    public void ReloadScene() 
    {
       SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

}
