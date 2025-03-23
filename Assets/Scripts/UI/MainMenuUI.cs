using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

public class MainMenuUI : MonoBehaviour {

	enum PanelType
	{
		MAINMENU,
		PAUSEMENU,
		GAME,
		GAMEOVER,
		OPTIONS,
		RANKING
	};

	public GameObject panelMainMenu;
	public GameObject panelOptions;
	
	public Button buttonSinglePlayer;
	public Button buttonMultiPlayer;
	public Button buttonRanking;
	public Button buttonSound;
	public Button buttonLogOut;
	
	public Text textUsername;
	public Sprite soundOn;
	public Sprite soundOff;
	
	private PanelType currentPanel;
	private PanelType lastPanel;

	void Awake(){
		currentPanel = 0;
		lastPanel = 0;
	}

	// Use this for initialization
	void Start () 
	{
		Time.timeScale = 1;
        
        ShowCanvas (PanelType.MAINMENU);
        
        OnUserInitialized();
        
        buttonSinglePlayer.onClick.AddListener(StartSinglePlayer);
        buttonMultiPlayer.onClick.AddListener(StartMultiplayer);
        buttonRanking.onClick.AddListener(GoToRanking);
        buttonSound.onClick.AddListener(MuteGame);
        buttonLogOut.onClick.AddListener(LogOut);
    }
    
    private void OnUserInitialized()
    {
        textUsername.text = UserManager.Instance.UserData.username;
        buttonSinglePlayer.interactable = true;
        buttonMultiPlayer.interactable = true;
    }
    
	private void ShowCanvas(PanelType type)
	{
		switch (type) {
		case PanelType.MAINMENU:
			panelMainMenu.SetActive (true);
			panelOptions.SetActive (false);
			break;
		case PanelType.PAUSEMENU:
			panelMainMenu.SetActive (false);
			panelOptions.SetActive (false);
			break;
		case PanelType.GAME:
			panelMainMenu.SetActive (false);
			panelOptions.SetActive (false);
			break;
		case PanelType.GAMEOVER:
			panelMainMenu.SetActive (false);
			panelOptions.SetActive (false);
			break;
		case PanelType.OPTIONS:
			panelMainMenu.SetActive (false);
			panelOptions.SetActive (true);
			break;
		case PanelType.RANKING:
			panelMainMenu.SetActive (false);
			panelOptions.SetActive (false);
			break;
		}

		lastPanel = currentPanel;
		currentPanel = type;
	}

	public void StartSinglePlayer()
	{
		GameData.Instance.isOnline = false;
		SceneManager.LoadScene("Game");
	}

	public void StartMultiplayer()
	{
		GameData.Instance.isOnline = true;
		SceneManager.LoadScene("Game");
	}
	
	public void LogOut()
	{
		UserManager.Instance.SavePlayerPrefs("", "");
		AuthManager.Logout();
		SceneManager.LoadScene("Login");
	}
		
	public void GoToMainMenu()
	{
		Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
	}

	public void GoToOptions()
	{
        ShowCanvas (PanelType.OPTIONS);
	}

	public void GoToRanking()
    {
        SceneManager.LoadScene("Ranking");
    }
	
	public void GoToLastPanel()
	{
        ShowCanvas (lastPanel);
	}

	public void MuteGame()
	{
		bool isMute = MusicManager.Instance.Mute();
		buttonSound.gameObject.GetComponent<Image>().sprite = isMute ? soundOff : soundOn;
	}
}