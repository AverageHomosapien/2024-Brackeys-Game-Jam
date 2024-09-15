using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerScript : MonoBehaviour
{
    public static event Action<string> OnSceneSwitch;

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
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void LoadMenuScene() 
    {
        SceneManager.LoadScene("MenuScene");
        OnSceneSwitch?.Invoke(SceneManager.GetActiveScene().name);
    }

    public void NextScene() 
    {    
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        OnSceneSwitch?.Invoke(SceneManager.GetActiveScene().name);
    }

    public void NextScene(string sceneName) 
    {
        SceneManager.LoadScene(sceneName);
        OnSceneSwitch?.Invoke(SceneManager.GetActiveScene().name);
    }

    public void ReloadScene() 
    {
       SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Quit()
    {
    #if UNITY_STANDALONE
        Application.Quit();
    #endif
    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    #endif
    }

}
