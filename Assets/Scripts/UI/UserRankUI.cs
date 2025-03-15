using UnityEngine;
using UnityEngine.UI;

public class UserRankUI : MonoBehaviour
{
    public Text posText;
    public Text usernameText;
    public Text killsText;

    public void SetUser(int position, AuthManager.UserRank userRank)
    {
        posText.text = position < 0 ? "-" : $"#{position}";
        usernameText.text = userRank.username;
        killsText.text = userRank.maxKills.ToString();
    }
}
