using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    // ������ת�ٶ�
    public float rotationSpeed = 10f;

    void Update()
    {
        // ÿ֡����ʱ�������ת�Ƕ�
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
    }
}
