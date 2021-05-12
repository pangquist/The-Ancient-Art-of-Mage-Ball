using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChangeMenuBackground : NetworkBehaviour
{
    MyNetworkManager networkManager;
    [SerializeField] Sprite[] backgrounds = new Sprite[0];
    [SerializeField] GameObject sceneSelectCanvas;
    [SerializeField] TMP_Text sceneNameText;
    [SyncVar(hook= nameof(HandleMapChange))]
    [SerializeField] string chosenMapName;

    public void SetMapName(string name)
    {
        chosenMapName = name;
    }

    private void Start()
    {
        SceneSelect.OnMenuBackgroundUpdated += ChangeBackground;
        networkManager = GameObject.Find("MenuNetworkManager").GetComponent<MyNetworkManager>();
    }

    private void OnDestroy()
    {
        SceneSelect.OnMenuBackgroundUpdated -= ChangeBackground;
    }
    
    void HandleMapChange(string oldValue, string newValue)
    {
        for (int i = 0; i < backgrounds.Length; i++)
        {
            if (backgrounds[i].name == chosenMapName)
            {
                gameObject.GetComponent<Image>().sprite = backgrounds[i];
                sceneNameText.text = backgrounds[i].name;
            }
        }
    }

    public void ToggleSceneWindow(bool state)
    {
        List<MyNetworkMenuPlayer> menuPlayers = networkManager.MenuPlayers;

        foreach (MyNetworkMenuPlayer player in menuPlayers)
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
