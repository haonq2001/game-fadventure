using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class boss_man3_code_khac : MonoBehaviour
{
    public Transform pointA; // Điểm A
    public Transform pointB; // Điểm B
    public float speed = 2f; // Tốc độ di chuyển
    public float attackRange = 2f; // Phạm vi tấn công chiêu 1
    public float attack2Range = 8f; // Phạm vi tấn công chiêu 2
    public float maxHeightDifference = 3f; // Phạm vi nhận diện theo chiều dọc
    public float attackCooldown = 4f; // Khoảng cách giữa các lần tấn công
    public GameObject lightningPrefab; // Prefab tia sét
    public Transform firePoint; // Vị trí bắn tia sét
    public float lightningSpeed = 5f; // Tốc độ tia sét

    private Transform player; // Nhân vật
    private Transform targetPoint; // Điểm hiện tại để di chuyển
    private bool isAttacking = false; // Boss đang tấn công
    private bool isDead = false; // Boss đã chết
    private int attackCounter = 0; // Đếm số lần tấn công chiêu 1
    private float lastAttackTime = -999f; // Thời gian lần tấn công gần nhất
    private Animator animator;

    public GameObject thanhmau1;
    public GameObject thanhmau2;
    public Slider hpboss1;
    public Slider hpboss2;
    public Image fillhp;
    public Image fillhp2;
    public float hp1 = 50;
    public float hp2 = 50;
    private bool isPaused = false; // Biến để kiểm tra boss có bị dừng hay không
    public GameObject cong3win;

    void Start()
    {
        targetPoint = pointB; // Điểm khởi tạo là pointB
        player = GameObject.FindGameObjectWithTag("Player").transform;
        animator = GetComponent<Animator>();
        hpboss1.maxValue = hp1;
        hpboss1.value = hp1;
        hpboss1.interactable = false;
        hpboss2.maxValue = hp2;
        hpboss2.value = hp2;
        hpboss2.interactable = false;
        cong3win.SetActive(false);
    }

    void Update()
    {
        if (isDead) return;

        if (IsPlayerInDetectionRange())
        {
            MoveTowardsPlayer();
            RotateTowardsPlayer(); // Xoay mặt về phía người chơi

            // Tấn công khi đủ thời gian
            if (Time.time - lastAttackTime >= attackCooldown)
            {
                if (attackCounter < 2 && IsPlayerInAttack1Range())
                {
                    StartAttack1();
                }
                else if (attackCounter >= 2 && IsPlayerInAttack2Range())
                {
                    StartAttack2();
                }
            }
        }
        else
        {
            MoveBetweenPoints();
        }

        animator.SetBool("isWalking", !isAttacking);
        animator.SetBool("isRunning", IsPlayerInDetectionRange() && !isAttacking);
    }

    bool IsPlayerInDetectionRange()
    {
        bool isWithinHorizontalRange = player.position.x >= pointA.position.x && player.position.x <= pointB.position.x;
        float distanceY = Mathf.Abs(transform.position.y - player.position.y);
        bool isWithinVerticalRange = distanceY <= maxHeightDifference;

        return isWithinHorizontalRange && isWithinVerticalRange;
    }

    bool IsPlayerInAttack1Range()
    {
        float distanceX = Mathf.Abs(transform.position.x - player.position.x);
        return distanceX <= attackRange;
    }

    bool IsPlayerInAttack2Range()
    {
        float distanceX = Mathf.Abs(transform.position.x - player.position.x);
        return distanceX <= attack2Range; // Tấn công trong cả phạm vi attackRange và attack2Range
    }

    void MoveTowardsPlayer()
    {
        if (!isAttacking)
        {
            float distanceToPlayer = Mathf.Abs(transform.position.x - player.position.x);

            if (distanceToPlayer > attackRange)
            {
                Vector2 direction = new Vector2(player.position.x - transform.position.x, 0).normalized;
                Vector2 targetPosition = new Vector2(player.position.x, transform.position.y) - direction * attackRange;

                transform.position = Vector2.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
            }
        }
    }

    void RotateTowardsPlayer()
    {
        Vector3 scale = transform.localScale;
        scale.x = (player.position.x < transform.position.x) ? -1 : 1; // Xoay theo vị trí nhân vật
        transform.localScale = scale;
    }

    void MoveBetweenPoints()
    {
        if (!isAttacking)
        {
            Vector2 newPosition = Vector2.MoveTowards(transform.position, new Vector2(targetPoint.position.x, transform.position.y), speed * Time.deltaTime);
            transform.position = newPosition;

            if (Mathf.Abs(transform.position.x - targetPoint.position.x) < 0.1f)
            {
                targetPoint = targetPoint == pointA ? pointB : pointA;
            }

            transform.localScale = new Vector3(targetPoint.position.x < transform.position.x ? -1 : 1, 1, 1);
        }
    }

    void StartAttack1()
    {
        isAttacking = true;
        animator.SetBool("Attack1", true);
        attackCounter++;
        lastAttackTime = Time.time;
        StartCoroutine(ResetAttackAnimation("Attack1"));
        StartCoroutine(EndAttack());
    }

    void StartAttack2()
    {
        isAttacking = true;
        animator.SetBool("Attack2", true);
        attackCounter = 0;
        lastAttackTime = Time.time;
        StartCoroutine(PerformAttack2());
    }

    IEnumerator PerformAttack2()
    {
        yield return new WaitForSeconds(0.5f);
        ShootLightning();
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(ResetAttackAnimation("Attack2"));
        StartCoroutine(EndAttack());
    }

    void ShootLightning()
    {
        if (firePoint != null && lightningPrefab != null)
        {
            Vector2 shootDirection = (player.position - firePoint.position).normalized;

            GameObject lightning = Instantiate(lightningPrefab, firePoint.position, Quaternion.identity);
            Rigidbody2D rb = lightning.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = shootDirection * lightningSpeed;
            }
        }
    }

    IEnumerator ResetAttackAnimation(string attackParameter)
    {
        yield return new WaitForSeconds(0.5f);
        animator.SetBool(attackParameter, false);
    }

    IEnumerator EndAttack()
    {
        yield return new WaitForSeconds(1f);
        isAttacking = false;
    }

    public void Die()
    {
        if (isDead) return;

        isDead = true;
        isAttacking = false;
        animator.SetBool("isWalking", false);
        animator.SetBool("isRunning", false);
        animator.SetTrigger("Die");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("bulletpow"))
        {
            StartCoroutine(PauseBossForSeconds(3f));
        }
        if (collision.CompareTag("Bullet") || collision.CompareTag("Sword"))
        {
            hpboss1.value -= 2;
            audioManager.Instance.PlaySFX("matmau");

            if (hpboss1.value <= 0)
            {
                Destroy(thanhmau1);
                hpboss2.value -= 2;

                if (hpboss2.value <= 0)
                {
                    StartCoroutine(WaitForDeathAnimation());
                }
            }
        }
    }

    IEnumerator WaitForDeathAnimation()
    {
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
        Destroy(thanhmau2);
        cong3win.SetActive(true);
    }

    IEnumerator PauseBossForSeconds(float duration)
    {
        isPaused = true;
        animator.SetBool("isWalking", false);
        animator.SetBool("Attack1", false);
        animator.SetBool("Attack2", false);
        yield return new WaitForSeconds(duration);
        isPaused = false;
    }
}