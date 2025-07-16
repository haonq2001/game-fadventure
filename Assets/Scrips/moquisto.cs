using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moquisto : MonoBehaviour
{
    #region Public Variables
    public float attackDistance;
    public float moveSpeed;
    public float timer;
    public Transform leftLimit;
    public Transform rightLimit;
    public Transform topLimit; // New top limit for vertical movement
    public Transform bottomLimit; // New bottom limit for vertical movement
    [HideInInspector] public Transform target;
    [HideInInspector] public bool inRange;
    public GameObject hotZone;
    public GameObject triggerArea;
    #endregion

    #region Private Variables
    private Animator anim;
    private float distance;
    private bool attackMode;
    private bool cooling;
    private float intTimer;
    #endregion

    void Awake()
    {
        anim = GetComponent<Animator>();
        intTimer = timer;
        SelectTarget();
    }

    void Update()
    {
        if (!attackMode)
        {
            Move();
        }

        if (!InsideofLimits() && !inRange && !anim.GetCurrentAnimatorStateInfo(0).IsName("moquisto_attack"))
        {
            SelectTarget();
        }

        if (inRange)
        {
            EnemyLogic();
        }
    }

    void EnemyLogic()
    {
        if (target == null) return;

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
            anim.SetBool("attack", false);
        }
    }

    void Move()
    {
        anim.SetBool("isFlying", true);
        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("moquisto_attack"))
        {
            if (target != null)
            {
                // Move horizontally and vertically within the limits
                Vector2 targetPosition = new Vector2(target.position.x, Random.Range(bottomLimit.position.y, topLimit.position.y));
                transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
                Flip();
            }
        }
    }

    void Attack()
    {
        timer = intTimer;
        attackMode = true;
        anim.SetBool("isFlying", false);
        anim.SetBool("attack", true);
    }

    void Cooldown()
    {
        timer -= Time.deltaTime;
        if (timer < 0 && cooling && attackMode)
        {
            cooling = false;
            timer = intTimer;
        }
    }

    void StopAttack()
    {
        cooling = false;
        attackMode = false;
        anim.SetBool("attack", false);
    }

    public void TriggerCooling()
    {
        cooling = true;
    }

    private bool InsideofLimits()
    {
        return transform.position.x > leftLimit.position.x && transform.position.x < rightLimit.position.x
            && transform.position.y > bottomLimit.position.y && transform.position.y < topLimit.position.y; // Check vertical limits
    }

    public void SelectTarget()
    {
        if (leftLimit == null || rightLimit == null || topLimit == null || bottomLimit == null) return;

        float distanceToLeft = Vector3.Distance(transform.position, leftLimit.position);
        float distanceToRight = Vector3.Distance(transform.position, rightLimit.position);

        // Randomly choose to move up or down to make the movement less predictable
        float distanceToTop = Vector3.Distance(transform.position, topLimit.position);
        float distanceToBottom = Vector3.Distance(transform.position, bottomLimit.position);

        // Select the closest horizontal target
        target = (distanceToLeft > distanceToRight) ? leftLimit : rightLimit;

        // Optionally, randomize vertical position to make the movement more dynamic
        if (Random.value > 0.5f)
            target.position = new Vector3(target.position.x, Random.Range(bottomLimit.position.y, topLimit.position.y), target.position.z);

        if (target != null) Flip();
        else Debug.LogWarning("Target is null; cannot flip!");
    }

    public void Flip()
    {
        if (target == null) return;

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
}
