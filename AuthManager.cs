using UnityEngine;
using TMPro;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;

public class AuthManager : MonoBehaviour
{
    public TMP_InputField emailInput;
    public TMP_InputField passwordInput;
    public TMP_InputField usernameInput;
    public TMP_Text message;

    public GameObject panelLogin;
    public GameObject panelUser;

    private FirebaseAuth auth;
    private DatabaseReference db;

    public TMP_Text welcomeText;
    public TMP_Text scoreText;

    void Start()
    {
        auth = FirebaseAuth.DefaultInstance;
        db = FirebaseDatabase.DefaultInstance.RootReference;

        // 🔥 Verificar sesión activa
        if (auth.CurrentUser != null)
        {
            panelLogin.SetActive(false);
            panelUser.SetActive(true);

            LoadUserData(auth.CurrentUser.UserId);
        }
        else
        {
            panelLogin.SetActive(true);
            panelUser.SetActive(false);
        }
    }
    public void Register()
    {
        string email = emailInput.text;
        string password = passwordInput.text;
        string username = usernameInput.text;

        auth.CreateUserWithEmailAndPasswordAsync(email, password)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    message.text = "Error en registro";
                    Debug.Log(task.Exception);
                    return;
                }

                FirebaseUser newUser = task.Result.User;

                // Guardar username en database
                UserData data = new UserData(username, 0);

                string json = JsonUtility.ToJson(data);

                db.Child("users")
                    .Child(newUser.UserId)
                    .SetRawJsonValueAsync(json);

                message.text = "Usuario creado";

                // Login automático
                Login();
            });
    }

    public void Login()
    {
        string email = emailInput.text;
        string password = passwordInput.text;

        auth.SignInWithEmailAndPasswordAsync(email, password)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    message.text = "Error en login";
                    return;
                }

                var user = task.Result.User;

                message.text = "Login exitoso";

                panelLogin.SetActive(false);
                panelUser.SetActive(true);

                // 🔥 CARGAR DATOS
                LoadUserData(user.UserId);
            });
    }
    // Clase para guardar datos
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


    void LoadUserData(string userId)
    {
        FirebaseDatabase.DefaultInstance
            .GetReference("users")
            .Child(userId)
            .GetValueAsync()
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted) return;

                var snapshot = task.Result;

                if (snapshot.Exists)
                {
                    string username = snapshot.Child("username").Value.ToString();
                    string score = snapshot.Child("score").Value.ToString();

                    welcomeText.text = "Bienvenido " + username;
                    scoreText.text = "Score: " + score;
                }
            });
    }
    public void ResetPassword()
    {
        string email = emailInput.text;

        auth.SendPasswordResetEmailAsync(email)
            .ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    message.text = "Error enviando correo";
                    return;
                }

                message.text = "Correo enviado";
            });
    }
    public void Logout()
    {
        auth.SignOut();

        panelUser.SetActive(false);
        panelLogin.SetActive(true);

        // Opcional limpiar inputs
        emailInput.text = "";
        passwordInput.text = "";
        usernameInput.text = "";

        message.text = "Sesión cerrada";
    }
}