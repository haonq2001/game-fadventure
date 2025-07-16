﻿using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;



public class playermap2 : MonoBehaviour

{
    private Vector3 originalScale;
    public TMPro.TextMeshProUGUI cooldownText2; // TextMeshPro hiển thị thời gian đếm ngược
    public TMPro.TextMeshProUGUI cooldownText1; // TextMeshPro hiển thị thời gian đếm ngược

    public TMPro.TextMeshProUGUI cooldownText; // TextMeshPro hiển thị thời gian đếm ngược

    // private bool isSkill2CoolingDown = false;



    private bool isFirstUse = true; // Cờ kiểm tra lần đầu sử dụng

    private bool isFirstUse1 = true; // Cờ kiểm tra lần đầu sử dụng




    public GameObject dungkhien;
    public GameObject chiakhoa;

    public GameObject keypostrai;  // Vị trí chìa khóa khi quay trái
    public GameObject keyposphai;
    [SerializeField]
    private bool hasReceivedKey = false; // Kiểm tra xem người chơi đã nhận chìa khóa chưa
    [SerializeField]
    private bool keyroundruong = false; // nhân vật có chia khóa chạm vào vào rương hay chưa
    public GameObject keyposmidtrai;
    public GameObject keyposmidphai;

    private bool isOnGround1 = false;
    public GameObject tang2;
    public GameObject tang21;
    private bool isOnGround2 = false;
    public GameObject tang3;
    public GameObject tang31;
    private bool isOnGround3 = false;
    public GameObject tang4;
    public GameObject tang41;

    private bool isClimbing = false; // Trạng thái leo thang
    private float climbSpeed = 3f;  // Tốc độ leo thang 

    //   private PolygonCollider2D boxCollider2D;x
    // private  bool isColliderActive= true;
    public GameObject skill2;
    public GameObject skill2mo;
    public GameObject skill3;
    public GameObject skill3mo;
    public GameObject skill4;
    public GameObject skill4mo;
    public GameObject skillpow;
    public GameObject imagepow;
    public GameObject anhmopow;

    public GameManagermap2 gameManager;

    public Slider playerHealth;
    public Image fillImage;
    public Slider playerMana;
    public Image fillImagemana;




    public float moveSpeed = 15f;

    public float moveNgang;
    public float moveDoc;
    public Vector2 movement;
    public Joystick joystick;



    // am thanh
    public List<AudioClip> audioClips;
    private AudioSource audioSource;

    public audioManager audioManager;

    private float Cooldowncast = 3f;
    private float Cooldowncastpow = 5f;
    private float Cooldownkhien = 15f;



    public float jumpForce = 2f;
    private Rigidbody2D rb;
    private bool isGrounded;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    public GameObject bullet;
    public GameObject bulletpow;
    public Transform bulletPos;
    public static bool facingRight = true;





    private float lastCastTime = 0f;
    public GameObject swordCollider;
    public GameObject swordCollider1;
    public GameObject swordCollider2;
    public int torchCount = 0;  // Biến để lưu trữ số ngọn lửa (hoặc đuốc) của người chơi
    //hp player
    public int health = 10;
    public int mana = 10;


    public Image[] buttonImages; // Biến để tham chiếu đến Image của button



    public GameObject boxattack;
    public GameObject boxskill2;
    public GameObject boxskill3;

    void Start()
    {
        chiakhoa.SetActive(false);
        skill4.SetActive(false);
        tang2.SetActive(false);
        //  boxCollider2D = GetComponent<PolygonCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>(); // Gán Animator từ component
        skill2mo.SetActive(false);
        skill2.SetActive(true);
        skill3mo.SetActive(false);
        skill3.SetActive(true);
        skillpow.SetActive(false);
        skillpow.SetActive(false);
        //  anhmopow.SetActive(true);
        swordCollider.SetActive(false);
        swordCollider1.SetActive(false);
        swordCollider2.SetActive(false);

        playerHealth.interactable = false;
        playerMana.interactable = false;
        playerHealth.maxValue = health;

        playerMana.maxValue = mana;
        playerHealth.value = Mathf.Clamp(health, 0, playerHealth.maxValue);
        playerMana.value = Mathf.Clamp(mana, 0, playerMana.maxValue);

        audioSource = GetComponent<AudioSource>();
        Time.timeScale = 1;


        boxattack.SetActive(false);
        boxskill2.SetActive(false);
        boxskill3.SetActive(false);

        dungkhien.SetActive(false);

    }
    // bh fix loi xong them chu d vao

