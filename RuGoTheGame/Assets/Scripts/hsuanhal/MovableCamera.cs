/* A simple script for moving the camera around to simulate the player's movement.
 * Usage: attach it to the main camera and initialize the speed. 3 is recommended. */

using UnityEngine;

public class MovableCamera : MonoBehaviour {
    public int speed;
    public float turnSpeed = 50.0f;

    // Update is called once per frame
    void Update ()
    {
        // Movement
        // X-axis
        if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.Translate(new Vector3(speed * Time.deltaTime, 0, 0));
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.Translate(new Vector3(-speed * Time.deltaTime, 0, 0));
        }
        // Z-axis
        if (Input.GetKey(KeyCode.UpArrow))
        {
            transform.Translate(new Vector3(0, 0, speed * Time.deltaTime));
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            transform.Translate(new Vector3(0, 0, -speed * Time.deltaTime));
        }
        // Y-axis
        if (Input.GetKey(KeyCode.X))
        {
            transform.Translate(new Vector3(0, speed * Time.deltaTime, 0));
        }
        if (Input.GetKey(KeyCode.Z))
        {
            transform.Translate(new Vector3(0, -speed * Time.deltaTime, 0));
        }

        // Rotation
        if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(Vector3.up * turnSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.down * turnSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.W))
        {
            transform.Rotate(Vector3.left * turnSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.Rotate(Vector3.right * turnSpeed * Time.deltaTime);
        }
    }
}
