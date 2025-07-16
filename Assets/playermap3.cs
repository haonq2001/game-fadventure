using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class playermap3 : MonoBehaviour
{

    [SerializeField] private PhysicsMaterial2D slideMaterial; // Gán trực tiếp từ Inspector
    private PhysicsMaterial2D originalMaterial;
    public TMPro.TextMeshProUGUI cooldownText2; // TextMeshPro hiển thị thời gian đếm ngược
    public GameObject skill3;
    public GameObject skill3mo;
    public GameObject skill1;
    public GameObject skill1mo;
    public TMPro.TextMeshProUGUI cooldownText1; // TextMeshPro hiển thị thời gian đếm ngược

    public gamemanagermap3 gameManager;

    public Slider playerHealth;
    public Image fillImage;
    public Slider playerMana;
    public Image fillImagemana;
    bool isCrouching = false;  // Biến kiểm tra trạng thái crouch

    public GameObject hoimau;
    public GameObject hoimaumo;
    public TMPro.TextMeshProUGUI cooldownText; // TextMeshPro hiển thị thời gian đếm ngược
    private bool isSkill2CoolingDown = false;
    public GameObject boxattack;
    public ParticleSystem healEffect; // Tham chiếu đến hiệu ứng hồi máu
    public ParticleSystem manaEffect; // Tham chiếu đến hiệu ứng hồi máu
    //     public GameObject boxskill3;

    public PushEnemy pushEnemy;  // Tham chiếu đến script PushEnemy

    public float moveSpeed = 15f;

    public float moveNgang;
    public float moveDoc;
    public Vector2 movement;
    public Joystick joystick;

    private bool isFirstUse = true; // Cờ kiểm tra lần đầu sử dụng

    private bool isFirstUse1 = true; // Cờ kiểm tra lần đầu sử dụng
    private bool isFirstUse2 = true; // Cờ kiểm tra lần đầu sử dụng
    // am thanh
    public List<AudioClip> audioClips;
    private AudioSource audioSource;

    public audioManager audioManager;


    private EnemyController enemyController;

    public float jumpForce = 2f;
    private Rigidbody2D rb;
    private bool isGrounded;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    public GameObject bullet;
    public Transform bulletPos;
    public static bool facingRight = true;
    private float castCooldown = 3f;
    private float Cooldownhoimau = 15f;
    private float lastCastTime = 0f;
    private float Cooldownkhien = 15f;
    public GameObject dungkhien;
    public int torchCount = 0;  // Biến để lưu trữ số ngọn lửa (hoặc đuốc) của người chơi
    public int health;
    public int mana;

    bool isSliding = false;
    public Image[] buttonImages; // Biến để tham chiếu đến Image của button


    public GameObject knockbackEffect; // Gán prefab từ Unity Editor
    void Start()
    {
        tathieuunghoimau();
        tathieuunghoimana();

        hoimau.SetActive(true);
        hoimaumo.SetActive(false);

        skill1.SetActive(true);
        skill1mo.SetActive(false);
        originalMaterial = GetComponent<Collider2D>().sharedMaterial;


        // Tìm đối tượng EnemyController trong scene (hoặc tham chiếu qua Inspector)
        enemyController = FindObjectOfType<EnemyController>();





        //  boxCollider2D = GetComponent<PolygonCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>(); // Gán Animator từ component

        skill3mo.SetActive(false);
        skill3.SetActive(true);

        boxattack.SetActive(false);

        dungkhien.SetActive(false);



        playerHealth.interactable = false;
        playerMana.interactable = false;
        playerHealth.maxValue = health;

        playerMana.maxValue = mana;
        playerHealth.value = Mathf.Clamp(health, 0, playerHealth.maxValue);
        playerMana.value = Mathf.Clamp(mana, 0, playerMana.maxValue);

        audioSource = GetComponent<AudioSource>();
        Time.timeScale = 1;
    }
    // bh fix loi xong them chu d vao
    public void bathieuunghoimau()
    {
        if (playerHealth.value < 50 && isCrouching)
        {
            healEffect.Play();
        }
        else
        {
            healEffect.Stop();
        }
    }

    public void tathieuunghoimau()
    {
        healEffect.Stop();
    }
    public void bathieuunghoimana()
    {
        if (playerMana.value < 50 && isCrouching)
        {
            manaEffect.Play();
        }
        else
        {
            manaEffect.Stop();
        }
    }

    public void tathieuunghoimana()
    {
        manaEffect.Stop();
    }
    void Update()
    {
        if (isSliding)
        {
            // Skip other actions if sliding
            return;
        }

        Move();
        Jump();
        Attack();
        sliding();
        keyhp();
        keymp();

        TryCastSpell(KeyCode.Q, 0);
        TryCastSpell(KeyCode.R, 14);
        TryCastSpell(KeyCode.F, 4);

        // Update all buttons based on mana
        foreach (Image buttonImage in buttonImages)
        {
            UpdateButton(buttonImage);
        }
    }

    private void TryCastSpell(KeyCode key, int manaCost)
    {
        if (playerMana.value > manaCost)
        {
            if (Input.GetKeyDown(key))
            {
                if (key == KeyCode.R)
                {
                    Attack1(); // Call Attack1
                }
                else if (key == KeyCode.F)
                {
                    Attack3(); // Call Attack3
                }
                else if (key == KeyCode.Q)
                {
                    Cast(); // Call Cast
                }
            }
        }
        else
        {
            Debug.Log($"Không đủ năng lượng để sử dụng kĩ năng với phím {key}");
        }
    }


    void sliding()
    {
        if (Input.GetKeyDown(KeyCode.Tab)) // Kích hoạt trượt
        {
            animator.SetTrigger("dash");
            StartCoroutine(SlideMovement());
            //   StartSliding();

        }
    }
    public void StartSliding()
    {
        Debug.Log("Nhân vật bắt đầu trượt!");
        // Gọi hàm bật collider vật lý của quái
        enemyController.EnablePhysicalCollider();
    }

    public void StopSliding()
    {
        Debug.Log("Nhân vật dừng trượt!");
        // Gọi hàm tắt collider vật lý của quái
        enemyController.DisablePhysicalCollider();
    }

    private IEnumerator SlideMovement()
    {
        // Cài đặt trượt
        float slideForce = 10f;   // Lực trượt
        float slideDuration = 0.3f; // Thời gian trượt
        float direction = facingRight ? 1 : -1; // Hướng trượt (dựa vào nhân vật)

        // Thay đổi vật liệu ma sát thấp khi trượt
        Collider2D collider = GetComponent<Collider2D>();
        PhysicsMaterial2D originalMaterial = collider.sharedMaterial; // Lưu vật liệu cũ
        PhysicsMaterial2D slideMaterial = Resources.Load<PhysicsMaterial2D>("SlideMaterial");
        collider.sharedMaterial = slideMaterial;

        isSliding = true;

        // Áp dụng lực trượt
        rb.AddForce(new Vector2(slideForce * direction, 0), ForceMode2D.Impulse);

        // Đợi trượt hoàn thành
        yield return new WaitForSeconds(slideDuration);

        // Dừng trượt và phục hồi trạng thái
        rb.velocity = new Vector2(0, rb.velocity.y); // Dừng di chuyển ngang
        collider.sharedMaterial = originalMaterial; // Khôi phục vật liệu ban đầu
        isSliding = false;
    }


    IEnumerator FlashEnemy(GameObject enemy)
    {
        // Hiệu ứng nổ
        if (knockbackEffect != null)
        {
            Instantiate(knockbackEffect, enemy.transform.position, Quaternion.identity);
        }

        // Flash màu đỏ
        SpriteRenderer sr = enemy.GetComponent<SpriteRenderer>();
        sr.color = Color.red;
        yield return new WaitForSeconds(0.2f);
        sr.color = Color.white;
    }




    void UpdateButton(Image button)
    {
        if (playerMana.value <= 0)
        {
            Color color = button.color;  // Truy cập thuộc tính color của Image
            color.a = 0.5f; // Làm mờ button
            button.color = color;
            button.GetComponent<Button>().interactable = false; // Tắt button
        }
        else
        {
            Color color = button.color;  // Truy cập thuộc tính color của Image
            color.a = 1f; // Đặt lại độ sáng
            button.color = color;
            button.GetComponent<Button>().interactable = true; // Bật lại button
        }
    }

    public void PlayerShoot()
    {
        GameObject newBullet = Instantiate(bullet, bulletPos.position, Quaternion.identity);
        Bullet bulletScript = newBullet.GetComponent<Bullet>();

        Debug.Log("Player Facing Right: " + facingRight);

        if (!facingRight)
        {
            bulletScript.SetDirection(-1);
        }
        else
        {
            bulletScript.SetDirection(1);
        }
    }

    void Attack()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            animator.SetTrigger("attack");
            isCrouching = false;
            audioManager.Instance.PlaySFX("skill1");
            StartCoroutine(boxvukhi());
        }
    }
    public void buttonattack()
    {
        animator.SetTrigger("attack");
        isCrouching = false;
        audioManager.Instance.PlaySFX("skill1");
        StartCoroutine(boxvukhi());
    }
    IEnumerator boxvukhi()
    {
        boxattack.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        boxattack.SetActive(false);
    }

    void Attack1()
    {
        if (isFirstUse1 || Time.time - lastCastTime >= Cooldownhoimau && isGrounded)
        {
            isCrouching = true;
            animator.SetTrigger("crouch");
            playerMana.value -= 15;
            audioManager.Instance.PlaySFX("kiemchem");
            lastCastTime = Time.time; // Lưu thời gian sử dụng
            isFirstUse1 = false; // Đánh dấu không còn lần đầu nữa
            StartCoroutine(thoigianhoimau());
            bathieuunghoimau();
            bathieuunghoimana();

        }



    }



    public void hoimaumna()
    {

        if (playerHealth.value < 50) // Chỉ hồi máu nếu chưa đầy
        {
            StartCoroutine(HealOverTime());
        }
        else
        {
            print("Máu đã đầy");
        }

        if (playerMana.value < 50) // Chỉ hồi mana nếu chưa đầy
        {
            StartCoroutine(ManaOverTime());
        }
        else
        {
            print("Mana đã đầy");
        }

        PlayerPrefs.SetInt("PlayerMana", mana);

    }




    IEnumerator HealOverTime()
    {

        float healDuration = 5f; // Thời gian hồi máu (5 giây)
        float healAmount = 10f; // Số lượng máu cần hồi trong 5 giây
        float healPerSecond = healAmount / healDuration; // Hồi bao nhiêu máu mỗi giây
        float elapsedTimehp = 0f; // Thời gian đã trôi qua

        // Kiểm tra nếu crouch, không thể hồi máu
        while (playerHealth.value < 50 && elapsedTimehp < healDuration && isCrouching)
        {

            playerHealth.value += healPerSecond * Time.deltaTime;
            elapsedTimehp += Time.deltaTime;
            if (playerHealth.value >= 50)
            {
                tathieuunghoimau(); // Dừng hiệu ứng nếu máu đầy
            }
            print("Hồi máu: " + playerHealth.value);
       //     audioManager.Instance.PlaySFX("hoimau");

            // Cập nhật màu thanh máu
            if (playerHealth.value >= 50)
            {
                fillImage.color = Color.red;
            }
            else if (playerHealth.value < 40)
            {
                fillImage.color = Color.red;
            }
            else
            {
                fillImage.color = Color.red;
            }

            yield return null;
        }

        // Đảm bảo không vượt quá số lượng cần hồi
        playerHealth.value = Mathf.Min(playerHealth.value, playerHealth.value + healAmount);
    }

    IEnumerator ManaOverTime()
    {
        float manaDuration = 5f; // Thời gian hồi mana (5 giây)
        float manaAmount = 10f; // Số lượng mana cần hồi trong 5 giây
        float manaPerSecond = manaAmount / manaDuration; // Hồi bao nhiêu mana mỗi giây
        float elapsedTime = 0f; // Thời gian đã trôi qua

        // Kiểm tra nếu crouch, không thể hồi mana
        while (elapsedTime < manaDuration && playerMana.value < 50 && isCrouching)
        {
            playerMana.value += manaPerSecond * Time.deltaTime;
            elapsedTime += Time.deltaTime;
            if (playerMana.value >= 50)
            {
                tathieuunghoimana(); // Dừng hiệu ứng nếu mana đầy
            }

            print("Hồi mana: " + playerMana.value);
        //    audioManager.Instance.PlaySFX("hoimau");

            yield return null;
        }

        // Đảm bảo mana không vượt quá số lượng cần hồi
        playerMana.value = Mathf.Min(playerMana.value, playerMana.value + manaAmount);
    }





    void Attack3()
    {
        // Nếu là lần đầu sử dụng, cho phép ngay
        if (isFirstUse2 || Time.time - lastCastTime >= Cooldownkhien)
        {
            playerMana.value -= 5; // Giảm mana khi sử dụng
            audioManager.Instance.PlaySFX("skill2"); // Phát âm thanh
            lastCastTime = Time.time; // Lưu thời gian sử dụng
            isFirstUse2 = false; // Đánh dấu không còn lần đầu nữa
            StartCoroutine(thoigiandungkhien()); // Bắt đầu logic hiệu ứng
            StartCoroutine(DisableButtonForCooldown2()); // Hiển thị hồi chiêu
        }
        else
        {
            Debug.Log("Skill đang hồi chiêu!");
        }
    }
    IEnumerator thoigiandungkhien()
    {
        dungkhien.SetActive(true);
        yield return new WaitForSeconds(5f);
        dungkhien.SetActive(false);

    }
    IEnumerator DisableButtonForCooldown2()
    {
        skill1mo.GetComponent<Button>().interactable = false;
        skill1.SetActive(false);
        skill1mo.SetActive(true);

        int cooldownTime2 = 15;         // Thời gian hồi chiêu (giây)
        while (cooldownTime2 > 0)
        {
            cooldownText2.text = cooldownTime2.ToString() + "s"; // Cập nhật Text đếm ngược
            yield return new WaitForSeconds(1f);         // Chờ 1 giây
            cooldownTime2--;                              // Giảm thời gian
        }

        cooldownText2.text = "";        // Xóa Text sau khi đếm ngược xong
        skill1.GetComponent<Button>().interactable = true; // Cho phép tương tác trở lại
        skill1mo.SetActive(false);     // Ẩn button mờ
        skill1.SetActive(true);
    }
    public void buttonattack1()
    {
        if (isFirstUse1 || playerMana.value > 14 && isGrounded)
        {
            isCrouching = true;
            animator.SetTrigger("crouch");
            playerMana.value -= 15;
            audioManager.Instance.PlaySFX("kiemchem");
            isFirstUse1 = false;
            StartCoroutine(thoigianhoimau());
            bathieuunghoimau();
            bathieuunghoimana();

            PlayerPrefs.SetInt("PlayerMana", mana);
        }
        else
        {
            Debug.Log("Không đủ mana để sử dụng kĩ năng!");
        }
    }

    IEnumerator thoigianhoimau()
    {
        // Giữ button hiển thị nhưng không thể tương tác
        // Đặt cờ hồi chiêu
        isSkill2CoolingDown = true;
        hoimaumo.GetComponent<Button>().interactable = false;
        hoimau.SetActive(false);
        hoimaumo.SetActive(true);

        int cooldownTime = 15;         // Thời gian hồi chiêu (giây)
        while (cooldownTime > 0)
        {
            cooldownText.text = cooldownTime.ToString() + "s"; // Cập nhật Text đếm ngược
            yield return new WaitForSeconds(1f);         // Chờ 1 giây
            cooldownTime--;                              // Giảm thời gian
        }

        cooldownText.text = "";        // Xóa Text sau khi đếm ngược xong
        hoimau.GetComponent<Button>().interactable = true; // Cho phép tương tác trở lại
        hoimaumo.SetActive(false);     // Ẩn button mờ
        hoimau.SetActive(true);
        // ket thuc hoi chieu
        isSkill2CoolingDown = false;
    }

    public void buttonattack2()
    {
        if (isFirstUse || playerMana.value > 0 && Time.time - lastCastTime >= castCooldown)
        {
            animator.SetTrigger("cast");
            isCrouching = false;
            playerMana.value -= 2;
            lastCastTime = Time.time;
            isFirstUse = false;
            audioManager.Instance.PlaySFX("bandan");
            PlayerPrefs.SetInt("PlayerMana", mana);

            // Làm mờ button và chạy cooldown
            StartCoroutine(DisableButtonForCooldown());

        }
        else
        {
            //  Debug.Log("Không đủ mana để sử dụng kĩ năng!");
        }
    }

    IEnumerator DisableButtonForCooldown()
    {
        skill3mo.GetComponent<Button>().interactable = false;
        skill3.SetActive(false);
        skill3mo.SetActive(true);

        int cooldownTime1 = 3;         // Thời gian hồi chiêu (giây)
        while (cooldownTime1 > 0)
        {
            cooldownText1.text = cooldownTime1.ToString() + "s"; // Cập nhật Text đếm ngược
            yield return new WaitForSeconds(1f);         // Chờ 1 giây
            cooldownTime1--;                              // Giảm thời gian
        }

        cooldownText1.text = "";        // Xóa Text sau khi đếm ngược xong
        skill3.GetComponent<Button>().interactable = true; // Cho phép tương tác trở lại
        skill3mo.SetActive(false);     // Ẩn button mờ
        skill3.SetActive(true);
    }

    public void buttonattack3()
    {
        if (playerMana.value > 4 || isFirstUse2)
        {
            // animator.SetTrigger("strike");
            playerMana.value -= 5;
            isFirstUse2 = false; // Đánh dấu không còn lần đầu nữa
            audioManager.Instance.PlaySFX("skill2");
            StartCoroutine(thoigiandungkhien());
            StartCoroutine(DisableButtonForCooldown2());
        }
        else
        {
            Debug.Log("Không đủ mana để sử dụng kĩ năng!");
        }
    }
    private void UpdateSwordColliderPosition()
    {
        // Đổi hướng dựa trên `facingRight`
        if (facingRight)
        {
            // Hướng sang phải
            boxattack.transform.localPosition = new Vector3(1.0f, boxattack.transform.localPosition.y, boxattack.transform.localPosition.z);
            boxattack.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f); // Scale mặc định



            //   boxskill3.transform.localPosition = new Vector3(1.0f, boxskill3.transform.localPosition.y, boxskill3.transform.localPosition.z);
            //   boxskill3.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f); // Scale mặc định
        }
        else
        {
            // Hướng sang trái
            boxattack.transform.localPosition = new Vector3(-1.0f, boxattack.transform.localPosition.y, boxattack.transform.localPosition.z);
            boxattack.transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f); // Lật đối tượng theo trục X
                                                                             // Có thể lật góc của collider nếu cần
                                                                             // boxattack.transform.localRotation = Quaternion.Euler(0, 180, 0); // Quay boxattack 180 độ trên trục Y



            //    boxskill3.transform.localPosition = new Vector3(-1.0f, boxskill3.transform.localPosition.y, boxskill3.transform.localPosition.z);
            //   boxskill3.transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f); 
        }
    }




    void Move()
    {
        // Lấy giá trị từ joystick và bàn phím
        float moveInput = joystick.Horizontal + Input.GetAxis("Horizontal");

        // Ngưỡng để bỏ qua giá trị rất nhỏ từ joystick
        if (Mathf.Abs(moveInput) > 0.1f)
        {
            rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);

            // Đổi hướng nhân vật
            if (moveInput < 0)
            {
                spriteRenderer.flipX = true;
                facingRight = false;
                UpdateSwordColliderPosition();
            }
            else if (moveInput > 0)
            {
                spriteRenderer.flipX = false;
                facingRight = true;
                UpdateSwordColliderPosition();
            }

            animator.SetBool("isWalking", isGrounded); // Animation "đi bộ" chỉ kích hoạt nếu đang chạm đất
            isCrouching = false;
            tathieuunghoimana();
            tathieuunghoimau();
        }
        else
        {
            // Chỉ dừng vận tốc ngang nếu đang chạm đất
            if (isGrounded)
            {
                rb.velocity = new Vector2(0, rb.velocity.y);
                animator.SetBool("isWalking", false); // Ngừng animation đi bộ
            }
        }
    }

    void Jump()
    {
        if ((joystick.Vertical > 0.5f || Input.GetButtonDown("Jump")) && isGrounded)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            isGrounded = false; // Đặt trạng thái "ở trên không"
            animator.SetTrigger("Jump");
            isCrouching = false;
            tathieuunghoimana();
            tathieuunghoimau();

            audioManager.Instance.PlaySFX("jump");
        }
    }

    void Cast()
    {
        if (isFirstUse || Time.time - lastCastTime >= castCooldown)
        {
            isCrouching = false;
            tathieuunghoimana();
            tathieuunghoimau();
            StartCoroutine(DisableButtonForCooldown());
            animator.SetTrigger("cast");
            playerMana.value -= 2;
            PlayerPrefs.SetInt("PlayerMana", mana);
            lastCastTime = Time.time;
            isFirstUse = false;
            audioManager.Instance.PlaySFX("bandan");
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.gameObject.CompareTag("Enemy") && isSliding)
        {
            // Đẩy quái
            // Lấy direction từ nhân vật đến quái vật
            Vector2 pushDirection = (collision.transform.position - transform.position).normalized;

            // Gọi hàm PushEnemyForward để đẩy quái vật
            pushEnemy.PushEnemyForward(pushDirection);
            //Rigidbody2D enemyRb = collision.gameObject.GetComponent<Rigidbody2D>();
            //if (enemyRb != null)
            //{
            //    float knockbackForce = 5f;
            //    Vector2 knockbackDirection = collision.transform.position.x > transform.position.x ? Vector2.right : Vector2.left;
            //    enemyRb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);

            // Hiệu ứng
            StartCoroutine(FlashEnemy(collision.gameObject));
            //   audioManager.Instance.PlaySFX("knockback");
            // }
        }
        if (collision.gameObject.CompareTag("Ground"))
        {
            foreach (ContactPoint2D contact in collision.contacts)
            {
                if (contact.normal.y > 0.5f) // chỉ chạm mặt đất dưới chân
                {
                    isGrounded = true;
                    Debug.Log("Chạm đất!");
                    animator.SetBool("IsGround", true);
                }
            }
                    if (!isSkill2CoolingDown)
            {
                cooldownText.text = ""; // Đảm bảo Text không hiển thị nếu không hồi chiêu
                hoimau.SetActive(true);
                hoimaumo.SetActive(false);
            }


        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (isSliding)
        {
            StopCoroutine(SlideMovement());
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            rb.velocity = Vector2.zero; // Dừng di chuyển
        }
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
            animator.SetBool("IsGround", false);
            tathieuunghoimana();
            tathieuunghoimau();
            if (!isSkill2CoolingDown)
            {
                hoimau.SetActive(false);
                hoimaumo.SetActive(true);
            }


        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.gameObject.layer == LayerMask.NameToLayer("trap"))
        {
            isCrouching = false;
            tathieuunghoimana();
            tathieuunghoimau();
            animator.SetTrigger("dizzy");
            audioManager.Instance.PlaySFX("bidau");
            PlayerPrefs.SetInt("PlayerHealth", health);
            playerHealth.value -= 1;
            print("cham quai, -1 mau");
            if (playerHealth.value < 8)
            {
                fillImage.color = Color.yellow;

                if (playerHealth.value < 4)
                {
                    fillImage.color = Color.red;

                }
                if (playerHealth.value == 0)
                {
                    animator.SetTrigger("die");
                    StartCoroutine(WaitForDeathAnimation());
                    audioManager.Instance.PlaySFX("chet");
                    ;
                };
            }

        }

        if (collision.gameObject.CompareTag("vukhi_enemy"))
        {
            isCrouching = false;
            tathieuunghoimana();
            tathieuunghoimau();
            animator.SetTrigger("hurt");
            audioManager.Instance.PlaySFX("bidau");
            playerHealth.value -= 2;

            print("cham quai, -1 mau");
            if (playerHealth.value < 8)
            {
                fillImage.color = Color.red;

                if (playerHealth.value < 4)
                {
                    fillImage.color = Color.red;

                }
                if (playerHealth.value == 0)
                {
                    animator.SetTrigger("die");
                    StartCoroutine(WaitForDeathAnimation());
                    audioManager.Instance.PlaySFX("chet");
                    ;
                };


            }

        }
        if (collision.gameObject.CompareTag("vukhi_enemy_man3"))
        {
            isCrouching = false;
            tathieuunghoimana();
            tathieuunghoimau();
            animator.SetTrigger("hurt");
            audioManager.Instance.PlaySFX("bidau");
            playerHealth.value -= 7;

            print("cham quai, -7 mau");
            if (playerHealth.value < 8)
            {
                fillImage.color = Color.red;

                if (playerHealth.value < 4)
                {
                    fillImage.color = Color.red;

                }
                if (playerHealth.value == 0)
                {
                    animator.SetTrigger("die");
                    StartCoroutine(WaitForDeathAnimation());
                    audioManager.Instance.PlaySFX("chet");
                    ;
                };


            }

        }
        if (collision.gameObject.CompareTag("ngonlua"))
        {
            StartCoroutine(DestroyTorchAfterDelay(collision.gameObject));
            //       gameManager.AddScore();
            //       gameManager.SetScoreText();
            torchCount++;
            audioManager.Instance.PlaySFX("anlua");
            Debug.Log("Bạn vừa nhận được ngọn đuốc");
            // Gọi hàm trong CotDuocManager để bật panel settings
            CotDuocManager cotDuocManager = FindObjectOfType<CotDuocManager>();
            if (cotDuocManager != null)
            {
                cotDuocManager.ActivatePanel();  // Bật panel settings
            }

        }

        if (collision.gameObject.CompareTag("winnhe"))
        {
            if (gameManager != null)
            {
                gameManager.OnMap3Complete();
                audioManager.Instance.PlaySFX("chienthang");
                
            }
            else
            {
                Debug.LogError("GameManager is null. Please check your setup!");
            }
        }
    }

        IEnumerator WaitForDeathAnimation()
    {
        yield return new WaitForSeconds(0.5f);
        Time.timeScale = 0;
        gameManager.GameOver();
        PlayerPrefs.DeleteAll(); // Xóa dữ liệu lưu
    }
    IEnumerator DestroyTorchAfterDelay(GameObject torch)
    {
        yield return new WaitForSeconds(0.3f);
        Destroy(torch);
    }



    IEnumerator HealWithDelay()
    {
        yield return new WaitForSeconds(2f); // Chờ 1 giây
        audioManager.Instance.PlaySFX("hoimau");
        // Cộng máu theo mức độ
        if (playerHealth.value > 48 && playerHealth.value < 50) // Cộng 1 máu
        {
            playerHealth.value += 1;
            print("+1 máu");
        }
        else if (playerHealth.value > 47 && playerHealth.value < 49) // Cộng 2 máu
        {
            playerHealth.value += 2;
            print("+2 máu");
        }
        else if (playerHealth.value <= 47) // Cộng 3 máu
        {
            playerHealth.value += 10;
            print("+3 máu");
        }


        // Giới hạn HP không vượt quá 10
        if (playerHealth.value > 50)
        {
            playerHealth.value = 10;
        }

      
    }
    void keyhp()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            sudungbinhhp();
        }
    }
    void keymp()
    {
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            sudungbinhmp();
        }
    }
    public void sudungbinhhp()
    {
        if (playerHealth.value < 50 && gameManager.scorehp > 0) // Kiểm tra máu và số lượng bình HP
        {
            gameManager.BlockScorehp(); // Giảm số lượng bình HP
            gameManager.SetScoreTexthp(); // Cập nhật UI
            StartCoroutine(HealWithDelay()); // Hồi máu
        }
        else if (gameManager.scorehp <= 0)
        {
            Debug.Log("Không còn bình HP để sử dụng");
        }
        else
        {
            Debug.Log("Máu đã đầy, không thể sử dụng bình HP");
        }
    }




    IEnumerator ManaWithDelay()
    {
        yield return new WaitForSeconds(2f); // Chờ 1 giây
        audioManager.Instance.PlaySFX("hoimau");

        if (playerMana.value < 50)
        {
            playerMana.value = Mathf.Min(50, playerMana.value + 10); // Cộng tối đa 3 MP
            print("+10 mp");
        }
    }


    public void sudungbinhmp()
    {
        if (playerMana.value < 50 && gameManager.scoremp > 0) // Kiểm tra mana và số lượng bình MP
        {
            gameManager.BlockScoremp(); // Giảm số lượng bình MP
            gameManager.SetScoreTextmp(); // Cập nhật UI
            StartCoroutine(ManaWithDelay()); // Hồi mana
        }
        else if (gameManager.scoremp <= 0)
        {
            Debug.Log("Không còn bình MP để sử dụng");
        }
        else
        {
            Debug.Log("Mana đã đầy, không thể sử dụng bình MP");
        }

    }
}