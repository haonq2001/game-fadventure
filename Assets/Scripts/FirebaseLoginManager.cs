using System.Collections;
using System.Collections.Generic;
using Firebase.Auth;
using Firebase.Extensions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FirebaseLoginManager : MonoBehaviour
{
    // Đăng ký
    [Header("Đăng Ký")]
    public InputField ipRegisterEmail;
    public InputField ipRegisterPassword;
    public Button buttonRegister;

    // Đăng nhập
    [Header("Đăng Nhập")]
    public InputField iploginEmail;
    public InputField iploginPassword;
    public Button buttonLogin;

    // Hiển thị lỗi
    [Header("Error Message")]
    public Text errorMessageText;

    // Firebase Authentication
    private FirebaseAuth auth;

    // Chuyển đổi giữa các form
    [Header("Switch Form")]
    public Button buttonMoveToSignIn;
    public Button buttonMoveToRegister;
    public GameObject loginForm;
    public GameObject registerForm;

    // Upload data User to Firebase
    private FirebaseDatabaseManager databaseManager;

    private void Start()
    {
        auth = FirebaseAuth.DefaultInstance;

        buttonRegister.onClick.AddListener(RegisterAccountWriteFirebase);
        buttonLogin.onClick.AddListener(SignInAccountWriteFirebase);

        buttonMoveToRegister.onClick.AddListener(SwitchForm);
        buttonMoveToSignIn.onClick.AddListener(SwitchForm);

        databaseManager = GetComponent<FirebaseDatabaseManager>();
    }

    private void ShowErrorMessage(string message)
    {
        errorMessageText.text = message;
        errorMessageText.color = Color.white; // Hiển thị lỗi bằng màu đỏ
    }

    private bool ValidateInputNotEmpty(string email, string password)
    {
        if (string.IsNullOrEmpty(email))
        {
            ShowErrorMessage("Email không được để trống.");
            return false;
        }

        if (string.IsNullOrEmpty(password))
        {
            ShowErrorMessage("Mật khẩu không được để trống.");
            return false;
        }

        return true;
    }

    private bool ValidateEmail(string email)
    {
        string emailRegex = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        if (!System.Text.RegularExpressions.Regex.IsMatch(email, emailRegex))
        {
            ShowErrorMessage("Email không hợp lệ. Email phải chứa '@' và có định dạng hợp lệ.");
            return false;
        }
        return true;
    }

    private bool ValidatePassword(string password)
    {
        if (password.Length < 6)
        {
            ShowErrorMessage("Mật khẩu phải có ít nhất 6 ký tự.");
            return false;
        }
        return true;
    }

    public void RegisterAccountWriteFirebase()
    {
        string email = ipRegisterEmail.text;
        string password = ipRegisterPassword.text;

        if (!ValidateInputNotEmpty(email, password) || !ValidateEmail(email) || !ValidatePassword(password))
        {
            return;
        }

        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                ShowErrorMessage("Đăng ký bị hủy.");
                return;
            }
            if (task.IsFaulted)
            {
                ShowErrorMessage($"Đăng ký thất bại: {task.Exception?.Message}");
                return;
            }
            if (task.IsCompleted)
            {
                Debug.Log("Đăng ký thành công!");
                FirebaseUser firebaseUser = task.Result.User;

                User userIngame = new User("");
                databaseManager.WriteDatabase("Users/" + firebaseUser.UserId, userIngame.ToString());

                LoadingManager.NEXT_SCENE = "hao";
                PlayerPrefs.DeleteAll(); // Xóa toàn bộ dữ liệu đã lưu
                SceneManager.LoadScene("LoadScene");
            }
        });
    }

    public void SignInAccountWriteFirebase()
    {
        string email = iploginEmail.text;
        string password = iploginPassword.text;

        if (!ValidateInputNotEmpty(email, password) || !ValidateEmail(email) || !ValidatePassword(password))
        {
            return;
        }

        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                ShowErrorMessage("Đăng nhập bị hủy.");
                return;
            }
            if (task.IsFaulted)
            {
                ShowErrorMessage($"Đăng nhập thất bại: {task.Exception?.Message}");
                return;
            }
            if (task.IsCompleted)
            {
                Debug.Log("Đăng nhập thành công!");
                FirebaseUser user = task.Result.User;

                LoadingManager.NEXT_SCENE = "hao";
                SceneManager.LoadScene("LoadScene");
            }
        });
    }

    public void SwitchForm()
    {
        loginForm.SetActive(!loginForm.activeSelf);
        registerForm.SetActive(!registerForm.activeSelf);

        // Xóa thông báo lỗi khi chuyển form
        errorMessageText.text = "";
    }
}
