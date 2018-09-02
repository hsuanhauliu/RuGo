// a sample code from: https://www.youtube.com/watch?v=VBZFYGWvm4A

using UnityEngine;

public class GadgetPlacer : MonoBehaviour
{
    // create a grid first
    private Grid grid;

    private void Awake()
    {
        grid = FindObjectOfType<Grid>();
    }
	
	// Update is called once per frame
	private void Update ()
    {
		//Check if there is a mouse click
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hitInfo;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hitInfo))
            {
                PlaceGadgetNear(hitInfo.point);
            }
        }
	}

    // create a object at nearest grid. spawning cube for now
    private void PlaceGadgetNear(Vector3 clickPoint)
    {
        var finalPosition = grid.GetNearestPointOnGrid(clickPoint);
        GameObject.CreatePrimitive(PrimitiveType.Cube).transform.position = finalPosition;
    }
}
