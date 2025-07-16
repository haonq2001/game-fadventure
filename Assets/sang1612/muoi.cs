using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Mosquito : MonoBehaviour
{
    public float startX = -5f; // Điểm đầu (trục X)
    public float endX = 5f; // Điểm cuối (trục X)
    public float maxY = 5f; // Chiều cao tối đa (trục Y)
    public float minY = 0f; // Chiều cao tối thiểu (trục Y)
    public float speed = 3f; // Tốc độ di chuyển
    public float waitTime = 1f; // Thời gian dừng ngẫu nhiên giữa các lần di chuyển
    public float detectionRadius = 5f; // Bán kính phát hiện người chơi
    public LayerMask playerLayer; // Layer để kiểm tra Player
    public float attackCooldown = 2f; // Thời gian chờ giữa các lần tấn công

    public int maxHealth = 100; // Máu tối đa
    private int currentHealth; // Máu hiện tại
    public Slider BossHealth;
    private Vector3 targetPosition; // Vị trí mục tiêu
    private Animator animator; // Animator để điều khiển animation
    private SpriteRenderer spriteRenderer; // SpriteRenderer để quay đầu
    private bool isDead = false; // Trạng thái chết
    private bool isAttacking = false; // Trạng thái tấn công

    private void Start()
    {
        // Lấy các component
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Khởi tạo máu
        currentHealth = maxHealth;
        BossHealth.maxValue = maxHealth;
        BossHealth.value = maxHealth;

        // Bắt đầu di chuyển ngẫu nhiên
        StartCoroutine(RandomMovement());
    }

    private void Update()
    {
        if (!isDead && !isAttacking)
        {
            CheckForPlayer();
        }
    }

    private IEnumerator RandomMovement()
    {
        while (!isDead)
        {
            // Chọn vị trí ngẫu nhiên
            targetPosition = GetRandomPosition();
            animator.SetBool("isFlying", true);

            // Di chuyển đến vị trí
            while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
            {
                if (isDead) yield break;

                // Tính hướng di chuyển
                Vector3 direction = (targetPosition - transform.position).normalized;
                spriteRenderer.flipX = direction.x < 0; // Quay đầu

                // Di chuyển
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
                yield return null;
            }

            animator.SetBool("isFlying", false);
            yield return new WaitForSeconds(waitTime);
        }
    }

    private Vector3 GetRandomPosition()
    {
        float randomX = Random.Range(startX, endX);
        float randomY = Random.Range(minY, maxY);
        return new Vector3(randomX, randomY, transform.position.z);
    }

    private void CheckForPlayer()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectionRadius, playerLayer);

        if (hits.Length == 0)
        {
            isAttacking = false; // Không tìm thấy người chơi, dừng tấn công
            return;
        }

        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                StartCoroutine(AttackPlayer());
                break;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Sword") && !isDead)
        {
            animator.SetTrigger("Hurt");
            TakeDamage(20);
        }
    }

    private IEnumerator AttackPlayer()
    {
        if (isAttacking || isDead) yield break;

        isAttacking = true;

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectionRadius, playerLayer);
        Collider2D targetPlayer = null;

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                targetPlayer = hit;
                break;
            }
        }

        if (targetPlayer != null)
        {
            while (Vector3.Distance(transform.position, targetPlayer.transform.position) > 0.5f)
            {
                Vector3 direction = (targetPlayer.transform.position - transform.position).normalized;
                spriteRenderer.flipX = direction.x < 0;

                transform.position = Vector3.MoveTowards(transform.position, targetPlayer.transform.position, speed * Time.deltaTime);
                yield return null;
            }

            animator.SetTrigger("attack");
            Debug.Log("Mosquito attacks player!");
        }

        yield return new WaitForSeconds(attackCooldown);
        isAttacking = false;
    }

    private void TakeDamage(int damage)
    {
        currentHealth -= damage;
        StartCoroutine(UpdateHealthBar());

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private IEnumerator UpdateHealthBar()
    {
        float elapsedTime = 0f;
        float duration = 0.3f;

        float startValue = BossHealth.value;
        float endValue = currentHealth;

        while (elapsedTime < duration)
        {
            BossHealth.value = Mathf.Lerp(startValue, endValue, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        BossHealth.value = endValue;
    }

    private void Die()
    {
        isDead = true;
        animator.SetTrigger("die");

        GetComponent<Collider2D>().enabled = false;
        StopAllCoroutines();

        Destroy(gameObject, 1f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