    void Update()
    {

        if (isOnGround1) // Kiểm tra nếu đang trên `ground1`
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                tang2.SetActive(true);
                tang21.SetActive(true);
                Debug.Log("Bật tầng 2");

            }
            else if (Input.GetKeyDown(KeyCode.A))
            {
                tang2.SetActive(false);
                Debug.Log("Tắt tầng 2");
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                tang21.SetActive(false);
                Debug.Log("Tắt tầng 2");
            }
        }
        if (isOnGround2) // Kiểm tra nếu đang trên `ground2`
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                tang3.SetActive(true);
                Debug.Log("Bật bac thang tầng 3");
                tang31.SetActive(true);
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                tang3.SetActive(false);
                Debug.Log("Tắt bac thang tang 3");
            }
            else if (Input.GetKeyDown(KeyCode.A))
            {
                tang31.SetActive(false);
                Debug.Log("Tắt bac thang tang 3");
            }
        }


        // Kiểm tra khi nhân vật đạt độ cao mong muốn
        if (isOnGround3 && Input.GetKeyDown(KeyCode.W))
        {

            StartCoroutine(thoigiannhaycaonhat());

        }

        Move();
        Jump();
        Attack();
        keyhp();
        keymp();

        // Kiểm tra nhấn phím Space để bắn đạn
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    PlayerShoot();
        //}



        if (playerMana.value > 0)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                Attack1(); // Gọi hàm Attack1
            }
            if (Input.GetKeyDown(KeyCode.Q))
            {
                Cast();
            }
            if (Input.GetKeyDown(KeyCode.T))
            {
                CastPow();
            }


        }
        else
        {
            Debug.Log("Không đủ năng lượng để sử dụng kĩ năng");
        }


        // Kiểm tra mana và cập nhật tất cả các button
        foreach (Image buttonImage in buttonImages)
        {
            UpdateButton(buttonImage);
        }
        // Di chuyển ngang
        float moveHorizontal = joystick.Horizontal + Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(moveHorizontal * climbSpeed, rb.velocity.y);

        // Kiểm tra leo thang
        if (isClimbing)
        {
            float moveVertical = joystick.Vertical + Input.GetAxis("Vertical"); // Nhấn phím lên/xuống
            rb.velocity = new Vector2(rb.velocity.x, moveVertical * climbSpeed);

            rb.gravityScale = 0; // Vô hiệu hóa trọng lực khi leo thang
            animator.SetBool("IsClimbing", true); // Gắn cờ leo thang trong Animator
                                                  // Kết hợp animation đi bộ khi nhân vật di chuyển trên cầu thang

        }
        else
        {
            rb.gravityScale = 1; // Khôi phục trọng lực khi không leo thang
            animator.SetBool("IsClimbing", false);


        }

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
    public void PlayerShootpow()
    {
        GameObject newBullet = Instantiate(bulletpow, bulletPos.position, Quaternion.identity);
        bulletpow bulletScript = newBullet.GetComponent<bulletpow>();

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
            StartCoroutine(boxvukhi());
            audioManager.Instance.PlaySFX("skill1");
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

    public void buttonattack()
    {
        animator.SetTrigger("attack");
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
        // Nếu là lần đầu sử dụng, cho phép ngay
        if (isFirstUse1 || Time.time - lastCastTime >= Cooldownkhien)
        {
            playerMana.value -= 1; // Giảm mana khi sử dụng
            audioManager.Instance.PlaySFX("skill2"); // Phát âm thanh
            lastCastTime = Time.time; // Lưu thời gian sử dụng
            isFirstUse1 = false; // Đánh dấu không còn lần đầu nữa
            StartCoroutine(thoigiandungkhien()); // Bắt đầu logic hiệu ứng
            StartCoroutine(DisableButtonForCooldown2()); // Hiển thị hồi chiêu
        }
        else
        {
            Debug.Log("Skill đang hồi chiêu!");
        }


    }
    void Attack3()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            animator.SetTrigger("block");
            playerMana.value -= 1;
            audioManager.Instance.PlaySFX("skill2");
            boxskill3.SetActive(true);

        }
    }
    public void buttonattack1()
    {
        if (playerMana.value > 0 || isFirstUse1)
        {
            // animator.SetTrigger("strike");
            playerMana.value -= 1;
            isFirstUse1 = false; // Đánh dấu không còn lần đầu nữa
            audioManager.Instance.PlaySFX("skill2");
            StartCoroutine(thoigiandungkhien());
            StartCoroutine(DisableButtonForCooldown2());
        }
        else
        {
            Debug.Log("Không đủ mana để sử dụng kĩ năng!");
        }
    }
    IEnumerator DisableButtonForCooldown2()
    {
        skill2mo.GetComponent<Button>().interactable = false;
        skill2.SetActive(false);
        skill2mo.SetActive(true);

        int cooldownTime2 = 15;         // Thời gian hồi chiêu (giây)
        while (cooldownTime2 > 0)
        {
            cooldownText2.text = cooldownTime2.ToString() + "s"; // Cập nhật Text đếm ngược
            yield return new WaitForSeconds(1f);         // Chờ 1 giây
            cooldownTime2--;                              // Giảm thời gian
        }

        cooldownText2.text = "";        // Xóa Text sau khi đếm ngược xong
        skill2.GetComponent<Button>().interactable = true; // Cho phép tương tác trở lại
        skill2mo.SetActive(false);     // Ẩn button mờ
        skill2.SetActive(true);
    }
    public void buttonattack2()
    {
        //  float Cooldown = 2f; // Cooldown 2 giây cho buttonattack2
        if (playerMana.value > 0 || isFirstUse)
        {
            animator.SetTrigger("cast");
            playerMana.value -= 2;
            lastCastTime = Time.time;
            audioManager.Instance.PlaySFX("bandan");
            isFirstUse = false; // Đánh dấu không còn lần đầu nữa

            // Làm mờ button và chạy cooldown
            StartCoroutine(DisableButtonForCooldown());
        }
        else
        {
            // Debug.Log("Không đủ mana để sử dụng kỹ năng!");
        }
    }

    public void buttonattack4()
    {
        //  float Cooldown = 5f; // Cooldown 5 giây cho buttonattack4
        if (playerMana.value > 0)
        {
            animator.SetTrigger("castpow");
            playerMana.value -= 2;
            lastCastTime = Time.time;
            audioManager.Instance.PlaySFX("bandan");

            // Làm mờ button và chạy cooldown
            StartCoroutine(DisableButtonForCooldown1());
        }
        else
        {
            // Debug.Log("Không đủ mana để sử dụng kỹ năng!");
        }
    }

    void Cast()
    {

        if (isFirstUse || Time.time - lastCastTime >= Cooldowncast)
        {
            animator.SetTrigger("cast");
            playerMana.value -= 2;
            lastCastTime = Time.time;
            isFirstUse = false;
            audioManager.Instance.PlaySFX("bandan");
            StartCoroutine(DisableButtonForCooldown());

        }
    }
    void CastPow()
    {

        if (isFirstUse || Time.time - lastCastTime >= Cooldowncastpow)
        {
            animator.SetTrigger("castpow");
            playerMana.value -= 2;
            lastCastTime = Time.time;
            isFirstUse = false;
            audioManager.Instance.PlaySFX("bandan");
            StartCoroutine(DisableButtonForCooldown1());
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
    IEnumerator DisableButtonForCooldown1()
    {
        skill4mo.GetComponent<Button>().interactable = false;
        skill4.SetActive(false);
        skill4mo.SetActive(true);

        int cooldownTime = 5;         // Thời gian hồi chiêu (giây)
        while (cooldownTime > 0)
        {
            cooldownText.text = cooldownTime.ToString() + "s"; // Cập nhật Text đếm ngược
            yield return new WaitForSeconds(1f);         // Chờ 1 giây
            cooldownTime--;                              // Giảm thời gian
        }

        cooldownText.text = "";        // Xóa Text sau khi đếm ngược xong
        skill4.GetComponent<Button>().interactable = true; // Cho phép tương tác trở lại
        skill4mo.SetActive(false);     // Ẩn button mờ
        skill4.SetActive(true);
    }

    public void buttonattack3()
    {
        if (playerMana.value > 0)
        {
            animator.SetTrigger("block");
            playerMana.value -= 1;
            audioManager.Instance.PlaySFX("skill2");
            boxskill3.SetActive(true);
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

            boxskill2.transform.localPosition = new Vector3(1.0f, boxskill2.transform.localPosition.y, boxskill2.transform.localPosition.z);
            boxskill2.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f); // Scale mặc định

            boxskill3.transform.localPosition = new Vector3(1.0f, boxskill3.transform.localPosition.y, boxskill3.transform.localPosition.z);
            boxskill3.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f); // Scale mặc định
        }
        else
        {
            // Hướng sang trái
            boxattack.transform.localPosition = new Vector3(-1.0f, boxattack.transform.localPosition.y, boxattack.transform.localPosition.z);
            boxattack.transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f); // Lật đối tượng theo trục X
                                                                             // Có thể lật góc của collider nếu cần
                                                                             // boxattack.transform.localRotation = Quaternion.Euler(0, 180, 0); // Quay boxattack 180 độ trên trục Y

            boxskill2.transform.localPosition = new Vector3(-1.0f, boxskill2.transform.localPosition.y, boxskill2.transform.localPosition.z);
            boxskill2.transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);

            boxskill3.transform.localPosition = new Vector3(-1.0f, boxskill3.transform.localPosition.y, boxskill3.transform.localPosition.z);
            boxskill3.transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
        }
    }


    //public void ShowSword()
    //{
    //    swordCollider.SetActive(true);

    //    UpdateSwordColliderPosition();
    //    swordCollider1.SetActive(false);
    //    swordCollider2.SetActive(false);

    //}

    //public void HideSword()
    //{
    //    swordCollider.SetActive(false);
    //    swordCollider1.SetActive(false);
    //    swordCollider2.SetActive(false);
    //}
    //public void ShowSword1()
    //{
    //    swordCollider1.SetActive(true);
    //    UpdateSwordColliderPosition();
    //    swordCollider.SetActive(false);
    //    swordCollider2.SetActive(false);
    //}

    //public void HideSword1()
    //{
    //    swordCollider1.SetActive(false);
    //    swordCollider.SetActive(false);
    //    swordCollider2.SetActive(false);
    //}
    //public void ShowSword2()
    //{
    //    swordCollider2.SetActive(true);
    //    UpdateSwordColliderPosition();
    //    swordCollider.SetActive(false);
    //    swordCollider1.SetActive(false);
    //}

    //public void HideSword2()
    //{
    //    swordCollider2.SetActive(false);
    //    swordCollider.SetActive(false);
    //    swordCollider1.SetActive(false);
    //}
    void UpdateTorchOrientation()
    {
        if (keyroundruong)
        {
            if (facingRight)
            {
                // Nếu nhân vật quay phải, vật phẩm quay phải (không xoay)
                // chiakhoa.transform.rotation = Quaternion.Euler(0, 180, 0);
                chiakhoa.GetComponent<SpriteRenderer>().flipX = false;
                chiakhoa.transform.position = keyposmidphai.transform.position;
            }
            else
            {
                // Nếu nhân vật quay trái, vật phẩm quay trái (xoay 180 độ)
                //  chiakhoa.transform.rotation = Quaternion.Euler(0, 180, 0);
                chiakhoa.transform.position = keyposmidtrai.transform.position;
                chiakhoa.GetComponent<SpriteRenderer>().flipX = true;
            }
        }
        else
        {
            if (facingRight)
            {
                // Nếu nhân vật quay phải, vật phẩm quay phải (không xoay)
                // chiakhoa.transform.rotation = Quaternion.Euler(0, 0, 0);
                chiakhoa.transform.position = keypostrai.transform.position;
                chiakhoa.GetComponent<SpriteRenderer>().flipX = false;
            }
            else
            {
                // Nếu nhân vật quay trái, vật phẩm quay trái (xoay 180 độ)
                //  chiakhoa.transform.rotation = Quaternion.Euler(0, 180, 0);
                chiakhoa.transform.position = keyposphai.transform.position;
                chiakhoa.GetComponent<SpriteRenderer>().flipX = true;
            }
        }
    }

    void Move()
    {
        float moveInput = joystick.Horizontal + Input.GetAxis("Horizontal");
        Debug.Log("Joystick Horizontal: " + joystick.Horizontal);
        if (Mathf.Abs(moveInput) > 0.1f)
        {

            rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
            Debug.Log("Velocity: " + rb.velocity);
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
            if (hasReceivedKey)
            {
                UpdateTorchOrientation(); // Cập nhật hướng của chìa khóa
            }

            animator.SetBool("isWalking", moveInput != 0);
            animator.SetBool("IsGround", isGrounded);
        }
        else
        {
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
            isGrounded = false;
            animator.SetTrigger("Jump");
            audioManager.Instance.PlaySFX("jump");
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("candauvan"))
        {
            isGrounded = true;
          //  animator.SetBool("crouch", true);
            originalScale = transform.localScale;
            transform.parent = collision.transform;
        }
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            Debug.Log("Chạm đất!");
            animator.SetBool("IsGround", true);
        }
        if (collision.gameObject.CompareTag("ground1"))
        {
            isGrounded = true;
            Debug.Log("Chạm đất!");
            animator.SetBool("IsGround", true);
            isOnGround1 = true;
        }
        if (collision.gameObject.CompareTag("ground2"))
        {
            isGrounded = true;
            Debug.Log("Chạm đất!");
            animator.SetBool("IsGround", true);
            isOnGround2 = true;
            tang4.SetActive(false);
        }
        if (collision.gameObject.CompareTag("ground3"))
        {
            isGrounded = true;
            Debug.Log("Chạm đất!");
            animator.SetBool("IsGround", true);
            isOnGround3 = true;
        }
        if (collision.gameObject.layer == LayerMask.NameToLayer("tang3"))
        {
            isGrounded = true;
            Debug.Log("Chạm đất!");
            tang3.SetActive(false);
        }
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        // Duy trì quan hệ cha-con trong khi tiếp xúc với vật thể di động
        if (collision.gameObject.CompareTag("candauvan"))
        {
            transform.parent = collision.transform;
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
            animator.SetBool("IsGround", false);
        }
        if (collision.gameObject.CompareTag("ground1"))
        {
            isGrounded = false;
            animator.SetBool("IsGround", false);
            isOnGround1 = false;
        }
        if (collision.gameObject.CompareTag("ground2"))
        {
            isGrounded = false;
            animator.SetBool("IsGround", false);
            isOnGround2 = false;
        }
        if (collision.gameObject.CompareTag("ground3"))
        {
            isGrounded = false;
            animator.SetBool("IsGround", false);
            isOnGround3 = false;
        }
        if (collision.gameObject.CompareTag("candauvan"))
        {
           // animator.SetBool("crouch", false);

            // Chuyển đổi vị trí về hệ toàn cục trước khi tháo parent
            Vector3 worldPosition = transform.position;
            transform.parent = null;
            transform.position = worldPosition;

            // Khôi phục scale ban đầu để đảm bảo hướng không bị đảo ngược
            transform.localScale = originalScale;

        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.CompareTag("cauthang")) // Nếu va chạm với thang
        {
            isClimbing = true;
            Debug.Log("Nhân vật bắt đầu leo thang.");
        }

        if (collision.gameObject.layer == LayerMask.NameToLayer("trap"))
        {
            animator.SetTrigger("dizzy");
            audioManager.Instance.PlaySFX("bidau");
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
            animator.SetTrigger("hurt");
            audioManager.Instance.PlaySFX("bidau");
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
                    //    audioSource.PlayOneShot(audioClips[1]);
                    // Time.timeScale = 0;
                };
            }
        }
        if (collision.gameObject.CompareTag("vatphamchiakhoa"))
        {
            StartCoroutine(DestroyTorchAfterDelay(collision.gameObject));


            audioManager.Instance.PlaySFX("anlua");

        }

        if (collision.gameObject.CompareTag("ruongkhobau"))
        {
            keyroundruong = true;

        }
        if (collision.gameObject.CompareTag("ruongdamo"))
        {
            chiakhoa.SetActive(false);
            gameManager.BlockScore();

            gameManager.SetScoreText();


        }
        if (collision.gameObject.CompareTag("skillpow"))
        {

            skill4.SetActive(true);
            skillpow.SetActive(false);
            anhmopow.SetActive(false);
            StartCoroutine(thoigiandeglogpow());

        }
        if (collision.gameObject.CompareTag("vatphamhoihp"))
        {
            audioManager.Instance.PlaySFX("anlua");
            gameManager.AddScorehp();
            gameManager.SetScoreTexthp();
            Destroy(collision.gameObject);


        }
        if (collision.gameObject.CompareTag("vatphamhoimp"))
        {
            audioManager.Instance.PlaySFX("anlua");
            gameManager.AddScoremp();
            gameManager.SetScoreTextmp();
            Destroy(collision.gameObject);


        }

        if (collision.gameObject.CompareTag("quaman3"))
        {
            SceneManager.LoadSceneAsync(4);
        }
    }

    void ReceiveKey()
    {
        hasReceivedKey = true; // Đánh dấu là đã nhận chìa khóa
        chiakhoa.SetActive(true); // Bật chìa khóa lên
        Debug.Log("Bạn vừa nhận được chìa khóa vạn năng!");
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("cauthang")) // Nếu rời khỏi thang
        {
            isClimbing = false;
            Debug.Log("Nhân vật rời khỏi thang.");
        }
        if (collision.gameObject.CompareTag("ruongkhobau"))
        {
            keyroundruong = false;
            hasReceivedKey = true;

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
        gameManager.AddScore();
        gameManager.SetScoreText();
        ReceiveKey();

    }


    IEnumerator thoigiannhaycaonhat()
    {
        yield return new WaitForSeconds(0.8f);
        tang4.SetActive(true); // Bật tầng 4
        tang41.SetActive(true);
        Debug.Log("Bật bậc thang tầng 4");

    }

    IEnumerator thoigiandeglogpow()
    {
        imagepow.SetActive(true);
        yield return new WaitForSeconds(2f);
        imagepow.SetActive(false);

    }
    IEnumerator thoigiandungkhien()
    {
        dungkhien.SetActive(true);
        yield return new WaitForSeconds(5f);
        dungkhien.SetActive(false);

    }
    // Coroutine để chờ 1 giây rồi cộng máu và phát âm thanh
    IEnumerator HealWithDelay()
    {
        yield return new WaitForSeconds(2f); // Chờ 1 giây
        audioManager.Instance.PlaySFX("hoimau");
        // Cộng máu theo mức độ
        if (playerHealth.value > 8 && playerHealth.value < 10) // Cộng 1 máu
        {
            playerHealth.value += 1;
            print("+1 máu");
        }
        else if (playerHealth.value > 7 && playerHealth.value < 9) // Cộng 2 máu
        {
            playerHealth.value += 2;
            print("+2 máu");
        }
        else if (playerHealth.value <= 7) // Cộng 3 máu
        {
            playerHealth.value += 3;
            print("+3 máu");
        }


        // Giới hạn HP không vượt quá 10
        if (playerHealth.value > 10)
        {
            playerHealth.value = 10;
        }

        // Cập nhật màu của fillImage dựa trên HP
        if (playerHealth.value >= 8)  // HP >= 8, thanh máu màu đỏ
        {
            fillImage.color = Color.red;
        }
        else if (playerHealth.value < 4)  // HP < 4, thanh máu màu xanh
        {
            fillImage.color = Color.green;
        }
        else  // HP từ 4 đến dưới 8, thanh máu màu vàng
        {
            fillImage.color = Color.yellow;
        }
    }
    public void sudungbinhhp()
    {
        if (playerHealth.value < 10 && gameManager.scorehp > 0) // Kiểm tra máu và số lượng bình HP
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

        if (playerMana.value < 10)
        {
            playerMana.value = Mathf.Min(10, playerMana.value + 3); // Cộng tối đa 3 MP
            print("+3 mp");
        }
    }


    public void sudungbinhmp()
    {
        if (playerMana.value < 10 && gameManager.scoremp > 0) // Kiểm tra mana và số lượng bình MP
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