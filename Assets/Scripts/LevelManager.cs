using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public float autoLoadNextLevelAfter;

    void Start()
    {
        if (autoLoadNextLevelAfter <= 0)
        {
            Debug.Log("Level auto load disabled use a positive number in seconds");
        }
        else
        {
            Invoke("LoadNextLevel", autoLoadNextLevelAfter);
        }
    }

    public void LoadLevel(int index)
    {

        SceneManager.LoadScene(index);
    }

    public void QuitLevel()
    {
        Application.Quit();
    }

    public void LoadNextLevel()
    {

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

}
