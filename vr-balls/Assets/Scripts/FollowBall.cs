using UnityEngine;
using System.Collections;

public class FollowBall : MonoBehaviour
{
    // Params
    float moveSpeed = 1; // Move speed
    float rotationSpeed = 500; // Speed of turning    

    // Find game objects
    public GameObject ball;

    public void SetBall(GameObject newBall)
    {
        ball = newBall;
    }

    void FixedUpdate()
    {
        if (ball != null)
        {
            float diameter = ball.transform.localScale.x;
            // If the distance is over 5 units
            if (Vector3.Distance(ball.transform.position, transform.position) > diameter)
            {
                Vector3 direction = ball.transform.position - transform.position;

                // Rotate towards player
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), rotationSpeed * Time.deltaTime);

                // Move forward at specified speed
                transform.position += transform.forward * moveSpeed * Time.deltaTime;
            }
        }
    }
}
