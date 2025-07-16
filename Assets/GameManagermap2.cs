using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManagermap2 : MonoBehaviour
{
    public GameObject gameOverUI;
    public TextMeshProUGUI scoreText;
     public TextMeshProUGUI scoreTexthp;
     public TextMeshProUGUI scoreTextmp;

    public TextMeshProUGUI playerNameText; // Text để hiển thị tên người chơi
    public playermap2 player;

    public int score = 0;
  public int scorehp = 0;
   public int scoremp = 0;
    public List<GameObject> enemies; // Danh sách chứa quái
    public List<GameObject> enemies1; // Danh sách chứa quái

    void Start()
    {
        LoadGameState();

        // Hiển thị tên người chơi
        if (LoadDataManager.userInGame != null && !string.IsNullOrEmpty(LoadDataManager.userInGame.Name))
        {
            playerNameText.text = "" + LoadDataManager.userInGame.Name;
        }
        else
        {
            playerNameText.text = "Player: Guest";
        }
        // Khôi phục trạng thái quái
        InitializeEnemies();
        InitializeEnemies1();
    }

    public void AddScore()
    {
        score++;
        SaveGameState();
        SetScoreText();
    }
    public void AddScorehp()
    {
        scorehp++; // Chỉ tăng máu
        SaveGameState();
        SetScoreTexthp();
    }

    public void AddScoremp()
    {
        scoremp++; // Chỉ tăng mana
        SaveGameState();
        SetScoreTextmp();
    }


    public void BlockScore()
    {
        if (score > 0) // Chỉ giảm nếu số lượng lớn hơn 0
        {
            score--;
            SaveGameState();
            SetScoreText();
        }
    }
    public void BlockScorehp()
    {
        if (scorehp > 0) // Chỉ giảm nếu số lượng lớn hơn 0
        {
            scorehp--;
            SaveGameState();
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
            SaveGameState();
            SetScoreTextmp();
        }
        else
        {
            Debug.Log("Không thể giảm số lượng bình MP, đã bằng 0");
        }
    }


    public void SetScoreText()
    {
        scoreText.text = "" + score.ToString("n0");
    }  public void SetScoreTexthp()
    {
        scoreTexthp.text = "" + scorehp.ToString("n0");
    }  public void SetScoreTextmp()
    {
        scoreTextmp.text = "" + scoremp.ToString("n0");
    }

    public void GameOver()
    {
        Time.timeScale = 0;
        gameOverUI.SetActive(true);
       
    }

    public void ReStart()
    {
        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("LoginScene");
    }

    public void Quit()
    {
        Application.Quit();
    }

    void OnApplicationQuit()
    {
        SaveGameState();
    }

    void SaveGameState()
    {
        //PlayerPrefs.SetInt("PlayerScore", score);
        //Vector3 playerPosition = player.transform.position;
        //PlayerPrefs.SetFloat("PlayerPosX", playerPosition.x);
        //PlayerPrefs.SetFloat("PlayerPosY", playerPosition.y);
        //PlayerPrefs.SetFloat("PlayerPosZ", playerPosition.z);
        //PlayerPrefs.Save();
    }

    void LoadGameState()
    {
        //if (PlayerPrefs.HasKey("PlayerScore"))
        //{
        //    score = PlayerPrefs.GetInt("PlayerScore");
        //    SetScoreText();
        //}

        //if (PlayerPrefs.HasKey("PlayerPosX") && PlayerPrefs.HasKey("PlayerPosY") && PlayerPrefs.HasKey("PlayerPosZ"))
        //{
        //    float x = PlayerPrefs.GetFloat("PlayerPosX");
        //    float y = PlayerPrefs.GetFloat("PlayerPosY");
        //    float z = PlayerPrefs.GetFloat("PlayerPosZ");
        //    player.transform.position = new Vector3(x, y, z);
        //}
    }
    public void SaveEnemyState(int enemyIndex, bool isDead)
    {
        string key = "Enemy" + enemyIndex + "Dead";
        PlayerPrefs.SetInt(key, isDead ? 1 : 0);  // Lưu trạng thái quái
    }



    public void RespawnEnemy(quai_kiem boss, float delay)
    {
        StartCoroutine(RespawnEnemyCoroutine(boss, delay));
    }
    public void RespawnEnemy1(quaiphep boss, float delay)
    {
        StartCoroutine(RespawnEnemyCoroutine1(boss, delay));
    }



    private IEnumerator RespawnEnemyCoroutine(quai_kiem boss, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (boss != null)
        {
            boss.gameObject.SetActive(true); // Hiện lại quái
            boss.thanhmau.SetActive(true);  // Hiện thanh máu
            boss.ResetBoss();               // Đặt lại trạng thái ban đầu của quái
        }
    }
    private IEnumerator RespawnEnemyCoroutine1(quaiphep boss, float delay)
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
    void InitializeEnemies1()
    {
        for (int i = 0; i < enemies1.Count; i++)
        {
            string key = "Enemy" + i + "Dead";
            if (PlayerPrefs.GetInt(key, 0) == 1)
            {
                enemies1[i].SetActive(false);  // Ẩn quái nếu đã chết
            }
            else
            {
                enemies1[i].SetActive(true);  // Hiện quái nếu chưa chết
            }
        }
    }
}
