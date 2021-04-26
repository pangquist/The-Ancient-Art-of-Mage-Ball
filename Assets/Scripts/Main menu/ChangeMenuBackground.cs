using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeMenuBackground : MonoBehaviour
{
    MyNetworkManager networkManager;
    [SerializeField] Sprite[] backgrounds = new Sprite[0];
    [SerializeField] GameObject sceneSelectCanvas;

    private void Start()
    {
        SceneSelect.OnMenuBackgroundUpdated += ChangeBackground;
        networkManager = GameObject.Find("MenuNetworkManager").GetComponent<MyNetworkManager>();
    }

    private void OnDestroy()
    {
        SceneSelect.OnMenuBackgroundUpdated -= ChangeBackground;
    }

    public void ToggleSceneWindow(bool state)
    {
        List<MyNetworkPlayer> players = networkManager.Players;

        foreach (MyNetworkPlayer player in players)
        {
            if (player.isLocalPlayer && player.IsPartyOwner)
            {
                sceneSelectCanvas.SetActive(state);
            }
        }
    }
    
    public void ChangeBackground()
    {
        Debug.Log("Trying to change background!");
        for (int i = 0; i < backgrounds.Length; i++)
        {
            Debug.Log($"background name: {backgrounds[i].name} scene name: {networkManager.SelectedScene}");
            if(backgrounds[i].name == networkManager.SelectedScene)
            {
                gameObject.GetComponent<Image>().sprite = backgrounds[i];
            }
        }
    }
}
