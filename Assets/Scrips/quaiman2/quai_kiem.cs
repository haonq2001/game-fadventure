using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class quai_kiem : MonoBehaviour
{
    public GameObject boxvukhi; // Reference to the weapon box
    public GameObject thanhmau;

    private bool hasBeenAttacked = false;
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
    public float health = 10;

    public GameObject torchPrefab;  // Reference to the torch object
    public Transform dropPoint;
    public int enemyIndex; // Chỉ số duy nhất của quái
    // Point where the torch will drop
    private AudioSource audioSource;
    //public GameObject batnhacnen;
    //public GameObject tatnhacnen;
 //   public GameObject batamthanh;
   // public GameObject tatamthanh;
 //   public audioManager audioManager;
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

    //    if (PlayerPrefs.HasKey("EnemyHealth" + enemyIndex))
    //    {
    //        BossHealth.value = PlayerPrefs.GetFloat("EnemyHealth" + enemyIndex); // Load saved health value
    //    }
    //    else
    //    {
    //        BossHealth.value = health; // Default health
    //    }

    //    BossHealth.maxValue = health;
    //    fillImage.color = Color.red; // Default health bar color
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
            // Check if the player is out of attack range
            if (Vector2.Distance(transform.position, player.position) > attackRange)
            {
                isAttacking = false;
                animator.SetBool("attack", false); // Stop attack animation
                targetPoint = Vector2.Distance(transform.position, pointA.position) < Vector2.Distance(transform.position, pointB.position) ? pointB : pointA;
            }
            else
            {
                FacePlayer(); // Face towards the player
                Attack();
            }
        }
        else
        {
            // Move between points and enable movement animation
            MoveBetweenPoints();
            if (Vector2.Distance(transform.position, player.position) < attackRange)
            {
                isAttacking = true;
                animator.SetBool("attack", true); // Start attack animation
            }
        }
    }

    void MoveBetweenPoints()
    {
        transform.position = Vector2.MoveTowards(transform.position, targetPoint.position, speed * Time.deltaTime);
        animator.SetBool("isMoving", true); // Enable movement animation

        // Flip the boss based on movement direction
        if (targetPoint.position.x < transform.position.x)
        {
            transform.localScale = new Vector3(-1, 1, 1); // Face left
            MoveBoxCollider(0.5f); // Move the box collider to the left
        }
        else
        {
            transform.localScale = new Vector3(1, 1, 1); // Face right
            MoveBoxCollider(0.5f); // Move the box collider to the right
        }

        // Change direction when reaching point A or B
        if (Vector2.Distance(transform.position, targetPoint.position) < 0.1f)
        {
            targetPoint = targetPoint == pointA ? pointB : pointA;
        }
    }

    void MoveBoxCollider(float offset)
    {
        // Update the position of the box collider based on the boss's facing direction
        Vector3 boxPosition = boxvukhi.transform.localPosition;
        boxvukhi.transform.localPosition = new Vector3(offset, boxPosition.y, boxPosition.z);
    }

    private void Attack()
    {
        Debug.Log("Boss is attacking!");
        animator.SetBool("attack", true);
        // Add additional attack logic here if needed
    }

    private void FacePlayer()
    {
        // Face towards the player when attacking
        if (player.position.x < transform.position.x)
            transform.localScale = new Vector3(-1, 1, 1); // Face left
        else
            transform.localScale = new Vector3(1, 1, 1); // Face right
    }

    private void OnDisable()
    {
        // Ensure all animations are stopped when the boss is disabled
        animator.SetBool("isMoving", false);
        animator.SetBool("attack", false);
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
        if (collision.CompareTag("Bullet") )
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
        if (torchPrefab != null && dropPoint != null)
        {
            Instantiate(torchPrefab, dropPoint.position, Quaternion.identity);  // Drop the torch at dropPoint
        }
        PlayerPrefs.DeleteKey("EnemyHealth" + enemyIndex); // Xóa dữ liệu máu của quái

        // Xác suất rơi vật phẩm (giá trị giữa 0 và 1)
        float dropChance = Random.Range(0f, 1f);

        // Kiểm tra nếu giá trị sinh ngẫu nhiên nhỏ hơn một ngưỡng (ví dụ: 0.5 - 50% cơ hội rơi)
        if (dropChance < 0.2f)
        {
            // Drop vật phẩm (nếu có)
            if (torchPrefab != null && dropPoint != null)
            {
                Instantiate(torchPrefab, dropPoint.position, Quaternion.identity);
            }
        }

        // Gọi hàm hồi sinh từ GameManager
        GameManagermap2 gameManager = FindObjectOfType<GameManagermap2>();
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

