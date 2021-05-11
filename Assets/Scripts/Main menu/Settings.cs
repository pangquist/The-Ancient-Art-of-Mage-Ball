using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using System.Linq;

public class Settings : MonoBehaviour
{
    public AudioMixer audioMixer;
    [SerializeField] Dropdown resDropDown;
    [SerializeField] Dropdown qualityDropDown;
    [SerializeField] Slider masterSlider;
    [SerializeField] Slider sfxSlider;
    [SerializeField] Slider musicSlider;
    [SerializeField] Slider senseSlider;
    [SerializeField] Toggle fullscreenToggler;

    Resolution[] resolutions;
    public static float sensePref;
    public static float sfxVolumePref;
    public static float masterVolumePref;
    public static float musicVolumePref;
    public static bool fullScreenPref;
    public static int resPref;
    public static int qualityPref;
    public static List<string> optionsPref;

    public void LoadSettings()
    {
        bool fullScreenBool = false;
        if (PlayerPrefs.HasKey("musicVolumePref"))
        {
            SetMusicVolume(PlayerPrefs.GetFloat("musicVolumePref"));
        }

        if (PlayerPrefs.HasKey("sfxVolumePref"))
        {
            SetSfxVolume(PlayerPrefs.GetFloat("sfxVolumePref"));
        }

        if (PlayerPrefs.HasKey("masterVolumePref"))
        {
            SetVolume(PlayerPrefs.GetFloat("masterVolumePref"));
        }

        if (PlayerPrefs.HasKey("sensePref"))
        {
            SetMouseSense(PlayerPrefs.GetFloat("sensePref"));
        }

        if (PlayerPrefs.HasKey("fullScreenPref"))
        {
            if (PlayerPrefs.GetInt("fullScreenPref") == 1)
            {
                fullScreenBool = true;
            }
            SetFullSreen(fullScreenBool);
        }

        else
        {
            SetFullSreen(true);
        }

        if (PlayerPrefs.HasKey("qualityPref"))
        {
            SetQuality(PlayerPrefs.GetInt("qualityPref"));
            qualityDropDown.RefreshShownValue();
        }

        if (PlayerPrefs.HasKey("optionsPref"))
        {
            resDropDown.AddOptions(optionsPref);
        }

        else
        {
            SetResolutionsValues();
        }

        if (PlayerPrefs.HasKey("resPref"))
        {
            SetResolution(PlayerPrefs.GetInt("resPref"));
            resDropDown.RefreshShownValue();
        }
    }

    public void SetResolutionsValues()
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

        resDropDown.AddOptions(options);
        optionsPref = options;
        resDropDown.value = currentResolutionIndex;
        SetResolution(currentResolutionIndex);
        resDropDown.RefreshShownValue();
    }

    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("volume", volume);
        masterSlider.value = volume;
        PlayerPrefs.SetFloat("masterVolumePref", volume);
        PlayerPrefs.Save();
    }

    public void SetSfxVolume(float sfxVolume)
    {
        audioMixer.SetFloat("sfx", sfxVolume);
        sfxSlider.value = sfxVolume;
        PlayerPrefs.SetFloat("sfxVolumePref", sfxVolume);
        PlayerPrefs.Save();
    }

    public void SetMusicVolume(float musicVolume)
    {
        audioMixer.SetFloat("music", musicVolume);
        musicSlider.value = musicVolume;
        PlayerPrefs.SetFloat("musicVolumePref", musicVolume);
        PlayerPrefs.Save();
    }

    public void SetQuality(int qualityIndex)
    {
        qualityDropDown.value = qualityIndex;
        QualitySettings.SetQualityLevel(qualityIndex);
        PlayerPrefs.SetInt("qualityPref", qualityIndex);
        PlayerPrefs.Save();
    }

    public void SetResolution(int resolutionIndex)
    {
        resDropDown.value = resolutionIndex;
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        PlayerPrefs.SetInt("resPref", resolutionIndex);
        PlayerPrefs.Save();
    }

    public void SetFullSreen(bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;
        fullscreenToggler.isOn = isFullScreen;
        PlayerPrefs.SetInt("fullScreenPref", isFullScreen ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void SetMouseSense(float mouseSense)
    {
        MouseLook.mouseSensitivity = mouseSense;
        senseSlider.value = mouseSense;
        PlayerPrefs.SetFloat("sensePref", mouseSense);
        PlayerPrefs.Save();
    }
}
