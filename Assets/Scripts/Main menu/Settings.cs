using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    public AudioMixer audioMixer;
    public Dropdown resDropDown;
    Resolution[] resolutions;
    bool alreadyAdded = false;


    private void Start()
    {
        resolutions = Screen.resolutions;

        resDropDown.ClearOptions();

        List<string> options = new List<string>();

        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            
            alreadyAdded = false;
            options.Add(option);
            

            if (resolutions[i].width == Screen.width
                    && resolutions[i].height == Screen.height)
            {
                currentResolutionIndex = i;
            }

        }
        Screen.SetResolution(1920, 1080, Screen.fullScreen);
        SetFullSreen(true);
        resDropDown.AddOptions(options);
        resDropDown.value = currentResolutionIndex;
        resDropDown.RefreshShownValue();
        
    }


    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("volume", volume);
    }

    public void SetSfxVolume(float sfxVolume)
    {
        audioMixer.SetFloat("sfx", sfxVolume);
    }

    public void SetMusicVolume(float musicVolume)
    {
        audioMixer.SetFloat("music", musicVolume);
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void SetFullSreen(bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;
    }

    public void SetMouseSense(float mouseSense)
    {
        MouseLook.mouseSensitivity = mouseSense;
    }
}
