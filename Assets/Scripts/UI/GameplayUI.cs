using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameplayUI : MonoBehaviour {

	enum PanelType
	{
		MAINMENU,
		PAUSEMENU,
		GAME,
		GAMEOVER,
		OPTIONS,
		RANKING,
		MULTIPAYER
	};

	public BallsSpawner spawner;
	
	[Header("Panels")] 
	public GameObject panelPauseMenu;
	public GameObject panelGame;
	public GameObject panelGameOver;
	public GameObject panelOptions;
	public GameObject panelMultiplayer;

	[Header("UI")] 
	public GameObject game;
    public GameObject topPanel;
    public GameObject bottomPanel;
    
	public Button attackButton;
    public Text playerName;
    public Text textScore;
    public Text textTime;
	public Text textGameOverReason;
	public Text textGiantScore;
	public Text textJoinCode;
	public List<Image> buttonsSound;
	public Sprite soundOn;
	public Sprite soundOff;
	
	[Header("RemotePlayer")]
	public GameObject remotePlayerPanel;
	public Text textRemoteUsername;
	public Text textRemoteScore;

	[Header("GameOver")] 
	public GameObject newHighScoreText;
	
	private PlayerStats playerStats;
	private PlayerStats remotePlayerStats;
	
	private float objetiveFade;
	private float currentFade;
	private float speedFade;
	private bool usedContinue;

	private PanelType currentPanel;
	private PanelType lastPanel;

	private NetworkManager netManager;

	private string RemoteUsername => remotePlayerStats.Username.Value.ToString() ?? "Your partner";

	void Awake()
	{
		objetiveFade = 0;
		currentFade = 0;
		speedFade = 0.01f;
		usedContinue = false;
		currentPanel = 0;
		lastPanel = 0;
	}

	// Use this for initialization
	void Start () 
	{
		MultiplayerManager.Instance.OnLocalPlayerReady += OnLocalPlayerReady;
		MultiplayerManager.Instance.OnRemotePlayerReady += OnRemotePlayerReady;
		MultiplayerManager.Instance.OnGameReady += StartGame;
		MultiplayerManager.Instance.OnGameOver += GameOver;
		
		if (!GameData.Instance.isOnline)
		{
			ShowCanvas(PanelType.GAME);
			MultiplayerManager.Instance.InitializeSinglePlayer();
		}
		else
		{
			ShowCanvas(PanelType.MULTIPAYER);
			var mpUI = panelMultiplayer.GetComponent<MultiplayerUI>();
			mpUI.OnHostStarted += OnHostStarted;
		}
		
		game.SetActive(false);
		remotePlayerPanel.SetActive(false);
		attackButton.gameObject.SetActive(false);
	}

	void Update ()
	{
		if (!MultiplayerManager.Instance.IsGameReady)
			return;
		
		RefreshTimer();
	}

	private void OnHostStarted(string code)
	{
		//ShowCanvas(PanelType.GAME);
		topPanel.SetActive(false);
		bottomPanel.SetActive(false);
		textJoinCode.gameObject.SetActive(true);
		textJoinCode.text = code;
	}
	
	public void ShareCode()
	{
		var joinCode = textJoinCode.text;
		var username = UserManager.Instance.UserData.username;
		
		new NativeShare()
			.SetSubject("Balls Merger")
			.SetText($"{username} has challenged you to a duel!")
			.SetUrl($"https://ballsmerger.web.app?code={joinCode}")
			.Share();
	}
	
	private void OnLocalPlayerReady(GameObject player)
	{
		playerStats = player.GetComponent<PlayerStats>();
		playerStats.Score.OnValueChanged += RefreshScore;
		playerName.text = UserManager.Instance.UserData.username;

		if (GameData.Instance.isOnline)
		{
			playerStats.OnAttackReady += () => { attackButton.gameObject.SetActive(true); };
			attackButton.onClick.AddListener(LocalPlayerAttack);
		}
	}

	private void LocalPlayerAttack()
	{
		playerStats.AttackRpc();
		attackButton.gameObject.SetActive(false);
	}

	private void OnRemotePlayerReady(GameObject player)
	{
		remotePlayerPanel.SetActive(true);
		remotePlayerStats = player.GetComponent<PlayerStats>();
		SetUsername(RemoteUsername, RemoteUsername);
		remotePlayerStats.Username.OnValueChanged += SetUsername;
		remotePlayerStats.Score.OnValueChanged += RefreshRemoteScore;
	}

	private void SetUsername(FixedString64Bytes old, FixedString64Bytes username)
	{
		textRemoteUsername.text = RemoteUsername;
	}
	
	private void ShowCanvas(PanelType type)
	{
		switch (type) {
		    case PanelType.PAUSEMENU:
			    panelPauseMenu.SetActive (true);
			    panelGame.SetActive (false);
                panelGameOver.SetActive (false);
                panelOptions.SetActive (false);
			    panelMultiplayer.SetActive(false);
			    break;
		    case PanelType.GAME:
			    panelPauseMenu.SetActive (false);
			    panelGame.SetActive (true);
			    panelGameOver.SetActive (false);
                panelOptions.SetActive (false);
			    panelMultiplayer.SetActive(false);
			    break;
            case PanelType.GAMEOVER:
                panelPauseMenu.SetActive (false);
                panelGame.SetActive (false);
                panelGameOver.SetActive (true);
                panelOptions.SetActive (false);
                panelMultiplayer.SetActive(false);
                break;
		    case PanelType.OPTIONS:
			    panelPauseMenu.SetActive (false);
			    panelGame.SetActive (false);
			    panelGameOver.SetActive (false);
			    panelOptions.SetActive (true);
			    panelMultiplayer.SetActive(false);
			    break;
		    case PanelType.MULTIPAYER:
			    panelPauseMenu.SetActive (false);
			    panelGame.SetActive (false);
			    panelGameOver.SetActive (false);
			    panelOptions.SetActive (false);
			    panelMultiplayer.SetActive(true);
			    break;
		}

		lastPanel = currentPanel;
		currentPanel = type;
	}

	private void StartGame()
	{
		ShowCanvas(PanelType.GAME);
		topPanel.SetActive(true);
		bottomPanel.SetActive(true);
		game.SetActive(true);
		spawner.enabled = true;
		textJoinCode.gameObject.SetActive(false);
	}

	public void PauseGame()
	{
		if(!GameData.Instance.isOnline)
			Time.timeScale = 0;

		ShowCanvas (PanelType.PAUSEMENU);
	}

	public void ResumeGame()
	{
		Time.timeScale = 1;
        ShowCanvas (PanelType.GAME);
	}

	public void GoToLastPanel()
	{
        ShowCanvas (lastPanel);
	}
    
    public void GoToOptions()
    {
        ShowCanvas (PanelType.OPTIONS);
    }

	public void MuteGame(){
		bool isMute = MusicManager.Instance.Mute ();
		for (int i = 0; i < buttonsSound.Count; i++) {
			if (isMute) {
				buttonsSound [i].sprite = soundOff;
			} else {
				buttonsSound [i].sprite = soundOn;
			}
		}
	}

	public void Continue()
	{
		#if UNITY_ANDROID
		// Watch add
		if (Advertisement.isInitialized)
		{
		}
		#endif
	}

	#if UNITY_ANDROID
	private void HandleShowResult(ShowResult result)
	{
		switch (result)
		{
		case ShowResult.Finished:
			usedContinue = true;
            ShowCanvas (PanelType.GAME);
			break;
		case ShowResult.Skipped:
			break;
		case ShowResult.Failed:
			break;
		}
	}
	#endif

	// ================================= Game Interface ==============================
	
	private void RefreshScore(int previousScore, int score)
	{
		textScore.text = score.ToString();
	}
	
	private void RefreshRemoteScore(int previousScore, int score)
	{
		textRemoteScore.text = score.ToString();
	}

	void RefreshTimer()
	{
		if (!GameData.Instance.isOnline)
		{
			textTime.transform.parent.gameObject.SetActive(false);
			return;
		}

		if (MultiplayerManager.Instance.Timer == null)
		{
			Debug.LogError("Timer not initialized for multiplayer.");
			return;
		}
		
		var min = MultiplayerManager.Instance.Timer.Minutes.Value;
		var sec = MultiplayerManager.Instance.Timer.Seconds.Value;
		
		string strMin = min.ToString ();
		string strSec = sec.ToString ();
		if(min < 10) {
			strMin = "0" + min;
		}
		if(sec < 10) {
			strSec = "0" + sec;
		}
		
		this.textTime.text = strMin + ":" + strSec;
	}

	private void AttackRemotePlayer()
	{
		
	}

	private void GameOver(MultiplayerManager.GameOverReason reason)
	{
        ShowCanvas (PanelType.GAMEOVER);
        
        switch (reason)
        {
	        case MultiplayerManager.GameOverReason.Disconnected:
		        textGameOverReason.text = "You got disconnected";
		        break;
	        case MultiplayerManager.GameOverReason.LocalPlayerLost:
		        textGameOverReason.text = "You LOST!";
		        break;
	        case MultiplayerManager.GameOverReason.RemotePlayerLost:
		        textGameOverReason.text = $"{RemoteUsername} LOST!";
		        break;
	        case MultiplayerManager.GameOverReason.TimeFinished:
		        var winner = remotePlayerStats != null && remotePlayerStats.Score.Value > playerStats.Score.Value ? $"{RemoteUsername}" : "You";
		        textGameOverReason.text = $"Time has run out! {winner} WON!";
		        break;
        }
        
		textGiantScore.text = "Your score is " + UserManager.Instance.Score + "!";


		bool newMaxScore = UserManager.Instance.CheckNewHighScore();
		newHighScoreText.SetActive(newMaxScore);
	}

}
