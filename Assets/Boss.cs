using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Boss : MonoBehaviour
{
    private bool hasBeenAttacked = false;
    public GameObject boxvukhi; // Reference to the weapon box
    public GameObject thanhmau;

    public Transform pointA;
    public Transform pointB;
    public float speed = 2f;
    public float attackRange = 3f;
    private Transform targetPoint;
    public Transform player;
    private bool isAttacking = false;
    private Animator animator;

    public Slider BossHealth;
    public Image fillImage;
    public float health = 30;

    public GameObject torchPrefab;  // Reference to the torch object
    public Transform dropPoint;

    // Point where the torch will drop
    private AudioSource audioSource;

    public int enemyIndex; // Chỉ số duy nhất của quái

    void Start()
    {
        targetPoint = pointB; // Initially move towards point B
        player = GameObject.FindGameObjectWithTag("Player").transform;
        animator = GetComponent<Animator>(); // Get the Animator component
        BossHealth.maxValue = health;
        BossHealth.value = health;
        BossHealth.interactable = false;

        boxvukhi.SetActive(false); // Initially hide the weapon box

        audioSource = GetComponent<AudioSource>();

        if (PlayerPrefs.HasKey("EnemyHealth" + enemyIndex))
        {
            BossHealth.value = PlayerPrefs.GetFloat("EnemyHealth" + enemyIndex); // Load saved health value
        }
        else
        {
            BossHealth.value = health; // Default health
        }

        BossHealth.maxValue = health;
        fillImage.color = Color.red; // Default health bar color
    }

    public void vukhion()
    {
        boxvukhi.SetActive(true); // Show the weapon box
    }

    public void vukhioff()
    {
        boxvukhi.SetActive(false); // Hide the weapon box
    }

    void Update()
    {
        if (isAttacking)
        {
            if (Vector2.Distance(transform.position, player.position) > attackRange)
            {
                isAttacking = false;
                animator.SetBool("attack", false);
                targetPoint = Vector2.Distance(transform.position, pointA.position) < Vector2.Distance(transform.position, pointB.position) ? pointB : pointA;
            }
            else
            {
                FacePlayer();
                Attack();
            }
        }
        else
        {
            MoveBetweenPoints();
            if (Vector2.Distance(transform.position, player.position) < attackRange)
            {
                isAttacking = true;
                animator.SetBool("attack", true);
            }
        }
    }

    void MoveBetweenPoints()
    {
        transform.position = Vector2.MoveTowards(transform.position, targetPoint.position, speed * Time.deltaTime);
        animator.SetBool("isMoving", true);

        if (targetPoint.position.x < transform.position.x)
        {
            transform.localScale = new Vector3(-1, 1, 1);
            MoveBoxCollider(-0.5f);
        }
        else
        {
            transform.localScale = new Vector3(1, 1, 1);
            MoveBoxCollider(0.5f);
        }

        if (Vector2.Distance(transform.position, targetPoint.position) < 0.1f)
        {
            targetPoint = targetPoint == pointA ? pointB : pointA;
        }
    }

    void MoveBoxCollider(float offset)
    {
        Vector3 boxPosition = boxvukhi.transform.localPosition;
        boxvukhi.transform.localPosition = new Vector3(offset, boxPosition.y, boxPosition.z);
    }

    private void Attack()
    {
        Debug.Log("Boss is attacking!");
        animator.SetBool("attack", true);
    }

    private void FacePlayer()
    {
        if (player.position.x < transform.position.x)
            transform.localScale = new Vector3(-1, 1, 1);
        else
            transform.localScale = new Vector3(1, 1, 1);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ( collision.CompareTag("Sword"))
        {
            BossHealth.value -= 2;
            PlayerPrefs.SetFloat("EnemyHealth" + enemyIndex, BossHealth.value);
            PlayerPrefs.Save();

        //    animator.SetBool("hulk", true);
      //      audioManager.Instance.PlaySFX("matmau");

            UpdateHealthUI();

            if (BossHealth.value <= 0)
            {
                animator.SetTrigger("die");
                StartCoroutine(WaitForDeathAnimation());
            }
        }

        if (collision.CompareTag("Bullet"))
        {
            BossHealth.value -= 5;
            PlayerPrefs.SetFloat("EnemyHealth" + enemyIndex, BossHealth.value);
            PlayerPrefs.Save();

            //    animator.SetBool("hulk", true);
            //      audioManager.Instance.PlaySFX("matmau");

            UpdateHealthUI();

            if (BossHealth.value <= 0)
            {
                animator.SetTrigger("die");
                StartCoroutine(WaitForDeathAnimation());
            }
        }
    }

    private void UpdateHealthUI()
    {
        if (BossHealth.value < 4)
        {
            fillImage.color = Color.red;
            hasBeenAttacked = true;
        }
        else if (BossHealth.value < 8 && BossHealth.value >= 4)
        {
            if (!hasBeenAttacked)
            {
           //     fillImage.color = Color.yellow;
            }
        }
        else
        {
        //    fillImage.color = Color.green;
            hasBeenAttacked = false;
        }
    }

    IEnumerator WaitForDeathAnimation()
    {
        yield return new WaitForSeconds(1f);
        Die();
    }

    public void Die()
    {
        gameObject.SetActive(false); // Tắt đối tượng quái
        thanhmau.SetActive(false);  // Tắt thanh máu

        PlayerPrefs.DeleteKey("EnemyHealth" + enemyIndex); // Xóa dữ liệu máu của quái

        // Xác suất rơi vật phẩm (giá trị giữa 0 và 1)
        float dropChance = Random.Range(0f, 1f);

        // Kiểm tra nếu giá trị sinh ngẫu nhiên nhỏ hơn một ngưỡng (ví dụ: 0.5 - 50% cơ hội rơi)
        if (dropChance < 0.4f)
        {
            // Drop vật phẩm (nếu có)
            if (torchPrefab != null && dropPoint != null)
            {
                Instantiate(torchPrefab, dropPoint.position, Quaternion.identity);
            }
        }

        // Gọi hàm hồi sinh từ GameManager
        GameManager gameManager = FindObjectOfType<GameManager>();
        if (gameManager != null)
        {
           gameManager.RespawnEnemy(this, 20f); // Hồi sinh quái sau 20 giây
        }
    }


    IEnumerator Respawn()
    {
        yield return new WaitForSeconds(20f);
        gameObject.SetActive(true);
        thanhmau.SetActive(true);
        ResetBoss();
    }

    public void ResetBoss()
    {
        health = BossHealth.maxValue;
        BossHealth.value = health;
        fillImage.color = Color.red;
        animator.ResetTrigger("die");
        animator.SetBool("isMoving", false);
        animator.SetBool("attack", false);
        hasBeenAttacked = false;
    }
}
