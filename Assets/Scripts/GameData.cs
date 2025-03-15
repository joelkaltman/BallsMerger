using UnityEngine;

public class GameData : MonoBehaviour
{
    public static GameData Instance;
    
    public bool isOnline;
    public string directJoinCode;
    public bool JoinWithDirectCode => !string.IsNullOrEmpty(directJoinCode);
    
    public void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(this);
    }
}
