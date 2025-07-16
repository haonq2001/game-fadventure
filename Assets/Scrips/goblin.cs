using System.Collections;
using UnityEngine;
using UnityEngine.UI; // Thêm thư viện UI

public class goblin : MonoBehaviour
{
    public float startX = -5f; // Điểm đầu trên trục X
    public float endX = 5f; // Điểm cuối trên trục X
    public float speed = 2f; // Tốc độ di chuyển
    public float waitTime = 3f; // Thời gian chờ giữa các lần di chuyển

    public int maxHealth = 3; // Máu tối đa
    private int currentHealth; // Máu hiện tại

    public Slider healthBar; // Thanh máu (Slider)

    private Animator animator; // Animator để điều khiển animation
    private SpriteRenderer spriteRenderer; // Để quay đầu
    private bool isDead = false; // Trạng thái chết

    private Vector3 targetPosition; // Vị trí mục tiêu

    private void Start()
    {
        // Lấy các component
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Khởi tạo máu
        currentHealth = maxHealth;

        // Gán giá trị khởi đầu cho thanh máu
        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
        }

        // Bắt đầu di chuyển ngẫu nhiên
        StartCoroutine(RandomMovement());
    }

    private void Update()
    {
        // Kiểm tra phím Q để gây sát thương
        if (Input.GetKeyDown(KeyCode.Q) && !isDead)
        {
            TakeDamage(1);
        }
    }

    private IEnumerator RandomMovement()
    {
        while (!isDead)
        {
            // Tạo vị trí mục tiêu mới trong vùng giới hạn
            targetPosition = GetRandomPosition();

            // Kích hoạt animation đi bộ
            animator.SetBool("IsWalking", true);

            // Di chuyển đến vị trí mục tiêu
            while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
            {
                if (isDead) yield break; // Nếu chết, dừng di chuyển

                // Tính toán hướng di chuyển
                Vector3 direction = (targetPosition - transform.position).normalized;

                // Quay đầu theo hướng di chuyển
                if (direction.x > 0)
                {
                    spriteRenderer.flipX = false; // Hướng sang phải
                }
                else if (direction.x < 0)
                {
                    spriteRenderer.flipX = true; // Hướng sang trái
                }

                // Di chuyển
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
                yield return null; // Chờ cho đến khung hình tiếp theo
            }

            // Ngừng animation đi bộ khi tới đích
            animator.SetBool("IsWalking", false);

            // Chờ một khoảng thời gian trước khi di chuyển tiếp
            yield return new WaitForSeconds(waitTime);
        }
    }

    private Vector3 GetRandomPosition()
    {
        // Tạo vị trí ngẫu nhiên trong giới hạn startX và endX
        float randomX = Random.Range(startX, endX);
        return new Vector3(randomX, transform.position.y, transform.position.z);
    }

    private void TakeDamage(int damage)
    {
        currentHealth -= damage;

        // Cập nhật thanh máu
        if (healthBar != null)
        {
            healthBar.value = currentHealth;
        }

        // Kích hoạt animation bị thương
        animator.SetTrigger("Hurt");

        if (currentHealth <= 0)
        {
            // Gọi hàm chết khi máu hết
            Die();
        }
    }

    private void Die()
    {
        isDead = true;

        // Kích hoạt animation chết
        animator.SetTrigger("Death");

        // Tắt collider và ngừng mọi hành động
        GetComponent<Collider2D>().enabled = false;
       

        // Dừng mọi hành động khác
        StopAllCoroutines();
        Destroy(gameObject, 1f);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            animator.SetTrigger("Attack");
        }
    }
}
