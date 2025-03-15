using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeepLinkManager : MonoBehaviour
{
    public static DeepLinkManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;                
            Application.deepLinkActivated += OnDeepLinkActivated;
            if (!string.IsNullOrEmpty(Application.absoluteURL))
            {
                OnDeepLinkActivated(Application.absoluteURL);
            }
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDeepLinkActivated(string code)
    {
        GameData.Instance.isOnline = true;
        GameData.Instance.directJoinCode = code.Split('?')[1];
        
        if (UserManager.Instance.Initialized)
        {
            SceneManager.LoadScene("Game");
        }
        else
        {
            PopupUI.Instance.ShowPopUp("Login needed", "You will be automatically redirected to the match after login in.", "Ok");
        }
    }
}
