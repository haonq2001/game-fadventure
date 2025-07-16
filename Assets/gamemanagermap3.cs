using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine;
using Firebase.Auth;
using System.Collections;
using UnityEngine.SocialPlatforms.Impl;

public class gamemanagermap3 : MonoBehaviour
{
    public GameObject gameOverUI;
    public HighTimeDisplay highTimeDisplay; // Kéo-thả trong Inspector

   

    private TimerManager timerManager;
    public TextMeshProUGUI scoreTexthp;
    public TextMeshProUGUI scoreTextmp;


    public int scorehp = 0;
    public int scoremp = 0;

    public List<GameObject> enemies; // Danh sách chứa quái

    void Start()
    {
        //if (highTimeDisplay == null)
        //{
        //    Debug.LogError("HighTimeDisplay is null. Please assign it in the Inspector!");
        //}
        //highTimeDisplay = FindObjectOfType<HighTimeDisplay>();
        timerManager = FindObjectOfType<TimerManager>();
        //// Tải điểm từ PlayerPrefs nếu tồn tại
        //if (PlayerPrefs.HasKey("PlayerScore"))
        //{
        //    score = PlayerPrefs.GetInt("PlayerScore");
        //}
        //else
        //{
        //    score = 0; // Nếu không có, bắt đầu từ 0
        //}
        //SetScoreText();

        //// Tải vị trí nhân vật
        //if (PlayerPrefs.HasKey("PlayerPosX") && PlayerPrefs.HasKey("PlayerPosY") && PlayerPrefs.HasKey("PlayerPosZ"))
        //{
        //    float x = PlayerPrefs.GetFloat("PlayerPosX");
        //    float y = PlayerPrefs.GetFloat("PlayerPosY");
        //    float z = PlayerPrefs.GetFloat("PlayerPosZ");

        //    Vector3 savedPosition = new Vector3(x, y, z);
        //    player.transform.position = savedPosition; // Đặt lại vị trí nhân vật
        //}

        // Khôi phục trạng thái quái
        InitializeEnemies();

        // Nạp lại trạng thái máu và mana
      //  LoadPlayerState();
    }

    public void AddScorehp()
    {
        scorehp+=20; // Chỉ tăng máu
      
        SetScoreTexthp();
    }

    public void AddScoremp()
    {
        scoremp+=20; // Chỉ tăng mana
       
        SetScoreTextmp();
    }


   
    public void BlockScorehp()
    {
        if (scorehp > 0) // Chỉ giảm nếu số lượng lớn hơn 0
        {
            scorehp--;
          
            SetScoreTexthp();
        }
        else
        {
            Debug.Log("Không thể giảm số lượng bình HP, đã bằng 0");
        }
    }


    public void BlockScoremp()
    {
        if (scoremp > 0) // Chỉ giảm nếu số lượng lớn hơn 0
        {
            scoremp--;
          
            SetScoreTextmp();
        }
        else
        {
            Debug.Log("Không thể giảm số lượng bình MP, đã bằng 0");
        }
    }


  
    public void SetScoreTexthp()
    {
        scoreTexthp.text = "" + scorehp.ToString("n0");
    }
    public void SetScoreTextmp()
    {
        scoreTextmp.text = "" + scoremp.ToString("n0");
    }

    public void OnMap3Complete()
    {
        if (timerManager != null)
        {
            timerManager.StopTimer(); // Dừng bộ đếm thời gian
        }
        else
        {
            Debug.LogError("TimerManager is null. Please check your setup!");
        }

        if (highTimeDisplay != null)
        {
            highTimeDisplay.ShowHighTime(); // Hiển thị bảng thời gian
        }
        else
        {
            Debug.LogError("HighTimeDisplay is null. Please check your setup!");
        }
    }

    public void GameOver()
    {
        Time.timeScale = 0;
        gameOverUI.SetActive(true);
    }

    public void ReStart()
    {
        PlayerPrefs.DeleteAll(); // Xóa toàn bộ dữ liệu đã lưu
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // Tải lại màn hiện tại
    }

    public void MainMenu()

    {
        FirebaseAuth.DefaultInstance.SignOut();
        // Xóa đối tượng LoadDataManager nếu nó vẫn còn
        //   LoadDataManager.Instance = null;
        Destroy(GameObject.Find("LoadDataManager")); // Xóa đối tượng LoadDataManager khỏi scene
        Destroy(GameObject.Find("Audio Manager")); // Xóa đối tượng LoadDataManager khỏi scene

        //   SceneManager.LoadScene("LoginScene");  // Đảm bảo rằng tên scene là đúng
        //   LoadingManager.NEXT_SCENE = "hao";
        SceneManager.LoadScene("LoginScene");

    }

    public void Quit()
    {
        Application.Quit();
    }

    void OnApplicationQuit()
    {
      //  SaveGameState();  // Lưu tất cả các dữ liệu game
      //  SavePlayerState(); // Lưu trạng thái nhân vật
    }

    //void SavePlayerState()
    //{
    //    PlayerPrefs.SetInt("PlayerHealth", player.health);  // Lưu máu
    //    PlayerPrefs.SetInt("PlayerMana", player.mana);      // Lưu mana
    //}

    //void LoadPlayerState()
    //{
    //    if (PlayerPrefs.HasKey("PlayerHealth"))
    //    {
    //        player.health = PlayerPrefs.GetInt("PlayerHealth");
    //    }

    //    if (PlayerPrefs.HasKey("PlayerMana"))
    //    {
    //        player.mana = PlayerPrefs.GetInt("PlayerMana");
    //    }
    //}

    //void SaveGameState()
    //{
    //    // Lưu màn chơi hiện tại
    //    PlayerPrefs.SetInt("CurrentScene", SceneManager.GetActiveScene().buildIndex);

    //    // Lưu vị trí nhân vật
    //    Vector3 playerPosition = player.transform.position;
    //    PlayerPrefs.SetFloat("PlayerPosX", playerPosition.x);
    //    PlayerPrefs.SetFloat("PlayerPosY", playerPosition.y);
    //    PlayerPrefs.SetFloat("PlayerPosZ", playerPosition.z);

    //    PlayerPrefs.Save(); // Lưu tất cả dữ liệu
    //}

    // Lưu trạng thái quái
    public void SaveEnemyState(int enemyIndex, bool isDead)
    {
        string key = "Enemy" + enemyIndex + "Dead";
        PlayerPrefs.SetInt(key, isDead ? 1 : 0);  // Lưu trạng thái quái
    }



    public void RespawnEnemy(Boss boss, float delay)
    {
        StartCoroutine(RespawnEnemyCoroutine(boss, delay));
    }



    private IEnumerator RespawnEnemyCoroutine(Boss boss, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (boss != null)
        {
            boss.gameObject.SetActive(true); // Hiện lại quái
            boss.thanhmau.SetActive(true);  // Hiện thanh máu
            boss.ResetBoss();               // Đặt lại trạng thái ban đầu của quái
        }
    }

    void InitializeEnemies()
    {
        for (int i = 0; i < enemies.Count; i++)
        {
            string key = "Enemy" + i + "Dead";
            if (PlayerPrefs.GetInt(key, 0) == 1)
            {
                enemies[i].SetActive(false);  // Ẩn quái nếu đã chết
            }
            else
            {
                enemies[i].SetActive(true);  // Hiện quái nếu chưa chết
            }
        }
    }
}
