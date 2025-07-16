using UnityEngine;

public class PushEnemy : MonoBehaviour
{
    public Transform enemy;       // Transform của quái
    public float pushDistance = 2f; // Khoảng cách đẩy
    public float pushSpeed = 5f;   // Tốc độ đẩy
    public float returnSpeed = 10f; // Tốc độ quay lại vị trí ban đầu

    private Vector2 targetPosition; // Vị trí mục tiêu khi bị đẩy
    private Vector2 originalPosition; // Vị trí ban đầu của quái
    private bool isPushing = false;
    private bool isReturning = false;

    public void PushEnemyForward(Vector2 direction)
    {
        if (isPushing || isReturning) return; // Nếu đang đẩy hoặc quay lại thì không làm gì
        isPushing = true;

        // Lưu vị trí ban đầu
        originalPosition = enemy.position;

        // Tính vị trí mục tiêu
        targetPosition = (Vector2)enemy.position + direction.normalized * pushDistance;
    }

    private void Update()
    {
        if (isPushing)
        {
            // Di chuyển quái về phía mục tiêu
            enemy.position = Vector2.MoveTowards(enemy.position, targetPosition, pushSpeed * Time.deltaTime);

            // Nếu đã đến vị trí mục tiêu, dừng lại và bắt đầu quay lại
            if (Vector2.Distance(enemy.position, targetPosition) < 0.01f)
            {
                isPushing = false;
                isReturning = true;
            }
        }
        else if (isReturning)
        {
            // Di chuyển quái quay lại vị trí ban đầu nhanh chóng
            enemy.position = Vector2.MoveTowards(enemy.position, originalPosition, returnSpeed * Time.deltaTime);

            // Nếu đã quay lại vị trí ban đầu, dừng lại
            if (Vector2.Distance(enemy.position, originalPosition) < 0.01f)
            {
                isReturning = false;
            }
        }
    }
}
