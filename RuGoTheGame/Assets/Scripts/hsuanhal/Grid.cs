// a sample code from: https://www.youtube.com/watch?v=VBZFYGWvm4A

using UnityEngine;

public class Grid : MonoBehaviour
{
    [SerializeField]
    private float gridSize = 1f;
	
    public Vector3 GetNearestPointOnGrid(Vector3 position)
    {
        position -= transform.position;

        int xCount = Mathf.RoundToInt(position.x / gridSize);
        int yCount = Mathf.RoundToInt(position.y / gridSize);
        int zCount = Mathf.RoundToInt(position.z / gridSize);

        Vector3 result = new Vector3(
            (float)xCount * gridSize,
            (float)yCount * gridSize,
            (float)zCount * gridSize);

        result += transform.position;

        return result;
    }

    // draw visual points on grid for reference. Not working for some reason...
    private void OnGrawGizmos()
    {
        Gizmos.color = Color.yellow;
        for (float x = 0; x < 40; x += gridSize)
        {
            for (float z = 0; z < 40; z += gridSize)
            {
                var point = GetNearestPointOnGrid(new Vector3(x, 0f, z));
                Gizmos.DrawSphere(point, 0.1f);
            }
        }
    }
}
