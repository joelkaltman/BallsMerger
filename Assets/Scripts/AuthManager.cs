using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Database;

public static class AuthManager
{
    public static DependencyStatus dependencyStatus;
    static FirebaseAuth auth;    
    static FirebaseUser User;
    static DatabaseReference dbReference;

    public struct Result
    {
        public bool valid;
        public string error;

        public static Result Valid()
        {
            return new Result() { valid = true };
        }

        public static Result Error(string message)
        {
            return new Result() { error = message };
        }
    }

    public static async Task<Result> Initialize()
    {
        try
        {
            var init = await FirebaseApp.CheckAndFixDependenciesAsync();
            if (dependencyStatus == init)
            {
                auth = FirebaseAuth.DefaultInstance;
                dbReference = FirebaseDatabase.DefaultInstance.RootReference;
                return Result.Valid();
            }
        }
        catch (Exception e)
        {
            return Result.Error(e.Message);
        }
        
        return Result.Error("Service Unreachable");
    }

    public static void Logout()
    {
        auth.SignOut();
        auth = null;
        User = null;
        dbReference = null;
    }

    public static async Task<Result> Login(string email, string password)
    {
        try
        {
            var loginResult = await auth.SignInWithEmailAndPasswordAsync(email, password);
            User = loginResult.User;
        }
        catch (Exception exception)
        {
            return Result.Error(exception.Message);
        }

        return Result.Valid();
    }

    public static async Task<Result> Register(string email, string password, string username, int caps, List<int> guns)
    {
        try
        {
            var register = await auth.CreateUserWithEmailAndPasswordAsync(email, password);
            User = register.User;
        }
        catch (Exception exception)
        {
            return Result.Error(exception.Message);
        }

        if (User != null)
        {
            await UpdateUserProfile(username);
            await SaveUserData(new UserData() { username = username, caps = caps, guns = guns });
        }

        return Result.Valid();
    }

    private static async Task<Result> UpdateUserProfile(string username)
    {
        UserProfile profile = new UserProfile() { DisplayName = username};

        try
        {
            await User.UpdateUserProfileAsync(profile);
        }
        catch (Exception exception)
        {
            return Result.Error(exception.Message);
        }

        return Result.Valid();
    }
    
    public class UserData
    {
        public string username;
        public int maxKills;
        public int caps;
        public List<int> guns = new ();
    }
    
    public static async Task<bool> SaveUserData(UserData userData)
    {
        bool result = true;
        result &= await WriteToDb("username", userData.username);
        result &= await WriteToDb("kills", userData.maxKills);
        result &= await WriteToDb("caps", userData.caps);
        result &= await WriteToDb("guns", userData.guns);
        return result;
    }

    public static async Task<bool> WriteToDb(string field, object value)
    {
        var userEntry = dbReference.Child("users").Child(User.UserId);

        try
        {
            await userEntry.Child(field).SetValueAsync(value);
        }
        catch (Exception exception)
        {
            Debug.LogError($"Exception: {exception}");
            return false;
        }

        Debug.Log($"{field} for user {User.DisplayName} has been updated!");
        return true;
    }

    public static async Task<UserData> GetUserData()
    {
        var username = await ReadDb<string>("username");
        var kills = await ReadDb<long>("kills");
        var caps = await ReadDb<long>("caps");
        var guns = await ReadDb<List<object>>("guns");
        
        return new UserData()
        {
            username = username,
            maxKills = Convert.ToInt32(kills),
            caps = Convert.ToInt32(caps),
            guns = guns.Select(Convert.ToInt32).ToList()
        };
    }

    private static async Task<T> ReadDb<T>(string field)
    {
        var userEntry = dbReference.Child("users").Child(User.UserId);

        try
        {
            var result = await userEntry.Child(field).GetValueAsync();
            return result.Value != null ? (T)result.Value : default;
        }
        catch (Exception exception)
        {
            Debug.Log($"Exception: {exception}");
        }

        return default;
    }

    public struct UserRank
    {
        public string username;
        public int maxKills;

        public UserRank(object username, object kills)
        {
            this.username = username != null ? (string)username : default;
            this.maxKills = kills != null ? Convert.ToInt32((long)kills) : default;
        }
    }

    private static List<UserRank> lastRankingRetrieved = new();
    public static async Task<List<UserRank>> GetScoreboard()
    {
        List<UserRank> userRanks = new();
        try
        {
            var dataSnapshot = await dbReference.Child("users").OrderByChild("kills").GetValueAsync();
            foreach (var data in dataSnapshot.Children.Reverse())
            {
                var username = data.Child("username").Value;
                var kills = data.Child("kills").Value;
            
                userRanks.Add(new UserRank(username, kills));
            }
        }
        catch (Exception exception)
        {
            Debug.LogError($"Exception: {exception}");
        }

        lastRankingRetrieved = userRanks;
        return userRanks;
    }

    public static async Task<bool> IsTopScore(string username)
    {
        if (lastRankingRetrieved.Count == 0)
        {
            await GetScoreboard();
        }

        int maxScore = lastRankingRetrieved.Max(x => x.maxKills);
        return lastRankingRetrieved.Any(x => x.maxKills >= maxScore && x.username == username);
    }
}
