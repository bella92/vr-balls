using UnityEngine;
using System.Collections;

public class HeadBall : MonoBehaviour
{
    // Params
    float moveSpeed = 1; // Move speed
    float rotationSpeed = 100; // Speed of turning    

    // Find game objects
    public Vector3[] points;
    private int currentPointIndex = 0;
    private Vector3 destinationPoint;

    void FixedUpdate()
    {
        if (points != null)
        {
            // If the distance is over 5 units
            if (Vector3.Distance(destinationPoint, transform.position) < 0.03f)
            {
                ChangeDestinationPoint();
            }
            else
            {
                Vector3 direction = destinationPoint - transform.position;
                // Rotate towards player
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), rotationSpeed * Time.deltaTime);

                // Move forward at specified speed
                transform.position += transform.forward * moveSpeed * Time.deltaTime;
            }
        }
    }

    public void SetPoints(Vector3[] newPoints)
    {
        points = newPoints;
        destinationPoint = points[0];
    }

    private void ChangeDestinationPoint()
    {
        currentPointIndex += 1;

        if (currentPointIndex >= points.Length)
        {
            Debug.Log("Gameover");
        }
        else
        {
            destinationPoint = points[currentPointIndex];
        }
    }
}
