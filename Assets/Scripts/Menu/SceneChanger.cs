using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

// Elm Sellberg

public class SceneChanger : MonoBehaviour
{
    [SerializeField] GameObject settingsCanvas;
    [SerializeField] GameObject menuCanvas;

    public void PlayGame()
    {
        SceneManager.LoadSceneAsync(1);
    }

    public void SwapToSettings()
    {
        menuCanvas.SetActive(false);
        settingsCanvas.SetActive(true);
    }

    public void SwapToMainMenu()
    {
        Debug.Log("Method Reached.");
        settingsCanvas.SetActive(false);
        Debug.Log("Canvas Set False:");
        menuCanvas.SetActive(true);
        Debug.Log("Canvas Set True:");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
