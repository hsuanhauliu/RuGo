using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DominoManager : MonoBehaviour
{

    public GameObject domino;

    public float dominoSpacing = 0.025f;

    public float pushForce = 0.25f;

    private List<GameObject> dominos;

    private Vector3 startPosition;

    private Vector3 endPosition;

    // Use this for initialization
    void Start()
    {
        dominos = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //Convert Mouse Screen Coordinates to Ray in 3D Space
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                GameObject d = Instantiate(domino, this.transform);
                d.transform.position = hit.point;

                //TODO FIX FALL THROUGH DOMINOS
                Vector3 position = d.transform.position;
                position.y = 0.025f;
                dominos.Add(d);

                if (dominos.Count == 1) {
                    startPosition = position;
                }
                else {
                    endPosition = position;
                }
            }
        }
        if (Input.GetKeyDown("space"))
        {
            //TODO: Account for Rotation
            Transform t = domino.transform;
            Vector3 pathDirection = endPosition - startPosition;
            Vector3 normalizedPath = pathDirection.normalized;

            for (float i = dominoSpacing; i < (pathDirection.magnitude - dominoSpacing); i += dominoSpacing)
            {
                GameObject d = Instantiate(domino, this.transform);
                d.transform.position = startPosition + (normalizedPath * i);
            }
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            Debug.Log("Knocking over domino");
            GameObject firstDomino = dominos[0];
            Rigidbody body = firstDomino.GetComponent<Rigidbody>();
            body.AddForce(firstDomino.transform.forward * pushForce);
        }
    }
}
