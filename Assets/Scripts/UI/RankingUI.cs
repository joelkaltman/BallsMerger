using System;
using System.Collections.Generic;
using UnityEngine;

public class RankingUI : MonoBehaviour
{
    public int maxEntries;
    public Transform rankingContainer;
    public GameObject userRankObject;
    public GameObject loadingText;

    public UserRankUI mainUserRank;
    
    private List<GameObject> instances = new ();

    private void Start()
    {
        LoadRanking();
    }

    private void OnDestroy()
    {
        DestroyRanking();
    }

    public async void LoadRanking()
    {
        loadingText.SetActive(true);
        
        var ranking = await AuthManager.GetScoreboard();
        
        for (int i = 0; i < maxEntries && i < ranking.Count; i++)
        {
            if(ranking[i].maxScore <= 0)
                continue;
            
            var rankObject = Instantiate(userRankObject, rankingContainer);
            var rankUi = rankObject.GetComponent<UserRankUI>();
            var pos = i + 1;
            rankUi.SetUser(pos, ranking[i]);
            instances.Add(rankObject);
        }

        var username = UserManager.Instance.UserData.username;
        var userIndex = ranking.FindIndex(x => x.username == username);
        var userRank = userIndex < 0 ? new AuthManager.UserRank(username, 0) : ranking[userIndex];
        var mainPos = userIndex < 0 ? -1 : userIndex + 1;
        mainUserRank.SetUser(mainPos, userRank);
        
        loadingText.SetActive(false);
    }

    public void DestroyRanking()
    {
        foreach (var rankInstance in instances)
        {
            Destroy(rankInstance);
        }
    }
}
