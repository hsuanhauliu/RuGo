using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour {
    public Transform LeftPole;
    public Transform RightPole;
    public int LaserId;

	void Start () {
        LineRenderer lineRenderer = this.GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, LeftPole.GetChild(LaserId).position - transform.position);
        lineRenderer.SetPosition(1, RightPole.GetChild(LaserId).position - transform.position);
	}

}
