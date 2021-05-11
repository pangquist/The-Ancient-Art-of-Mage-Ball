using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using System.Linq;

public class Settings : MonoBehaviour
{
    public AudioMixer audioMixer;
    public Dropdown resDropDown;
    Resolution[] resolutions;

    private void Start()
    {
        int currentRefreshRate = Screen.currentResolution.refreshRate;
        resolutions = Screen.resolutions.Where(resolution => resolution.refreshRate == currentRefreshRate).ToArray();
        resDropDown.ClearOptions();
        List<string> options = new List<string>();
        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.width
                    && resolutions[i].height == Screen.height)
            {
                currentResolutionIndex = i;
            }
        }

        SetFullSreen(true);
        resDropDown.AddOptions(options);
        resDropDown.value = currentResolutionIndex;
        SetResolution(currentResolutionIndex);
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
