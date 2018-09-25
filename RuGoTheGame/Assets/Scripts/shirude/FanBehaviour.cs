using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FanBehaviour : MonoBehaviour {

    public float rotateSpeed = 1500.0f;
    public float windMin = 20f;
    public float windMax = 40f;
    public float windFrequency = 0.6f;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        transform.Rotate(0, rotateSpeed * Time.deltaTime, 0);
	}

    void FixedUpdate()
    {
        Vector3 windDirection = Vector3.left;

        float currentWindForce = windMin + (windMax - windMin) * Mathf.PerlinNoise(Time.time * windFrequency, 0);

        GetComponent<Rigidbody>().AddForce(currentWindForce * windDirection);
    }

}
