using UnityEngine;
using System;

public class BallBehaviour : MonoBehaviour
{

    public float speed = 900.0f;
    public GameObject sphere;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //Convert Mouse Screen Coordinates to Ray in 3D Space
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                Vector3 finalizedPosition = hit.point;
                finalizedPosition.y = 0.025f;

                GameObject s = Instantiate(sphere, this.transform);

            }
        }

    }

    void FixedUpdate()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);

        GetComponent<Rigidbody>().AddForce(movement * speed * Time.deltaTime);
        if (Input.GetKey("w")) GetComponent<Rigidbody>().AddForce(Vector3.up * speed * Time.deltaTime);
    }

}
