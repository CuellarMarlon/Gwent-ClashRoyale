using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoaderEnd : MonoBehaviour
{
    public float delayTime = 5f; // Tiempo de espera antes de cambiar de escena

    void Start()
    {
        StartCoroutine(LoadNextSceneAfterDelay());
    }

    IEnumerator LoadNextSceneAfterDelay()
    {
        yield return new WaitForSeconds(delayTime);

        SceneManager.LoadScene(0);
    }
}