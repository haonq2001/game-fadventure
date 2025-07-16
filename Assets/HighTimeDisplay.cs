using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Firebase.Database;
using Firebase.Extensions;

public class HighTimeDisplay : MonoBehaviour
{
    public GameObject highTimePanel; // Kéo thả Panel từ UI vào đây
    public Text[] highTimeTexts; // Kéo thả các Text vào đây (Time1, Time2, ..., Time5)
    private TimerManager timerManager;

    private void Start()
    {
        highTimePanel.SetActive(false); // Ẩn bảng ban đầu
        timerManager = FindObjectOfType<TimerManager>();
    }

    public void ShowHighTime()
    {
        LoadHighScores(); // Lấy điểm cao từ Firebase và hiển thị

        highTimePanel.SetActive(true); // Hiển thị bảng
        Time.timeScale = 0; // Dừng game khi hiển thị bảng
    }

    public void LoadHighScores()
    {
        // Truy cập Firebase tại nhánh "highscores" (của từng người chơi)
        FirebaseDatabase.DefaultInstance.RootReference.Child("highscores").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;

                // Lấy dữ liệu điểm cao từ Firebase
                List<HighscoreData> highscoreList = new List<HighscoreData>();
                foreach (var childSnapshot in snapshot.Children)
                {
                    HighscoreData scoreData = JsonUtility.FromJson<HighscoreData>(childSnapshot.GetRawJsonValue());
                    highscoreList.Add(scoreData);
                }

                // Sắp xếp danh sách điểm cao (nếu có)
                highscoreList.Sort((a, b) => CompareTimes(b.time, a.time)); // Sắp xếp từ cao đến thấp theo thời gian

                // Hiển thị tối đa 5 điểm cao nhất
                for (int i = 0; i < highTimeTexts.Length; i++)
                {
                    if (i < highscoreList.Count)
                    {
                        highTimeTexts[i].text = (i + 1) + ". " + highscoreList[i].name + " - " + highscoreList[i].time;
                    }
                    else
                    {
                        highTimeTexts[i].text = ""; // Nếu ít hơn 5 điểm cao, để trống
                    }
                }
            }
        });
    }

    private int CompareTimes(string timeA, string timeB)
    {
        int minutesA = int.Parse(timeA.Substring(0, 2));
        int secondsA = int.Parse(timeA.Substring(3, 2));

        int minutesB = int.Parse(timeB.Substring(0, 2));
        int secondsB = int.Parse(timeB.Substring(3, 2));

        if (minutesA == minutesB)
        {
            return secondsB.CompareTo(secondsA); // Sắp xếp theo giây nếu phút bằng nhau
        }
        return minutesB.CompareTo(minutesA); // Sắp xếp theo phút
    }
}
