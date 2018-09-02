using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallInstantiation : MonoBehaviour
{
    public Transform brick;

    // build a wall at the origin
    void Start()
    {
        for (int y = 0; y < 5; y++)
        {
            for (int x = 0; x < 5; x++)
            {
                Instantiate(brick, new Vector3(x, (float)(y + 0.5), 0), Quaternion.identity);
            }
        }
    }
}