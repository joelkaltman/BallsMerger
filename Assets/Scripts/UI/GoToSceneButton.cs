using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GoToSceneButton : MonoBehaviour
{
    public string sceneName;
    public bool disconnectMultiplayer;
    
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(GoToScene);
    }

    void GoToScene()
    {
        if(disconnectMultiplayer && MultiplayerManager.Instance)
            MultiplayerManager.Instance.Disconnect();
            
        SceneManager.LoadScene(sceneName);
    }
}
