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

    public static async Task<Result> Register(string email, string password, string username)
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
            await SaveUserData(new UserData() { username = username });
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
        public int maxScore;
    }
    
    public static async Task<bool> SaveUserData(UserData userData)
    {
        bool result = true;
        result &= await WriteToDb("username", userData.username);
        result &= await WriteToDb("maxScore", userData.maxScore);
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
        var maxScore = await ReadDb<long>("maxScore");
        var caps = await ReadDb<long>("caps");
        var guns = await ReadDb<List<object>>("guns");
        
        return new UserData()
        {
            username = username,
            maxScore = Convert.ToInt32(maxScore),
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
        public int maxScore;

        public UserRank(object username, object maxScore)
        {
            this.username = username != null ? (string)username : default;
            this.maxScore = maxScore != null ? Convert.ToInt32((long)maxScore) : default;
        }
    }

    private static List<UserRank> lastRankingRetrieved = new();
    public static async Task<List<UserRank>> GetScoreboard()
    {
        List<UserRank> userRanks = new();
        try
        {
            var dataSnapshot = await dbReference.Child("users").OrderByChild("maxScore").GetValueAsync();
            foreach (var data in dataSnapshot.Children.Reverse())
            {
                var username = data.Child("username").Value;
                var maxScore = data.Child("maxScore").Value;
            
                userRanks.Add(new UserRank(username, maxScore));
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

        int maxScore = lastRankingRetrieved.Max(x => x.maxScore);
        return lastRankingRetrieved.Any(x => x.maxScore >= maxScore && x.username == username);
    }
}
