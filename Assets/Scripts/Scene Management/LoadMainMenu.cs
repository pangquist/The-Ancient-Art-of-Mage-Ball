using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadMainMenu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        LoadMenu();
    }

    void LoadMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
