using UnityEngine;
using TMPro;
using Firebase.Database;
using Firebase.Extensions;
using System.Collections.Generic;
using System.Linq;

public class LeaderboardManager : MonoBehaviour
{
    public GameObject leaderboardPanel;

    public TMP_Text[] entries;

    void Start()
    {
        leaderboardPanel.SetActive(false);
    }

    // 🔥 ABRIR PANEL
    public void OpenLeaderboard()
    {
        leaderboardPanel.SetActive(true);

        LoadLeaderboard();
    }

    // 🔥 CERRAR PANEL
    public void CloseLeaderboard()
    {
        leaderboardPanel.SetActive(false);
    }

    // 🔥 CARGAR DATOS
    void LoadLeaderboard()
    {
        FirebaseDatabase.DefaultInstance
            .GetReference("users")
            .GetValueAsync()
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.Log("Error leaderboard");
                    return;
                }

                DataSnapshot snapshot = task.Result;

                List<UserScore> scores = new List<UserScore>();

                foreach (var child in snapshot.Children)
                {
                    if (!child.Child("username").Exists ||
                        !child.Child("score").Exists)
                        continue;

                    string username =
                        child.Child("username").Value.ToString();

                    int score =
                        int.Parse(child.Child("score").Value.ToString());

                    scores.Add(new UserScore(username, score));
                }

                scores = scores
                    .OrderByDescending(x => x.score)
                    .ToList();

                for (int i = 0; i < entries.Length; i++)
                {
                    if (i < scores.Count)
                    {
                        entries[i].text =
                            (i + 1) + ". " +
                            scores[i].username +
                            " - " +
                            scores[i].score;
                    }
                    else
                    {
                        entries[i].text = "---";
                    }
                }
            });
    }

}

[System.Serializable]
public class UserScore
{
    public string username;
    public int score;

    public UserScore(string username, int score)
    {
        this.username = username;
        this.score = score;
    }
}