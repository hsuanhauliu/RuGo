using UnityEngine;
using System;

public class CameraControlBall : MonoBehaviour
{
    public GameObject player;
    Vector3 offset;

    void Start()
    {
        offset = transform.position;
    }

    void LateUpdate()
    {
        transform.position = player.transform.position + offset;
    }
}
