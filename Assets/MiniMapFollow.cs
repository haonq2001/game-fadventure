using UnityEngine;

public class MiniMapFollow : MonoBehaviour
{
    public Transform player; // Gắn nhân vật của bạn vào đây
    public Vector3 offset = new Vector3(0, 50, 0);

    void LateUpdate()
    {
        if (player != null)
        {
            Vector3 newPosition = player.position + offset;
            transform.position = newPosition;
        }
    }
}