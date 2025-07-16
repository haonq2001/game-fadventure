using System.Collections;
using System.Collections.Generic;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using Newtonsoft.Json;
using UnityEngine;

public class LoadDataManager : MonoBehaviour
{
    public static FirebaseUser firebaseUser; // Người dùng Firebase hiện tại
    public static User userInGame; // Dữ liệu người chơi trong game

    private DatabaseReference reference;

    private void Awake()
    {
        // Đảm bảo đối tượng không bị xóa khi chuyển cảnh
        DontDestroyOnLoad(this.gameObject);

        // Khởi tạo Firebase
        FirebaseApp app = FirebaseApp.DefaultInstance;

        reference = FirebaseDatabase.DefaultInstance.RootReference;

        // Lấy người dùng Firebase hiện tại
        firebaseUser = FirebaseAuth.DefaultInstance.CurrentUser;

        // Kiểm tra nếu người dùng đã đăng nhập
        if (firebaseUser != null)
        {
            GetUserInGame();
        }
        else
        {
            Debug.LogWarning("Firebase user is not logged in!");
        }
    }

    public void GetUserInGame()
    {
        if (firebaseUser == null)
        {
            Debug.LogError("Cannot get user data because firebaseUser is null!");
            return;
        }

        // Lấy dữ liệu từ Firebase
        reference.Child("Users").Child(firebaseUser.UserId).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted && task.Result.Exists)
            {
                DataSnapshot snapshot = task.Result;

                if (snapshot.Value != null)
                {
                    // Chuyển dữ liệu từ JSON sang object User
                    userInGame = JsonConvert.DeserializeObject<User>(snapshot.Value.ToString());
                    Debug.Log("User data loaded successfully!");
                }
                else
                {
                    Debug.LogWarning("No data found for this user in Firebase.");
                }
            }
            else if (task.IsFaulted)
            {
                Debug.LogError("Error reading data from Firebase: " + task.Exception);
            }
            else if (task.IsCanceled)
            {
                Debug.LogWarning("Task to read data from Firebase was canceled.");
            }
        });
    }
}
