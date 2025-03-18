using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class UserManager
{
    private static UserManager userManager;
    public static UserManager Instance => userManager ??= new UserManager();
    
    public AuthManager.UserData UserData { get; private set; }

    private UserManager()
    {
        UserData = new AuthManager.UserData()
        {
            username = "FAKE_USER",
        };
    }
    
    public int Kills { get; private set; }
    public bool Initialized { get; private set; }
    public bool AimingAutomatic { get; private set; }

    public event Action OnInitialize;
    public event Action OnCapCountChange;

    public async Task Initialize()
    {
        UserData = await AuthManager.GetUserData();
        AimingAutomatic = GetAutoAimingPrefs();
        Initialized = true;
        OnInitialize?.Invoke();
    }
    
    public void Clean()
    {
        UserData = null;
        Initialized = false;
    }
    
    public async void SaveUserData()
    {
        await AuthManager.SaveUserData(UserData);
    }

    public async void SaveKills()
    {
        await AuthManager.WriteToDb("kills", UserData.maxKills);
    }
	
    public async void SaveCaps()
    {
        await AuthManager.WriteToDb("caps", UserData.caps);
    }
	
    public async void SaveGuns()
    {
        await AuthManager.WriteToDb("guns", UserData.guns);
    }
    
    public void AddCap()
    {
        UserData.caps++;
        SaveCaps();
        
        OnCapCountChange?.Invoke ();
    }
    
    public void SetKills(int score)
    {
        if (score < Kills)
            return;
        
        Kills = score;
    }

    public void ResetKills()
    {
        Kills = 0;
    }

    public bool CheckNewHighScore()
    {
        bool newHighScore = Kills > UserData.maxKills;
        if (newHighScore)
        {
            UserData.maxKills = Kills;
            SaveKills();
        }

        return newHighScore;
    }

    public bool GetPlayerPrefs(out string email, out string password)
    {
        if (!PlayerPrefs.HasKey("email") || !PlayerPrefs.HasKey("password"))
        {
            email = null;
            password = null;
            return false;
        }

        email = PlayerPrefs.GetString("email");
        password = PlayerPrefs.GetString("password");
        return !string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(password);
    }
    
    public void SavePlayerPrefs(string email, string password)
    {
        PlayerPrefs.SetString("email", email);
        PlayerPrefs.SetString("password", password);
    }

    public bool GetAutoAimingPrefs()
    {
        return PlayerPrefs.GetInt("autoaim", 1) != 0;
    }
    
    public void SaveAutoAimingPrefs(bool automatic)
    {
        AimingAutomatic = automatic;
        PlayerPrefs.SetInt("autoaim", Convert.ToInt32(automatic));
    }
}
