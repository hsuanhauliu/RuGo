using UnityEngine;
using System;

public class CameraController : MonoBehaviour
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
