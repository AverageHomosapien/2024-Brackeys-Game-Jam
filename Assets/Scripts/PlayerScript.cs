using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        SceneManagerScript.OnSceneSwitch += OnSceneSwitchTriggered;
        DontDestroyOnLoad(gameObject);
    }

    private void OnSceneSwitchTriggered(string obj)
    {
        if (obj == "Tornado Lift Scene")
        {
            gameObject.GetComponent<Rigidbody>().isKinematic = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
