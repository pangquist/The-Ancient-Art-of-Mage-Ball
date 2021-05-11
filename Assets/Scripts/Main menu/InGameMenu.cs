using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameMenu : MonoBehaviour
{
    [SerializeField] GameObject pauseMenuUI;
    [SerializeField] GameObject settingsMenuUI;
    [SerializeField] CharacterController controller;
    public static bool gameIsPaused = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (gameIsPaused)
            {
                Resume();
            }

            else
            {
                Pause();
            }
        }

        if (gameIsPaused)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        settingsMenuUI.SetActive(false);
        gameIsPaused = false;
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        gameIsPaused = true;
    }

    public void QuitGame()
    {
        Debug.Log("Quitted Game");
        Application.Quit();
    }
}
