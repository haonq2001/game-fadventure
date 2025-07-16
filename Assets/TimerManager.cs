using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine;
using UnityEngine.UI;

public class TimerManager : MonoBehaviour
{
    public Text timerText; // Kéo thả Text từ UI vào đây
    private float elapsedTime = 0f; // Tổng thời gian chơi
    private bool isTiming = true; // Cờ kiểm tra trạng thái thời gian
    private string filePath; // Đường dẫn file lưu

    private DatabaseReference reference;

    private static TimerManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Đảm bảo đối tượng không bị hủy khi chuyển Scene
        }
        else
        {
            Destroy(gameObject); // Nếu đối tượng đã tồn tại, xóa đối tượng mới tạo
        }
    }

    private void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            FirebaseApp app = FirebaseApp.DefaultInstance;
            reference = FirebaseDatabase.DefaultInstance.RootReference;
        });
    }

    private void Update()
    {
        if (isTiming)
        {
            elapsedTime += Time.deltaTime; // Tính thời gian chơi
            UpdateTimerUI();
        }
    }

    private void UpdateTimerUI()
    {
        int minutes = Mathf.FloorToInt(elapsedTime / 60);
        int seconds = Mathf.FloorToInt(elapsedTime % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds); // Cập nhật UI
    }

    public void StopTimer()
    {
        isTiming = false;
        SaveTimeToFirebase(); // Lưu thời gian khi dừng
    }

    private void SaveTimeToFirebase()
    {
        string formattedTime = FormatTime(elapsedTime);

        // Lưu thời gian vào Firebase với key là User ID
        string userId = LoadDataManager.firebaseUser.UserId; // Giả sử bạn đã có User ID từ Firebase Auth
        string userName = LoadDataManager.userInGame.Name;

        var timeData = new HighscoreData(userName, formattedTime);

        string json = JsonUtility.ToJson(timeData); // Chuyển data thành JSON để gửi lên Firebase

        reference.Child("highscores").Child(userId).SetRawJsonValueAsync(json); // Gửi lên Firebase

        Debug.Log("Thời gian đã được lưu vào Firebase!");
    }


    private string FormatTime(float timeInSeconds)
    {
        int minutes = Mathf.FloorToInt(timeInSeconds / 60);
        int seconds = Mathf.FloorToInt(timeInSeconds % 60);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }
    public void ResetTimer()
    {
        // Đặt lại thời gian đã trôi qua về 0
        elapsedTime = 0f;
        // Cập nhật lại UI của thời gian
        UpdateTimerUI();

        // Tiếp tục lại bộ đếm thời gian
        isTiming = true;
    }
}

[System.Serializable]
public class HighscoreData
{
    public string name;
    public string time;

    public HighscoreData(string name, string time)
    {
        this.name = name;
        this.time = time;
    }
}