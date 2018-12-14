using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; //allows loading of scenes

public class SceneController : MonoBehaviour {

    public void LoadSceneDelayed(string sceneName) //function for loading scenes with a delay
    { 
        StartCoroutine("Load", sceneName); //Calls coroutine
    }

    IEnumerator Load(string sceneName) //coroutine for loading a scene
    { 
        yield return new WaitForSeconds(0.3f); //waits for x seconds
        SceneManager.LoadScene(sceneName); //Loads the assigned scene when this function is run
    }

    public void QuitDelayed() //function quit quitting with delay
    { 
        Invoke("Quit", 0.3f); //calls function with x seconds delay
    }

    public void Quit() //function for quitting the game when Quit button is clicked
    { 
        print("Quitting game"); //"Displays "Quitting game" to console
        Application.Quit(); //Quits the application (Built game only, doesn't work within Unity editor)
    }
}