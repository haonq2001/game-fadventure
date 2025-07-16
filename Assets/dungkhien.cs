using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dungkhien : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform poskhien; 
    public Vector3 offset; 

    void Update()
    {
        if (poskhien != null)
        {
            // Cập nhật vị trí của thanh máu theo Boss
            transform.position = poskhien.position + offset;
        }
    }
}
