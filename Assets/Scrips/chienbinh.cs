﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class chienbinh : MonoBehaviour
{
    private bool isPaused = false; // Biến để kiểm tra boss có bị dừng hay không

    public GameObject thanhmau1;
    public GameObject thanhmau2;
    public Slider hpboss1;
    public Slider hpboss2;
    public Image fillhp;
    public Image fillhp2;
    public float hp1 = 10;
    public float hp2 = 10;

    #region Public Variables
    public float attackDistance; // Khoảng cách tấn công
    public float moveSpeed; // Tốc độ di chuyển
    public float timer; // Thời gian hồi chiêu
    public Transform leftLimit; // Giới hạn trái
    public Transform rightLimit; // Giới hạn phải
    [HideInInspector] public Transform target; // Mục tiêu hiện tại
    [HideInInspector] public bool inRange; // Kẻ thù trong phạm vi
    public GameObject hotZone;
    public GameObject triggerArea;
    #endregion

    #region Private Variables
    private Animator anim;
    private float distance;
    private bool attackMode;
    private bool cooling;
    private float intTimer;
    private float defaultAttackInterval = 2f; // Thời gian mặc định giữa các lần tấn công
    #endregion

    void Awake()
    {
        anim = GetComponent<Animator>();
        intTimer = timer;
        SelectTarget();
        hpboss1.maxValue = hp1;
        hpboss1.value = hp1;
        hpboss1.interactable = false;
        hpboss2.maxValue = hp2;
        hpboss2.value = hp2;
        hpboss2.interactable = false;
    }

    void Update()
    {
        if (isPaused) return;

        if (!attackMode)
        {
            Move();
        }

        if (!InsideofLimits() && !inRange && !anim.GetCurrentAnimatorStateInfo(0).IsName("Enemy_Attack"))
        {
            SelectTarget();
        }

        if (inRange)
        {
            EnemyLogic();
        }
    }

    void Move()
    {
        anim.SetBool("isWalking", true); // Kích hoạt hoạt ảnh đi lại

        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Enemy_Attack")) // Kiểm tra không phải trạng thái tấn công
        {
            if (target != null) // Kiểm tra mục tiêu hợp lệ
            {
                // Di chuyển Boss tới mục tiêu trên trục X
                Vector2 targetPosition = new Vector2(target.position.x, transform.position.y);
                transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

                // Gọi hàm Flip() để điều chỉnh hướng
                Flip();
            }
        }
    }

    void EnemyLogic()
    {
        if (target == null) return; // Kiểm tra null

        distance = Vector2.Distance(transform.position, target.position);
        if (distance > attackDistance)
        {
            StopAttack();
        }
        else if (distance <= attackDistance && !cooling)
        {
            Attack();
        }

        if (cooling)
        {
            Cooldown();
            anim.SetBool("Attack", false);
        }
    }

    void Attack()
    {
        // Reset thời gian giữa các lần tấn công
        timer = defaultAttackInterval;

        attackMode = true;
        anim.SetBool("isWalking", false);

        anim.SetBool("Attack", true); // Sử dụng chiêu 1
    }

    void Cooldown()
    {
        timer -= Time.deltaTime; // Đếm ngược thời gian hồi chiêu
        if (timer <= 0 && cooling && attackMode)
        {
            cooling = false; // Kết thúc thời gian hồi chiêu
            timer = intTimer;
        }
    }

    void StopAttack()
    {
        cooling = false;
        attackMode = false;
        anim.SetBool("Attack", false);
    }

    public void TriggerCooling()
    {
        cooling = true; // Kích hoạt trạng thái cooling
    }

    private bool InsideofLimits()
    {
        return transform.position.x > leftLimit.position.x && transform.position.x < rightLimit.position.x;
    }

    public void SelectTarget()
    {
        if (leftLimit == null || rightLimit == null) return;

        float distanceToLeft = Vector3.Distance(transform.position, leftLimit.position);
        float distanceToRight = Vector3.Distance(transform.position, rightLimit.position);

        target = (distanceToLeft > distanceToRight) ? leftLimit : rightLimit;

        if (target != null) Flip();
        else Debug.LogWarning("Target is null; cannot flip!");
    }

    public void Flip()
    {
        if (target == null) return; // Kiểm tra null trước khi quay

        Vector3 rotation = transform.eulerAngles;
        if (transform.position.x > target.position.x)
        {
            rotation.y = 180;
        }
        else
        {
            rotation.y = 0;
        }
        transform.eulerAngles = rotation;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("bulletpow"))
        {
            StartCoroutine(PauseBossForSeconds(3f)); // Tạm dừng boss trong 3 giây
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
        Destroy(gameObject);  // Destroy the boss
        Destroy(thanhmau2);
    }

    IEnumerator PauseBossForSeconds(float duration)
    {
        isPaused = true; // Dừng boss
        anim.SetBool("isWalking", false); // Tắt hoạt ảnh đi lại
        anim.SetBool("Attack", false); // Tắt hoạt ảnh tấn công
        yield return new WaitForSeconds(duration); // Đợi thời gian được chỉ định
        isPaused = false; // Tiếp tục boss
    }
}
