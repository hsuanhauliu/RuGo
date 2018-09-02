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
                Vector3 finalizedPosition = hit.point;
                finalizedPosition.y = 0.025f;

                GameObject d = Instantiate(domino, this.transform);
                d.transform.position = finalizedPosition;

                dominos.Add(d);

                if (dominos.Count == 1) {
                    startPosition = finalizedPosition;
                }
                else {
                    endPosition = finalizedPosition;
                }
            }
        }
        if (Input.GetKeyDown("space"))
        {
            Transform t = domino.transform;
            Vector3 pathDirection = endPosition - startPosition;
            Vector3 normalizedPath = pathDirection.normalized;

            for (float i = dominoSpacing; i < (pathDirection.magnitude - dominoSpacing); i += dominoSpacing)
            {
                GameObject d = Instantiate(domino, this.transform);
                d.transform.position = startPosition + (normalizedPath * i);
                dominos.Add(d);
            }

            dominos.ForEach((GameObject d) =>
            {
                d.transform.rotation = Quaternion.LookRotation(pathDirection);
            });
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            Debug.Log("Knocking over domino");
            GameObject firstDomino = dominos[0];
            Rigidbody body = firstDomino.GetComponent<Rigidbody>();
            body.AddForce(firstDomino.transform.forward * pushForce);

            dominos.Clear();
        }
    }
}
