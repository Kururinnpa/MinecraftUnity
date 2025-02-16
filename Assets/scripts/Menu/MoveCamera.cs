using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    // 控制旋转速度
    public float rotationSpeed = 10f;

    void Update()
    {
        // 每帧根据时间更新旋转角度
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
    }
}
