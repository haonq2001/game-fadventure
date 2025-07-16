using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CandauVanCollider : MonoBehaviour
{
    public float moveSpeed = 5f;     // Tốc độ di chuyển
    public float moveDistance = 5f;  // Khoảng cách di chuyển
    private Vector3 initialPosition;
    private bool isMovingRight = true;

    private void Start()
    {
        initialPosition = transform.position;
    }

    private void Update()
    {
        // Di chuyển đối tượng sang trái hoặc phải
        if (isMovingRight)
        {
            transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);
        }
        else
        {
            transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);
        }

        // Kiểm tra giới hạn và đổi hướng nếu cần
        if (transform.position.x >= initialPosition.x + moveDistance)
        {
            isMovingRight = false;
            Flip(); // Lật đối tượng
            transform.position = new Vector3(initialPosition.x + moveDistance, transform.position.y, transform.position.z);
        }
        else if (transform.position.x <= initialPosition.x)
        {
            isMovingRight = true;
            Flip(); // Lật đối tượng
            transform.position = new Vector3(initialPosition.x, transform.position.y, transform.position.z);
        }
    }

    // Lật đối tượng
    private void Flip()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1; // Đảo chiều trục X
        transform.localScale = scale;
    }
}