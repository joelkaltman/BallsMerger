using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MultiplayerUI : MonoBehaviour
{
    [Header("Host")] 
    public GameObject hostPanel;
    public Button hostButton;
    
    [Header("Client")] 
    public GameObject clientPanel;
    public InputField inputJoinCodeIn;
    public Text textJoinCodeIn;
    public Button joinButton;

    [Header("Other")] 
    public Text middleText;
    public GameObject loadingIcon;

    public Action<string> OnHostStarted;
    public Action OnClientStarted;

    public void Start()
    {
        DisableUI("Booting services...");
        hostButton.enabled = false;
        joinButton.enabled = false;
        
        inputJoinCodeIn.onValueChanged.AddListener((text) =>
        {
            inputJoinCodeIn.text = text.ToUpper(); 
        });

        MultiplayerManager.Instance.InitializeMultiplayer();

        if (MultiplayerManager.Instance.ServicesReady())
        {
            OnServicesReady(true);
        }
        else
        {
            MultiplayerManager.Instance.OnServicesBooted -= OnServicesReady;
            MultiplayerManager.Instance.OnServicesBooted += OnServicesReady;
        }
    }

    private void OnServicesReady(bool ready)
    {
        if (!ready)
        {
            PopupUI.Instance.ShowPopUp("Error", "Error booting multiplayer services.", 
                "Ok", null, 
                "Retry", () => MultiplayerManager.Instance.InitializeMultiplayer());
            return;
        }
        
        ResetUI();

        if (GameData.Instance.JoinWithDirectCode)
        {
            JoinClient();
        }
    }

    public async void StartHost()
    {
        DisableUI("Creating match...");
        var result = await MultiplayerManager.Instance.StartHost();

        if (!result.Result)
        {
            Debug.LogError(result.Error);
            
            PopupUI.Instance.ShowPopUp("Error", result.Error, "Close");
            ResetUI();
            return;
        }
        
        OnHostStarted?.Invoke(result.JoinCode);
    }

    public async void JoinClient()
    {
        DisableUI("Joining match...");

        string joinCode = textJoinCodeIn.text;
        
        if (GameData.Instance.JoinWithDirectCode)
        {
            joinCode = GameData.Instance.directJoinCode;
            GameData.Instance.directJoinCode = "";
        }
        
        var result = await MultiplayerManager.Instance.JoinClient(joinCode);

        if (!result.Result)
        {
            Debug.LogError(result.Error);
            
            PopupUI.Instance.ShowPopUp("Error", result.Error, "Close");
            ResetUI();
            return;
        }
        
        OnClientStarted?.Invoke();
    }

    private void DisableUI(string displayText)
    {
        hostPanel.SetActive(false);
        clientPanel.SetActive(false);
        hostButton.enabled = false;
        joinButton.enabled = false;
        loadingIcon.SetActive(true);
        
        middleText.text = displayText;
    }
    
    private void ResetUI()
    {
        hostPanel.SetActive(true);
        clientPanel.SetActive(true);
        hostButton.enabled = true;
        joinButton.enabled = true;
        loadingIcon.SetActive(false);
        
        middleText.text = "Or...";
    }
}
