using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = System.Random;

public class LoginUI : MonoBehaviour
{
    public Text usernameText;
    public Text passwordText;

    public GameObject loadingText;
    public GameObject inputPanel;


    private const string emailSufix = "@asg.com";
    
    void Start()
    {
        UserManager.Instance.Clean();
        TryAutomaticLogic();
    }

    private async Task CreateFakeUsers()
    {
        List<string> users = new List<string>()
        {
            "Actorgael",
            "BizarreMedium",
            "Copebr",
            "Dominte",
            "Farrenbe",
            "FraserFootball",
            "Giblup",
            "GodzillaSra",
            "GrabsStar",
            "Griffonic",
            "Guantoci",
            "InspiringIce",
            "Jeanac",
            "Kinost",
            "LifeLegend",
            "LiveStronger",
            "MaidKrypto",
            "MisterHunter",
            "Murphydr",
            "Neareg",
            "Poddapp",
            "Remoldpa",
            "Sarahanch",
            "ShadesGold",
            "Stablacco",
            "Staceyma",
            "Trickfi",
            "Xpotri",
            "ZinPrecise"
        };

        await InitializeAuth();
        
        Random random = new Random();
        foreach (var user in users)
        {
            if(user.Length < 6 || user.Contains(" "))
                continue;
            
            var result = await AuthManager.Register(user + emailSufix, user, user);
            
            if (!result.valid)
            {
                Debug.LogError("Error: " + result.error);
                return;
            }
            await AuthManager.WriteToDb("maxScore", random.Next(5, 10000));
            //uthManager.Logout();
            
            Debug.Log("FINISHED " + user);
            await Task.Delay(100);
        }
        
        
    }

    private async void TryAutomaticLogic()
    {
        inputPanel.SetActive(false);
        loadingText.SetActive(true);
        
        if (UserManager.Instance.GetPlayerPrefs(out string email, out string password))
        {
            if(await Login(email, password))
                return;
        }
        
        inputPanel.SetActive(true);
        loadingText.SetActive(false);
    }

    private async Task InitializeAuth()
    {
        var result = await AuthManager.Initialize();
        if (!result.valid)
        {
            Debug.LogError(result.error);
        }
    }

    public void LoginButton()
    {
        if (!ValidateFields())
            return;
        
        string email = usernameText.text + emailSufix;
        string password = passwordText.text;
        
        Login(email, password);
    }

    private async Task<bool> Login(string email, string password)
    {
        await InitializeAuth();

        var result = await AuthManager.Login(email, password);

        if (!result.valid)
        {
            PopupUI.Instance.ShowPopUp("Error", result.error, "Ok");
            return false;
        }

        UserManager.Instance.SavePlayerPrefs(email, password);
        EnterGame();
        return true;
    }

    public void RegisterButton()
    {
        if (!ValidateFields())
            return;
        
        string username = usernameText.text;
        string email = usernameText.text + emailSufix;
        string password = passwordText.text;
        
        Register(username, email, password);
    }

    public async Task Register(string username, string email, string password)
    {
        await InitializeAuth();
        
        var result = await AuthManager.Register(email, password, username);
        
        if (!result.valid)
        {
            PopupUI.Instance.ShowPopUp("Error", result.error, "Ok");
            return;
        }
        
        UserManager.Instance.SavePlayerPrefs(email, password);
        EnterGame();
    }

    private async void EnterGame()
    {
        await UserManager.Instance.Initialize();
        
        if (GameData.Instance.JoinWithDirectCode)
            SceneManager.LoadScene("Game");
        else
            SceneManager.LoadScene("MainMenu");
    }

    private bool ValidateFields()
    {
        if (usernameText.text.Length == 0)
        {
            PopupUI.Instance.ShowPopUp("Error", "Username is empty!", "Ok");
            return false;
        }

        if (usernameText.text.Length < 6)
        {
            PopupUI.Instance.ShowPopUp("Error", "Username is too short! Needs to have at least 6 characters.", "Ok");
            return false;
        }
        
        if (usernameText.text.Contains(" "))
        {
            PopupUI.Instance.ShowPopUp("Error", "Username cannot contain spaces!", "Ok");
            return false;
        }

        if (passwordText.text.Length == 0)
        {
            PopupUI.Instance.ShowPopUp("Error", "Password is empty!", "Ok");
            return false;
        }

        if(passwordText.text.Length < 6)
        {
            PopupUI.Instance.ShowPopUp("Error", "Password is too short! Needs to have at least 6 characters.", "Ok");
            return false;
        }
        
        if (passwordText.text.Contains(" "))
        {
            PopupUI.Instance.ShowPopUp("Error", "Password cannot contain spaces!", "Ok");
            return false;
        }

        return true;
    }
}
