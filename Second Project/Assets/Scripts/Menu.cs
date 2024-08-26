using GwentPlus;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public void OnStartButtonClicked()
    {
        SceneManager.LoadScene(2);
    }

    public void OnCreateButtonClicked()
    {
        SceneManager.LoadScene(1);
    }

    public void OnExitButtonClicked()
    {
        Application.Quit();
    }

    public void OnBackMenuButtonClicked()
    {
        SceneManager.LoadScene(0); 

    }
}
