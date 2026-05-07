using UnityEngine;
using TMPro;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;

public class GameManager : MonoBehaviour
{
    public int score = 0;

    [Header("UI")]
    public TMP_Text scoreText;
    public GameObject gameOverPanel;
    public TMP_Text finalScoreText;

    private bool isGameOver = false;

    void Start()
    {
        UpdateScoreUI();

        gameOverPanel.SetActive(false);

        Time.timeScale = 1f;
    }

    // =========================
    // SCORE
    // =========================

    public void AddScore(int points)
    {
        if (isGameOver) return;

        score += points;

        UpdateScoreUI();
    }

    void UpdateScoreUI()
    {
        scoreText.text = "Score: " + score;
    }

    // =========================
    // GAME OVER
    // =========================

    public void GameOver()
    {
        if (isGameOver) return;

        isGameOver = true;

        // Mostrar panel
        gameOverPanel.SetActive(true);

        finalScoreText.text = "Final Score: " + score;

        // Guardar high score
        SaveScore();

        // Pausar juego
        Time.timeScale = 0f;
    }

    // =========================
    // SAVE HIGH SCORE
    // =========================

    public void SaveScore()
    {
        string userId =
            FirebaseAuth.DefaultInstance.CurrentUser.UserId;

        DatabaseReference userRef =
            FirebaseDatabase.DefaultInstance
            .GetReference("users")
            .Child(userId);

        // Leer datos actuales
        userRef.GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.Log("Error leyendo datos");
                return;
            }

            DataSnapshot snapshot = task.Result;

            string username = "";

            int previousScore = 0;

            // Leer username
            if (snapshot.Child("username").Exists)
            {
                username =
                    snapshot.Child("username")
                    .Value
                    .ToString();
            }

            // Leer score previo
            if (snapshot.Child("score").Exists)
            {
                previousScore =
                    int.Parse(
                        snapshot.Child("score")
                        .Value
                        .ToString()
                    );
            }

            Debug.Log("Previous Score: " + previousScore);
            Debug.Log("Current Score: " + score);

            // Solo guardar si es mayor
            if (score > previousScore)
            {
                UserData data =
                    new UserData(username, score);

                string json =
                    JsonUtility.ToJson(data);

                userRef.SetRawJsonValueAsync(json);

                Debug.Log("🔥 Nuevo High Score guardado");
            }
            else
            {
                Debug.Log("No superó el high score");
            }
        });
    }

    // =========================
    // BOTONES
    // =========================

    public void BackToMenu()
    {
        Time.timeScale = 1f;

        UnityEngine.SceneManagement.SceneManager
            .LoadScene("MainScene");
    }

    public void TryAgain()
    {
        Time.timeScale = 1f;

        UnityEngine.SceneManagement.SceneManager
            .LoadScene("GameScene");
    }
}

// =========================
// USER DATA
// =========================

[System.Serializable]
public class UserData
{
    public string username;
    public int score;

    public UserData(string username, int score)
    {
        this.username = username;
        this.score = score;
    }
}